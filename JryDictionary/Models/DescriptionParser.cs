using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Jasily;
using JryDictionary.Models.DocPlugins;
using JryDictionary.Models.Parsers;

// ReSharper disable InconsistentNaming

namespace JryDictionary.Models
{
    public class DescriptionParser
    {
        private readonly string[] lines;
        private readonly List<Inline> inlines = new List<Inline>();
        private readonly List<string> galleries = new List<string>();

        public DescriptionParser(string text)
        {
            this.lines = text.AsLines();
        }

        public DescriptionParser ParseBody()
        {
            this.inlines.Clear();
            var index = 0;
            while (index < this.lines.Length && string.IsNullOrWhiteSpace(this.lines[index]))
            {
                index++;
            }
            for (var i = index; i < this.lines.Length; i++)
            {
                var line = this.lines[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    this.inlines.Add(new LineBreak());
                    this.inlines.Add(this.Height(20));
                }
                else
                {
                    var range = line.AsRange();
                    var trim = range.Trim();
                    if (trim == "---")
                    {
                        this.inlines.Add(this.Line());
                    }
                    else if (trim.StartsWith('#'))
                    {
                        var count = trim.TakeWhile(z => z == '#').Count();
                        count = Math.Min(6, count);
                        this.AddHeader(count, trim.SubRange(count).ToString());
                    }
                    else if (trim.StartsWith("{{") && trim.EndsWith("}}"))
                    {
                        IDocPlugin plugin = null;
                        var header = trim.SubRange(2, trim.Length - 4);
                        if (header.StartsWith("gallery:", StringComparison.OrdinalIgnoreCase))
                        {
                            plugin = new GalleryPlugin(header.ToString());
                        }
                        if (plugin != null)
                        {
                            var end = i + 1;
                            for (; end < this.lines.Length; end++)
                            {
                                if (this.lines[end].AsRange().Trim() == "{{}}") break;
                            }
                            this.inlines.AddRange(plugin.ParseLine(this.lines.Skip(i + 1).Take(end - i - 1).ToArray()));
                            i = end;
                        }
                    }
                    else
                    {
                        this.inlines.Add(new Run(trim.InsertToEnd(' ').GetString()));
                    }
                }
            }

            if (this.galleries.Count > 0)
            {
                this.AddHeader(2, "Galleries");
                this.CreateGallery(this.galleries, this.Cover == null ? 4 : 3);
            }

            return this;
        }

        private void CreateGallery(List<string> urls, int columnCount)
        {
            var rowCount = urls.Count / columnCount + (urls.Count % columnCount != 0 ? 1 : 0);

            var grid = new Grid();
            grid.ColumnDefinitions.AddRange(Generater.Create<ColumnDefinition>(columnCount));
            grid.RowDefinitions.AddRange(Generater.Create<RowDefinition>(rowCount));

            foreach (var gallery in urls.EnumerateIndexValuePair())
            {
                var uri = new Uri(gallery.Value);
                if (uri.Scheme == Uri.UriSchemeFile)
                {
                    if (!File.Exists(gallery.Value)) continue;
                }

                var image = new Image
                {
                    Source = UriInfo.BitmapFromUri(uri),
                    Margin = new Thickness(2),
                    Tag = gallery.Value
                };
                image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
                RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
                var col = gallery.Index % columnCount;
                var row = gallery.Index / columnCount;
                Grid.SetColumn(image, col);
                Grid.SetRow(image, row);
                grid.Children.Add(image);
            }

            this.inlines.Add(new InlineUIContainer(grid));
        }

        public static void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var image = sender as Image;
                if (image == null) return;
                var path = image.Tag as string;
                if (path != null)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            if (File.Exists(path))
                            {
                                using (Process.Start(path)) { }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    });
                }
            }
        }

        private void MapBackground(string line)
            => this.Background =
            Singleton.Instance<Parsers.UriParser>().TryParse(line.Substring(3))?.Uri.OriginalString
            ?? this.Background;

        public string Background { get; private set; }

        public string Cover { get; private set; }

        private void MapCover(string line)
            => this.Cover =
            Singleton.Instance<Parsers.UriParser>().TryParse(line.Substring(3))?.Uri.OriginalString
            ?? this.Cover;

        private void MapGalleries(string line)
        {
            var uri = Singleton.Instance<Parsers.UriParser>().TryParse(line.Substring(3))?.Uri;
            if (uri == null) return;
            this.galleries.Add(uri.OriginalString);
        }

        public string Logo { get; private set; }

        private void MapLogo(string line)
            => this.Logo =
            Singleton.Instance<Parsers.UriParser>().TryParse(line.Substring(3))?.Uri.OriginalString
            ?? this.Logo;

        public Inline[] Inlines => this.inlines.ToArray();

        private void AddHeader(int level, string text)
        {
            var fontSize = 32 - 3 * level;

            this.inlines.Add(new Run(text)
            {
                FontSize = fontSize
            });

            switch (level)
            {
                case 1:
                    this.inlines.Add(this.Line());
                    break;

                case 2:
                    this.inlines.Add(this.Line(Brushes.LightGray));
                    break;
            }
        }

        private Inline Line(Brush brush = null) => new InlineUIContainer(new Border
        {
            Width = 10000,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(3),
            BorderBrush = brush ?? Brushes.Black,
            BorderThickness = new Thickness(0, 1, 0, 0),
            SnapsToDevicePixels = true,
            UseLayoutRounding = true,
        });

        private Inline Height(double height) => new InlineUIContainer(new Grid { Height = height });
    }
}
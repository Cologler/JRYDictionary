using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jasily;
using JryDictionary.Common;

// ReSharper disable InconsistentNaming

namespace JryDictionary.Models
{
    public class DescriptionParser
    {
        private static readonly Regex WebUriRegex = new Regex("^(https?://[^ ]+)", RegexOptions.IgnoreCase);

        private readonly string text;
        private readonly string[] lines;
        private readonly int metaSpliterIndex;
        private readonly int contentStartIndex;
        private readonly List<Inline> inlines = new List<Inline>();
        private FormatType format;
        private readonly List<string> galleries = new List<string>();
        private bool isMetaParsed;

        public DescriptionParser(string text)
        {
            this.text = text;
            this.lines = text.AsLines();
            this.metaSpliterIndex = this.lines.FindIndex(z => z == "-");
            this.contentStartIndex = this.metaSpliterIndex + 1;
        }

        public DescriptionParser ParseMetaData()
        {
            if (this.metaSpliterIndex >= 0)
            {
                for (var i = 0; i < this.metaSpliterIndex; i++)
                {
                    var line = this.lines[i];
                    if (line.Length > 0)
                    {
                        switch (line[0])
                        {
                            case 'B':
                                if (line.StartsWith("BG:"))
                                {
                                    this.MapBackground(line);
                                }
                                break;

                            case 'C':
                                if (line.StartsWith("CV:"))
                                {
                                    this.MapCover(line);
                                }
                                break;

                            case 'F':
                                if (line.StartsWith("FM:"))
                                {
                                    this.FM(line);
                                }
                                break;

                            case 'G':
                                if (line.StartsWith("GL:"))
                                {
                                    this.MapGalleries(line);
                                }
                                break;

                            case 'L':
                                if (line.StartsWith("LG:"))
                                {
                                    this.MapLogo(line);
                                }
                                break;
                        }
                    }
                }
            }

            this.isMetaParsed = true;
            return this;
        }

        public DescriptionParser ParseBody()
        {
            Debug.Assert(this.isMetaParsed);

            switch (this.format)
            {
                case FormatType.PlainText:
                    this.BuildForPlainText();
                    break;

                case FormatType.Markdown:
                    this.BuildForMarkdown();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (this.galleries.Count > 0)
            {
                this.AddHeader(2, "Galleries");

                var columnCount = this.Cover == null ? 4 : 3;
                var rowCount = this.galleries.Count / columnCount + (this.galleries.Count % columnCount != 0 ? 1 : 0);

                var grid = new Grid();
                grid.ColumnDefinitions.AddRange(Generater.Create<ColumnDefinition>(columnCount));
                grid.RowDefinitions.AddRange(Generater.Create<RowDefinition>(rowCount));

                foreach (var gallery in this.galleries.EnumerateIndexValuePair())
                {
                    var image = new Image
                    {
                        Source = BitmapFromUri(new Uri(gallery.Value)),
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

            return this;
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

        private void FM(string line)
        {
            switch (line.Replace(" ", string.Empty))
            {
                case "FM:MD":
                    this.format = FormatType.Markdown;
                    break;
            }
        }

        private enum FormatType
        {
            PlainText,

            Markdown
        }

        private void MapBackground(string line) => this.Background = this.GetUri(line.AsRange().SubRange(3)) ?? this.Background;

        public string Background { get; private set; }

        public string Cover { get; private set; }

        private void MapCover(string line) => this.Cover = this.GetUri(line.AsRange().SubRange(3)) ?? this.Cover;

        private void MapGalleries(string line)
        {
            var uri = this.GetUri(line.AsRange().SubRange(3));
            if (uri == null) return;
            this.galleries.Add(uri);
        }

        public string Logo { get; private set; }

        private void MapLogo(string line) => this.Logo = this.GetUri(line.AsRange().SubRange(3)) ?? this.Logo;

        private string GetUri(StringRange line)
        {
            line = line.Trim();
            var match = WebUriRegex.Match(line.Trim().ToString());
            if (match.Success)
            {
                return match.Value;
            }
            if (line.StartsWith("file:///", StringComparison.OrdinalIgnoreCase))
            {
                var ret = line.SubRange(8).ToString();
                // onedrive
                if (ret.Contains("%onedrive%", StringComparison.OrdinalIgnoreCase))
                {
                    ret = ret.Replace("%onedrive%", FolderHelper.GetOneDriveLocation(),
                        StringComparison.OrdinalIgnoreCase);
                }
                return ret;
            }
            return null;
        }

        private void BuildForPlainText()
        {
            this.inlines.Clear();
            for (var i = this.contentStartIndex; i < this.lines.Length; i++)
            {
                var line = this.lines[i];
                this.inlines.Add(new Run(line));
                this.inlines.Add(new LineBreak());
            }
        }

        private void BuildForMarkdown()
        {
            this.inlines.Clear();
            var index = this.contentStartIndex;
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
                    else
                    {
                        this.inlines.Add(new Run(trim.InsertToEnd(' ').GetString()));
                    }
                }
            }
        }

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

        private static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
    }
}
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
            var uriParser = new Parsers.UriParser();
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
                    if (trim.All(z => z == '-'))
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
                        var header = trim.SubRange(2, trim.Length - 4).ToString();
                        var plugin = GalleryPlugin.TryCreate(header) ?? MediaPlugin.TryCreate(header);
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
                        var uri = uriParser.TryParse(trim.ToString());
                        if (uri != null)
                        {
                            this.inlines.Add(this.Hyperlink(uri.Uri, uri.Name));
                        }
                        else
                        {
                            this.inlines.Add(new Run(trim.InsertToEnd(' ').GetString()));
                        }
                    }
                }
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

        private Inline Hyperlink(Uri url, string name)
        {
            var link = new Hyperlink(new Run(name))
            {
                NavigateUri = url
            };
            link.RequestNavigate += (sender, e) =>
            {
                try
                {
                    Process.Start(e.Uri.ToString());
                }
                catch
                {
                    // ignored
                }
            };
            return link;
        }
    }
}
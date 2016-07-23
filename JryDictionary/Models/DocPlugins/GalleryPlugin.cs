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

namespace JryDictionary.Models.DocPlugins
{
    public sealed class GalleryPlugin : IDocPlugin
    {
        private static readonly Regex Pattern = new Regex(
            "^gallery(?::(?:col=(\\d))?)?$",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private readonly int columnCount = 3;

        public GalleryPlugin(string header)
        {
            var match = Pattern.Match(header.Replace(" ", ""));
            if (!match.Success) return;
            var col = match.Groups[1].Value;
            this.columnCount = Math.Max(1, Math.Min(int.Parse(col), 8));
        }

        public IEnumerable<Inline> ParseLine(string[] line)
        {
            var rowCount = line.Length / this.columnCount + (line.Length % this.columnCount != 0 ? 1 : 0);

            var grid = new Grid();
            grid.ColumnDefinitions.AddRange(Generater.Create<ColumnDefinition>(this.columnCount));
            grid.RowDefinitions.AddRange(Generater.Create<RowDefinition>(rowCount));

            foreach (var gallery in line.EnumerateIndexValuePair())
            {
                var url = DescriptionParser.GetUri(gallery.Value.AsRange());
                if (url == null) continue;
                var uri = new Uri(url);
                if (uri.Scheme == Uri.UriSchemeFile)
                {
                    if (!File.Exists(url)) continue;
                }

                var image = new Image
                {
                    Source = BitmapFromUri(uri),
                    Margin = new Thickness(2),
                    Tag = url
                };
                image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
                RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
                var col = gallery.Index % this.columnCount;
                var row = gallery.Index / this.columnCount;
                Grid.SetColumn(image, col);
                Grid.SetRow(image, row);
                grid.Children.Add(image);
            }

            yield return new InlineUIContainer(grid);
        }

        private static void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

        private static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        public void Dispose()
        {

        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Jasily;
using JryDictionary.Controls.ImagesViewer;

namespace JryDictionary.Models.DocPlugins
{
    public sealed class GalleryPlugin : IDocPlugin, IEnumerable<Uri>
    {
        private static readonly Regex Pattern = new Regex(
            "^gallery(?::(?:col=(\\d))?)?$",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private readonly int columnCount = 3;
        private readonly List<Uri> images = new List<Uri>();

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

            foreach (var item in line
                .Select(z => DescriptionParser.GetUri(z.AsRange()))
                .Where(z => z != null)
                .EnumerateIndexValuePair())
            {
                var uri = item.Value;

                if (uri.Scheme == Uri.UriSchemeFile && !File.Exists(uri.LocalPath)) continue;
                this.images.Add(uri);
                var image = new Image
                {
                    Source = DescriptionParser.BitmapFromUri(uri),
                    Margin = new Thickness(2),
                    Tag = uri
                };
                image.ToolTip = uri;
                image.MouseLeftButtonDown += this.Image_MouseLeftButtonDown;
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
                RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
                var col = item.Index % this.columnCount;
                var row = item.Index / this.columnCount;
                Grid.SetColumn(image, col);
                Grid.SetRow(image, row);
                grid.Children.Add(image);
            }

            yield return new InlineUIContainer(grid);
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var image = sender as Image;
                if (image == null) return;
                var uri = image.Tag as Uri;
                if (uri != null)
                {
                    var owner = App.Current.Windows.OfType<Window>().FirstOrDefault(z => z.IsActive);
                    var win = new ImagesViewerWindow
                    {
                        DataContext = this,
                        Owner = owner
                    };
                    var index = this.images.IndexOf(uri);
                    win.Show();
                    win.SelectedIndex = index;
                }
            }
        }

        public void Dispose()
        {

        }

        public IEnumerator<Uri> GetEnumerator() => this.images.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
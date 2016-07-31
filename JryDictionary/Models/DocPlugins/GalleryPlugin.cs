using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using JryDictionary.Controls.ImagesViewer;
using JryDictionary.Controls.MediaPlayer;
using JryDictionary.Models.Parsers;

namespace JryDictionary.Models.DocPlugins
{
    public sealed class GalleryPlugin : IDocPlugin
    {
        private static readonly Regex Pattern = new Regex(
            @"^gallery(?::(?:col=(\d))?)?$",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private readonly int columnCount = 3;
        private readonly List<UriInfo> items = new List<UriInfo>();

        private GalleryPlugin(string header)
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

            var imageUriParser = new ImageUriParser();

            foreach (var item in line
                .Select(z => imageUriParser.TryParse(z))
                .Where(z => z != null)
                .EnumerateIndexValuePair())
            {
                var uri = item.Value.Uri;
                if (uri.IsFile && !File.Exists(uri.LocalPath)) continue;
                this.items.Add(item.Value);
                var image = new Image
                {
                    Source = item.Value.CreateImageSource(),
                    Margin = new Thickness(2),
                    Tag = item.Value
                };
                image.ToolTip = string.IsNullOrWhiteSpace(item.Value.Name)
                    ? uri.ToString()
                    : $"{item.Value.Name}\r\n{uri}";
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
                var uri = image.Tag as UriInfo;
                if (uri != null)
                {
                    var items = this.items.Select(z => new ImagesViewerItemViewModel(z.Uri) { Name = z.Name }).ToArray();
                    ImagesViewerWindow.Show(items, items.FirstOrDefault(z => z.Uri == uri.Uri));
                }
            }
        }

        public void Dispose()
        {

        }

        public static IDocPlugin TryCreate(string header)
            => header.StartsWith("gallery", StringComparison.OrdinalIgnoreCase) ? new GalleryPlugin(header) : null;
    }

    public sealed class MediaPlugin : IDocPlugin
    {
        private static readonly Regex Pattern = new Regex(
            @"^media(?::(autoplay)?)?$",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private readonly bool isAutoPlay;

        public MediaPlugin(string header)
        {
            var match = Pattern.Match(header.Replace(" ", ""));
            if (!match.Success) return;
            this.isAutoPlay = match.Groups[1].Success;
        }

        public void Dispose()
        {
            
        }

        public IEnumerable<Inline> ParseLine(string[] lines)
        {
            var uriParser = new ImageUriParser();

            foreach (var line in lines)
            {
                var uri = uriParser.TryParse(line);
                if (uri == null) continue;

                var player = new MediaPlayerControl
                {
                    IsAutoPlay = this.isAutoPlay,
                    DisplayName = uri.Name ?? string.Empty
                };
                player.SetSource(uri.Uri);
                yield return new InlineUIContainer(player);
            }
        }

        public static IDocPlugin TryCreate(string header)
            => header.StartsWith("media", StringComparison.OrdinalIgnoreCase) ? new MediaPlugin(header) : null;
    }
}
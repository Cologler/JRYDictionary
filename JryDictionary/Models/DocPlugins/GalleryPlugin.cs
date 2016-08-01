using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using JryDictionary.Controls.ImagesViewer;
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
                var imageCreator = new ImagesCreator();
                var image = imageCreator.AddImage(item.Value);
                if (image == null) continue;
                var col = item.Index % this.columnCount;
                var row = item.Index / this.columnCount;
                Grid.SetColumn(image, col);
                Grid.SetRow(image, row);
                grid.Children.Add(image);
            }

            yield return new InlineUIContainer(grid);
        }

        public void Dispose()
        {

        }

        public static IDocPlugin TryCreate(string header)
            => header.StartsWith("gallery", StringComparison.OrdinalIgnoreCase) ? new GalleryPlugin(header) : null;
    }
}
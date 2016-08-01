using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using JryDictionary.Controls.MediaPlayer;
using JryDictionary.Models.Parsers;

namespace JryDictionary.Models.DocPlugins
{
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
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace JryDictionary.Models.Parsers
{
    public class UriParser
    {
        private static readonly Regex MarkdownUrlRegex = new Regex(
            @"^\[
                    (.*)
               \]
               \(
                    (.*)
               \)$",
            RegexOptions.IgnorePatternWhitespace);

        public virtual UriInfo TryParse(string text)
        {
            if (text == null) return null;
            text = text.Trim();
            var uri = GetUri(text);
            if (uri != null) return new UriInfo(uri);
            var m = MarkdownUrlRegex.Match(text);
            if (m.Success)
            {
                uri = GetUri(m.Groups[2].Value);
                if (uri != null) return new UriInfo(uri, m.Groups[1].Value);
            }
            return null;
        }

        private static Uri GetUri(string line)
        {
            line = line.Trim();
            if (line.Contains("%"))
            {
                var endpoints = App.Current.JsonSettings?.EndPoints;
                if (endpoints != null)
                {
                    line = endpoints
                        .Where(z => !string.IsNullOrEmpty(z.Name) && z.Value != null)
                        .Aggregate(line, (current, endpoint) =>
                            current.Replace("%" + endpoint.Name + "%", endpoint.Value, StringComparison.OrdinalIgnoreCase));
                }
            }
            return CreateOrNull.CreateUri(line, UriKind.Absolute);
        }
    }
}
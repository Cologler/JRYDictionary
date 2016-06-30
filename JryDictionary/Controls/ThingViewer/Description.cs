using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using JryDictionary.Common;

// ReSharper disable InconsistentNaming

namespace JryDictionary.Controls.ThingViewer
{
    public class Description
    {
        private static readonly Regex WebUriRegex = new Regex("^(https?://[^ ]+)", RegexOptions.IgnoreCase);

        private readonly string text;
        private readonly string[] lines;
        private readonly int metaSpliterIndex;
        private readonly int contentStartIndex;
        private readonly List<Inline> inlines = new List<Inline>();
        private FormatType format;

        public Description(string text)
        {
            this.text = text;
            this.lines = text.AsLines();
            this.metaSpliterIndex = this.lines.FindIndex(z => z == "-");
            this.contentStartIndex = this.metaSpliterIndex + 1;
            this.Parse();
        }

        private void Parse()
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
                                    this.BG(line);
                                }
                                break;

                            case 'C':
                                if (line.StartsWith("CV:"))
                                {
                                    this.CV(line);
                                }
                                break;

                            case 'F':
                                if (line.StartsWith("FM:"))
                                {
                                    this.FM(line);
                                }
                                break;
                        }
                    }
                }
            }

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

        private void BG(string line) => this.Background = this.GetUri(line.Substring(3)) ?? this.Background;

        public string Background { get; private set; }

        private void CV(string line) => this.Cover = this.GetUri(line.Substring(3)) ?? this.Cover;

        private string GetUri(string line)
        {
            line = line.Trim();
            var match = WebUriRegex.Match(line.Trim());
            if (match.Success)
            {
                return match.Value;
            }
            if (line.ToLower().StartsWith("file:///"))
            {
                var ret = line.Substring(8);
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

        public string Cover { get; private set; }

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
            for (var i = this.contentStartIndex; i < this.lines.Length; i++)
            {
                var line = this.lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    this.inlines.Add(new LineBreak());
                }
                else
                {
                    this.inlines.Add(new Run(line + " "));
                }
            }
        }

        public Inline[] Inlines => this.inlines.ToArray();
    }
}
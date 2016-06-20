using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Documents;

namespace JryDictionary
{
    public class DocumentViewModel
    {
        private readonly string document;
        public event EventHandler<string> OnHyperlinkClicked;

        public DocumentViewModel(string document)
        {
            this.document = document;
        }

        public IEnumerable<Inline> BuildInlines()
        {
            if (this.document.Length == 0) yield break;

            var ptr = 0;
            var index = 0;
            while ((index = this.document.IndexOf("[[", index, StringComparison.Ordinal)) >= 0)
            {
                if (index + 4 >= this.document.Length) // ..[[]]$
                {
                    break;
                }

                if (index > 0 && this.document[index - 1] == '\\')
                {
                    index++;
                    continue;
                }

                var endIndex = index + 3; // [[]] must contain > 0 char
                while ((endIndex = this.document.IndexOf("]]", endIndex, StringComparison.Ordinal)) >= 0)
                {
                    if (this.document[endIndex - 1] == '\\')
                    {
                        endIndex++;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                if (endIndex < 0) break;

                if (index > ptr)
                {
                    yield return new Run(this.document.Substring(ptr, index - ptr));
                    ptr = index;
                }

                var name = this.document.Substring(ptr + 2, endIndex - ptr - 2);
                var hl = new Hyperlink(new Run(name))
                {
                    NavigateUri = new Uri($@"internal:\\?name={name.UrlEncode()}")
                };
                ptr = endIndex + 2;
                hl.RequestNavigate += this.Hl_RequestNavigate;
                yield return hl;

                index = endIndex + 2;
                if (index > this.document.Length - 2) break;
            }

            if (ptr < this.document.Length - 1)
            {
                yield return new Run(this.document.Substring(ptr));
            }
        }

        private void Hl_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Debug.Print(e.Uri.ToString());
            e.Handled = true;
        }
    }
}
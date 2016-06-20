using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Jasily.Collections.Generic;

namespace JryDictionary.Controls.ThingViewer
{
    public class ThingViewerViewModel
    {
        public ThingViewModel ThingViewModel { get; }

        public ThingViewerViewModel(ThingViewModel thingViewModel)
        {
            this.ThingViewModel = thingViewModel;
            this.GroupedFields = this.ThingViewModel.Fields.GroupBy(z => z.Source.Name).Select(z => z.ToList()).ToList();

            this.Document = "[[12]]..[[2\\]]...[[4325]].";
        }

        public List<GroupedList<string, FieldViewModel>> GroupedFields { get; }

        public static implicit operator ThingViewerViewModel(ThingViewModel thingViewModel)
            => new ThingViewerViewModel(thingViewModel);

        public string Document { get; }

        public IEnumerable<Inline> BuildInlines()
        {
            if (this.Document.Length == 0) yield break;

            var ptr = 0;
            var index = 0;
            while ((index = this.Document.IndexOf("[[", index, StringComparison.Ordinal)) >= 0)
            {
                if (index + 4 >= this.Document.Length) // ..[[]]$
                {
                    break;
                }

                if (index > 0 && this.Document[index - 1] == '\\')
                {
                    index++;
                    continue;
                }

                var endIndex = index + 3; // [[]] must contain > 0 char
                while ((endIndex = this.Document.IndexOf("]]", endIndex, StringComparison.Ordinal)) >= 0)
                {
                    if (this.Document[endIndex - 1] == '\\')
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
                    yield return new Run(this.Document.Substring(ptr, index - ptr));
                    ptr = index;
                }

                var name = this.Document.Substring(ptr + 2, endIndex - ptr - 2);
                var hl = new Hyperlink(new Run(name))
                {
                    NavigateUri = new Uri($@"internal:\\?name={name.UrlEncode()}")
                };
                ptr = endIndex + 2;
                yield return hl;

                index = endIndex + 2;
                if (index > this.Document.Length - 2) break;
            }

            if (ptr < this.Document.Length - 1)
            {
                yield return new Run(this.Document.Substring(ptr));
            }
        }
    }
}
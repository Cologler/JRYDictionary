using System;
using Jasily.ComponentModel;

namespace JryDictionary.Controls.ImagesViewer
{
    public class ImagesViewerItemViewModel : JasilyViewModel
    {
        public Uri Uri { get; }

        public ImagesViewerItemViewModel(Uri uri)
        {
            this.Uri = uri;
        }

        public string Name { get; set; }

        public bool IsNameNotEmpty => !string.IsNullOrWhiteSpace(this.Name);
    }
}
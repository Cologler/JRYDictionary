using System;
using System.Net.Cache;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;

namespace JryDictionary.Models.Parsers
{
    public class UriInfo
    {
        public UriInfo(Uri uri, string name = null)
        {
            this.Name = name ?? string.Empty;
            this.Uri = uri;
        }

        [NotNull]
        public string Name { get; }

        public Uri Uri { get; }

        public ImageSource CreateImageSource() => BitmapFromUri(this.Uri);

        public static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
            bitmap.EndInit();
            return bitmap;
        }
    }
}
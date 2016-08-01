using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JryDictionary.Models.Parsers;

namespace JryDictionary.Controls.ImagesViewer
{
    public sealed class ImagesCreator
    {
        [DllImport("WinInet.dll", PreserveSig = true, SetLastError = true)]
        public static extern void DeleteUrlCacheEntry(string url);

        private readonly List<UriInfo> items = new List<UriInfo>();

        public Image AddImage(UriInfo info)
        {
            var uri = info.Uri;
            if (uri.IsFile && !File.Exists(uri.LocalPath)) return null;
            this.items.Add(info);
            var image = new Image
            {
                Source = info.CreateImageSource(),
                Margin = new Thickness(2),
                Tag = info
            };
            var menu1 = new MenuItem
            {
                Tag = uri,
                Header = "open in brower"
            };
            menu1.Click += this.Menu1_Click;
            var menu2 = new MenuItem
            {
                Tag = uri,
                Header = "reset cache"
            };
            menu2.Click += (s, e) => ClearCache(uri.ToString());
            image.ContextMenu = new ContextMenu
            {
                Items =
                {
                    menu1, menu2
                }
            };
            image.ToolTip = string.IsNullOrWhiteSpace(info.Name)
                ? uri.ToString()
                : $"{info.Name}\r\n{uri}";
            image.MouseLeftButtonDown += this.Image_MouseLeftButtonDown;
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
            return image;
        }

        private void Menu1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (Process.Start(((Uri)((FrameworkElement)sender).Tag).ToString())) { }
            }
            catch
            {
                // ignored
            }
        }

        public static void ClearCache(string url)
        {
            if (url == null) return;
            try
            {
                DeleteUrlCacheEntry(url);
            }
            catch
            {
                // ignored
            }
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
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace JryDictionary.Controls.ImagesViewer
{
    /// <summary>
    /// ImagesViewerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImagesViewerWindow
    {
        public ImagesViewerWindow()
        {
            this.InitializeComponent();
        }

        public int SelectedIndex
        {
            get { return this.ImagesViewerControl.SelectedIndex; }
            set { this.ImagesViewerControl.SelectedIndex = value; }
        }

        public static void Show(IEnumerable<ImagesViewerItemViewModel> items, ImagesViewerItemViewModel selected = null)
        {
            var owner = App.Current.Windows.OfType<Window>().FirstOrDefault(z => z.IsActive);
            var win = new ImagesViewerWindow
            {
                DataContext = items,
                Owner = owner
            };
            win.Show();
            win.ImagesViewerControl.FlipView.SelectedItem = selected;
        }
    }
}

using System.Windows.Controls;

namespace JryDictionary.Controls.ImagesViewer
{
    /// <summary>
    /// ImagesViewerControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImagesViewerControl : UserControl
    {
        public ImagesViewerControl()
        {
            this.InitializeComponent();
        }

        public int SelectedIndex
        {
            get { return this.FlipView.SelectedIndex; }
            set { this.FlipView.SelectedIndex = value; }
        }
    }
}

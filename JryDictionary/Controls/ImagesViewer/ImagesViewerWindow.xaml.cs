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
    }
}

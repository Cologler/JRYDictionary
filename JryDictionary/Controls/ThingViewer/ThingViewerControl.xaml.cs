using System.Windows.Controls;

namespace JryDictionary.Controls.ThingViewer
{
    /// <summary>
    /// ThingViewerControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThingViewerControl : UserControl
    {
        private ThingViewerViewModel viewModel;

        public ThingViewerControl()
        {
            this.InitializeComponent();
        }

        public ThingViewerViewModel ViewModel
        {
            get { return this.viewModel; }
            set { this.DataContext = this.viewModel = value; }
        }
    }
}

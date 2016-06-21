using System.Windows;
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
            set
            {
                if (this.viewModel == value) return;
                this.DataContext = this.viewModel = value;
            }
        }

        private async void ViewFieldMenuItem_OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var field = (FieldViewModel)((FrameworkElement)sender).DataContext;
            var thing = await App.Current.ThingSetAccessor.FindOneAsync(field.TargetId);
            this.ViewModel = new ThingViewerViewModel(new ThingViewModel(thing));
        }
    }
}

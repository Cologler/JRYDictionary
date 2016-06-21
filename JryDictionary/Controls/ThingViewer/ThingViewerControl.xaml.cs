using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JryDictionary.Modules.Copyer;

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

        private void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)((FrameworkElement) sender).DataContext;
            var copyer = (IWordCopyer)((FrameworkElement)e.OriginalSource).DataContext;
            copyer.Copy(word.Thing.Source, word.Source);
        }

        private async void FieldUIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var field = (FieldViewModel)((FrameworkElement)sender).DataContext;
            var thing = await App.Current.ThingSetAccessor.FindOneAsync(field.TargetId);
            this.ViewModel = new ThingViewerViewModel(new ThingViewModel(thing));
        }
    }
}

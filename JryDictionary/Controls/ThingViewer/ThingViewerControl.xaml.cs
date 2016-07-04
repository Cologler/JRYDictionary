using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JryDictionary.Models;
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

        private void ViewFieldMenuItem_OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var field = (FieldViewModel)((FrameworkElement)sender).DataContext;
            this.ViewThing(field.TargetId);
        }

        private void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)((FrameworkElement)sender).DataContext;
            var copyer = (IWordCopyer)((FrameworkElement)e.OriginalSource).DataContext;
            copyer.Copy(word.Thing.Source, word.Source);
        }

        private void FieldUIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var field = ((FrameworkElement)sender).DataContext as FieldViewModel;
            if (field == null) return;
            this.ViewThing(field.TargetId);
        }

        public async void ViewThing(string thingId)
        {
            var thing = await App.Current.ThingSetAccessor.FindOneAsync(thingId);
            if (thing == null) return;
            this.ViewModel = new ThingViewerViewModel(new ThingViewModel(thing));
        }

        private void Image_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DescriptionParser.Image_MouseLeftButtonDown(sender, e);
        }
    }
}

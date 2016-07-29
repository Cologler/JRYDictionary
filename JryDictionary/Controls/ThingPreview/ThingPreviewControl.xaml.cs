using System.Windows;

namespace JryDictionary.Controls.ThingPreview
{
    /// <summary>
    /// ThingPreviewControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThingPreviewControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(object), typeof(ThingPreviewControl),
            new PropertyMetadata(OnViewModelChangedCallback));

        private static void OnViewModelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ThingPreviewControl) d).ViewModel = e.NewValue as IThingPreviewViewModel;
        }

        public ThingPreviewControl()
        {
            this.InitializeComponent();
        }

        public IThingPreviewViewModel ViewModel
        {
            get { return this.RootGrid.DataContext as IThingPreviewViewModel; }
            set { this.RootGrid.DataContext = value; }
        }
    }
}

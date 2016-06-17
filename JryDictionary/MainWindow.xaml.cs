using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JryDictionary.Builders;
using JryDictionary.Controls.ThingEditor;

namespace JryDictionary
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
            : this(Singleton.Instance<ViewerMainViewModel>())
        {
        }

        public MainViewModel ViewModel { get; }

        public MainWindow(MainViewModel viewModel)
        {
            this.DataContext = this.ViewModel = viewModel;
            this.InitializeComponent();
        }

        #region Overrides of Window

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override async void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var vm = this.ViewModel;
            await vm.InitializeAsync();
            await vm.LoadAsync();
        }

        #endregion

        private async void CommitButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (await this.ViewModel.CommitFooterInputAsnyc())
            {
                this.DialogResult = true;
            }
        }

        private void WordsDataGrid_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var word = ((FrameworkElement)e.OriginalSource).DataContext as WordViewModel;
            if (word == null)
            {
                e.Handled = true;
            }
            else
            {
                this.WordsDataGridContextMenu.DataContext = word;
            }
        }

        private async void SearchModeSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await this.ViewModel.LoadAsync();
        }

        private async void EditMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            var editor = new ThingEditorViewModel(word.Thing);
            this.ViewModel.Editing = editor;
            this.EditorFlyout.IsOpen = true;
            await editor.InitializeAsync();
        }

        private async void EditorCommitButton_OnClick(object sender, RoutedEventArgs e)
        {
            await this.ViewModel.Editing.CommitAsync();
            this.ViewModel.Editing = null;
            this.EditorFlyout.IsOpen = false;
            await this.ViewModel.LoadAsync();
        }

        private void EditorCancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Editing = null;
            this.EditorFlyout.IsOpen = false;
        }

        private void RemoveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            if (word.Thing.MajorWord != word)
            {
                this.ViewModel.Remove(word);
            }
            else
            {

            }
        }

        private async void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            var time = 0;
            while (time < 5)
            {
                try
                {
                    Clipboard.SetText(word.Word);
                    return;
                }
                catch
                {
                    // ignored
                }
                time++;
                await Task.Delay(50);
            }
            Debug.WriteLine("copy failed.");
        }

        private void BuildMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            var builder = (IWordBuilder)((FrameworkElement)e.OriginalSource).DataContext;
            this.ViewModel.Build(word, builder);
        }

        private void CreateFieldMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            BeginCreateField(this, word.Thing.Source.Id);
        }

        private async void ViewFieldMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            var field = (FieldViewModel)((FrameworkElement)e.OriginalSource).DataContext;
            var thing = await App.Current.ThingSetAccessor.FindOneAsync(field.Source.TargetId);
            if (thing == null) return;
            this.ViewThing(new ThingViewModel(thing));
        }

        private void RemoveFieldMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            var field = (FieldViewModel)((FrameworkElement)e.OriginalSource).DataContext;

            this.ViewModel.RemoveField(word.Thing, field);
        }

        private void ViewThing(ThingViewModel viewModel)
        {
            Debug.Assert(viewModel != null);
            (this.ViewModel as ViewerMainViewModel)?.SetViewer(viewModel);
            this.ThingViewerControl.ViewModel = viewModel;
            this.ViewerFlyout.IsOpen = true;
        }

        public static void BeginCreateField(MainWindow owner, string thingId)
        {
            var selector = new MainWindow(new SelectorMainViewModel(thingId))
            {
                Owner = owner
            };
            selector.ShowDialog();
        }

        private void ViewMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            this.ViewThing(word.Thing);
        }

        private void ViewerCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            (this.ViewModel as ViewerMainViewModel)?.SetViewer(null);
            this.ViewerFlyout.IsOpen = false;
        }
    }
}

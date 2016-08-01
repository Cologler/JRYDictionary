using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using JryDictionary.Controls.ThingEditor;
using JryDictionary.Controls.ThingViewer;
using JryDictionary.Modules.Builders;
using JryDictionary.Modules.Copyer;

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
            if (viewModel.ViewModelType == MainViewModelType.Selector)
            {
                this.SetReadOnly();
            }
        }

        private void SetReadOnly()
        {
            this.WordsColumn.IsReadOnly = true;
            this.LanguagesColumn.IsReadOnly = true;
            this.EditMenuItem.IsEnabled = false;
            this.CreateFieldMenuItem.IsEnabled = false;
            this.RemoveFieldMenuItem.IsEnabled = false;
            this.BuildMenuItem.IsEnabled = false;
            this.RemoveMenuItem.IsEnabled = false;
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
            var element = (e.OriginalSource as Run)?.Parent as FrameworkElement ?? e.OriginalSource as FrameworkElement;
            var word = element?.DataContext as WordViewModel;
            if (word != null)
            {
                this.WordsDataGridContextMenu.DataContext = word;
            }
            else
            {
                e.Handled = true;
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

        private void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            var copyer = (IWordCopyer)((FrameworkElement)e.OriginalSource).DataContext;
            copyer.Copy(word.Thing.Source, word.Source);
        }

        private void BuildMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            var builder = (IWordBuilder)((FrameworkElement)e.OriginalSource).DataContext;
            this.ViewModel.Build(word, builder);
        }

        private async void CreateFieldMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            if (BeginCreateField(this, word.Thing.Source.Id) == true)
            {
                await this.ViewModel.LoadAsync();
            }
        }

        private async void ViewFieldMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
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
            this.ThingViewerControl.ViewThing(viewModel.Source.Id);
            this.ViewerFlyout.IsOpen = true;
        }

        public static bool? BeginCreateField(MainWindow owner, string thingId)
        {
            var selector = new MainWindow(new SelectorMainViewModel(thingId))
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner
            };
            return selector.ShowDialog();
        }

        private void ViewMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            this.ViewThing(word.Thing);
        }

        private void ViewerCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.ThingViewerControl.ViewModel = null;
            this.ViewerFlyout.IsOpen = false;
        }

        private void ThingViewerControl_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (this.ViewModel as ViewerMainViewModel)?.SetViewer(
                (this.ThingViewerControl.DataContext as ThingViewerViewModel)?.ThingViewModel);
        }
    }
}

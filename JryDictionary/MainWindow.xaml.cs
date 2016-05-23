using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JryDictionary.Models;

namespace JryDictionary
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = Singleton.Instance<MainViewModel>();
        }

        #region Overrides of Window

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override async void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            await Singleton.Instance<MainViewModel>().LoadAsync();
        }

        #endregion

        private async void CommitButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Singleton.Instance<MainViewModel>().CommitAddThingAsnyc();
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
            await Singleton.Instance<MainViewModel>().LoadAsync();
        }

        private void EditMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel) this.WordsDataGridContextMenu.DataContext;
            Singleton.Instance<MainViewModel>().Editing = new ThingEditorViewModel(word.Thing);
            this.EditorFlyout.IsOpen = true;
        }

        private async void EditorCommitButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Singleton.Instance<MainViewModel>().Editing.CommitAsync();
            Singleton.Instance<MainViewModel>().Editing = null;
            this.EditorFlyout.IsOpen = false;
            await Singleton.Instance<MainViewModel>().LoadAsync();
        }

        private void EditorCancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Singleton.Instance<MainViewModel>().Editing = null;
            this.EditorFlyout.IsOpen = false;
        }

        private void EditorRemoveWordButton_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordEditorViewModel)((FrameworkElement)sender).DataContext;
            Debug.Assert(word != null);
            if (Singleton.Instance<MainViewModel>().Editing.MajorWord == word)
            {

            }
            else
            {
                Singleton.Instance<MainViewModel>().Editing.Remove(word);
            }
        }

        private void EditorToNameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordEditorViewModel)((FrameworkElement)sender).DataContext;
            Debug.Assert(word != null);
            if (Singleton.Instance<MainViewModel>().Editing.MajorWord != word)
            {
                Singleton.Instance<MainViewModel>().Editing.SetMajor(word);
            }
        }

        private void RemoveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            if (word.Thing.MajorWord != word)
            {
                Singleton.Instance<MainViewModel>().Remove(word);
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

        private void BuildPinYinMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var word = (WordViewModel)this.WordsDataGridContextMenu.DataContext;
            Singleton.Instance<MainViewModel>().BuildPinYin(word);
        }
    }
}

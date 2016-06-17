using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using JryDictionary.Models;

namespace JryDictionary.Controls.ThingEditor
{
    /// <summary>
    /// ThingEditorControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThingEditorControl : UserControl
    {
        public ThingEditorControl()
        {
            this.InitializeComponent();
        }

        public ThingEditorViewModel CurrentViewModel => (ThingEditorViewModel)this.DataContext;

        private void ToMajorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = this.CurrentViewModel;
            var word = (WordEditorViewModel)((FrameworkElement)sender).DataContext;
            Debug.Assert(word != null);
            if (vm.MajorWord != word)
            {
                vm.SetMajor(word);
            }
        }

        private void RemoveWordButton_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = this.CurrentViewModel;
            var word = (WordEditorViewModel)((FrameworkElement)sender).DataContext;
            Debug.Assert(word != null);
            if (vm.MajorWord == word)
            {

            }
            else
            {
                vm.Remove(word);
            }
        }

        private void RemoveCategoryMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = this.CurrentViewModel;
            vm.Categorys.Remove(((FrameworkElement)sender).DataContext as string);
        }

        private void AddCategoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = this.CurrentViewModel;
            vm.AddCategory(vm.CategoryInput);
        }

        private void RemoveFieldMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = this.CurrentViewModel;
            vm.Fields.Remove(((FrameworkElement)sender).DataContext as Field);
        }
    }
}

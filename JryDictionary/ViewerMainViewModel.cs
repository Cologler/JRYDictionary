using System;
using System.Threading.Tasks;
using JryDictionary.Models;

namespace JryDictionary
{
    public class ViewerMainViewModel : MainViewModel
    {
        private ThingViewModel viewerViewModel;

        public ViewerMainViewModel()
        {
            this.FooterHeader = "create thing".ToUpper();
        }

        public override MainViewModelType ViewModelType => MainViewModelType.Viewer;

        public override string WindowTitle =>
            this.viewerViewModel != null
                ? $"jry dictionary ¡¸{this.viewerViewModel.MajorWord.Word}¡¹"
                : this.Searched ? $"jry dictionary ({this.Things.Count}{(this.HasNext ? "+" : "")})" : "jry dictionary";

        public override async Task<bool> CommitFooterInputAsnyc()
        {
            var value = this.FooterContent;
            this.FooterContent = string.Empty;
            if (string.IsNullOrWhiteSpace(value)) return false;
            value = value.Trim();
            var thing = new Thing();
            thing.Id = Guid.NewGuid().ToString().ToUpper();
            thing.Words.Add(new Word
            {
                Text = value
            });
            await App.Current.ThingSetAccessor.UpdateAsync(thing);
            await this.LoadAsync();
            return false;
        }

        public void SetViewer(ThingViewModel thing)
        {
            this.viewerViewModel = thing;
            this.NotifyPropertyChanged(nameof(this.WindowTitle));
        }
    }
}
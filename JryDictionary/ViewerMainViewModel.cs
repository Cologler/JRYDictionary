using System;
using System.Threading.Tasks;
using JryDictionary.Models;

namespace JryDictionary
{
    public class ViewerMainViewModel : MainViewModel
    {
        public ViewerMainViewModel()
        {
            this.FooterHeader = "create thing".ToUpper();
        }

        public override MainViewModelType ViewModelType => MainViewModelType.Viewer;

        public override string WindowTitle => this.Searched ? $"jry dictionary ({this.Things.Count}{(this.HasNext ? "+" : "")})" : "jry dictionary";

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
    }
}
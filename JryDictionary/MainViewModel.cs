using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jasily;
using Jasily.ComponentModel;
using Jasily.Windows.Data;
using JryDictionary.Controls.ThingEditor;
using JryDictionary.Models;
using JryDictionary.Modules.Builders;
using JryDictionary.Modules.Copyer;
using MongoDB.Driver;

namespace JryDictionary
{
    public abstract class MainViewModel : JasilyViewModel
    {
        private string searchText;
        private ThingEditorViewModel editing;
        private string footerHeader;
        private string footerContent;

        protected MainViewModel()
        {
            this.SearchModes.Collection.AddRange(EnumCache<SearchMode>.Default
                .All()
                .Select(z => new ValueReference<NameValuePair<SearchMode>>(new NameValuePair<SearchMode>(EnumCache<SearchMode>.Default.ToString(z), z))));
            this.SearchModes.Selected = this.SearchModes.Collection[0];
        }

        public abstract MainViewModelType ViewModelType { get; }

        protected bool Searched { get; private set; }

        protected bool HasNext { get; private set; }

        public string SearchText
        {
            get { return this.searchText; }
            set
            {
                if (this.SetPropertyRef(ref this.searchText, value))
                    this.BeginDelaySearch();
            }
        }

        private async void BeginDelaySearch()
        {
            var text = this.SearchText ?? string.Empty;
            await Task.Delay(400);
            if (text == this.searchText)
            {
                await this.LoadAsync();
            }
        }

        public virtual async Task InitializeAsync()
        {
            this.Builders.AddRange(App.Current.ModuleManager.Builders);
            this.Copyers.AddRange(App.Current.ModuleManager.Copyers);

            var categorys = (await App.Current.ThingSetAccessor.GroupCategorysAsync()).Insert(0, string.Empty).ToArray();
            this.ExistsCategorys.Collection.Reset(categorys);
            if (this.ExistsCategorys.Selected == null) this.ExistsCategorys.Selected = string.Empty;
            App.Current.ThingSetAccessor.SavedNewCategory += this.ThingSetAccessor_SavedNewCategory;

            this.ExistsLanguages.AddRange(await App.Current.ThingSetAccessor.GroupLanguagesAsync());
        }

        private void ThingSetAccessor_SavedNewCategory(object sender, string e) => this.ExistsCategorys.Collection.Add(e);

        public async Task LoadAsync()
        {
            var value = this.searchText;
            if (string.IsNullOrWhiteSpace(value))
            {
                this.Searched = false;
                this.Things.Clear();
                this.Words.Collection.Clear();
            }
            else
            {
                this.Searched = true;
                var builder = new FilterDefinitionBuilder<Thing>();
                FilterDefinition<Thing> filter;
                switch (this.SearchModes.Selected.Value.Value)
                {
                    case SearchMode.Normal:
                        filter = builder.Regex(
                            PropertySelector<Thing>.Start().SelectMany(z => z.Words).Select(z => z.Text).ToString(),
                            new Regex(Regex.Escape(value), RegexOptions.IgnoreCase));
                        break;

                    case SearchMode.WholeWord:
                        filter = builder.Eq(PropertySelector<Thing>.Start().SelectMany(z => z.Words).Select(z => z.Text).ToString(), value);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                string category = null;
                if (this.ExistsCategorys.Selected != string.Empty)
                {
                    category = this.ExistsCategorys.Selected;
                    filter = filter & builder.AnyEq(z => z.Categorys, this.ExistsCategorys.Selected);
                }
                filter = builder.Eq(z => z.Id, value) | filter;
                var queryResult = await App.Current.ThingSetAccessor.FindAsync(filter, 20);
                this.HasNext = queryResult.HasNext;
                var sortedItems = await queryResult.Items
                    .OrderBy(z => z.Words.Any(x => x.Text == value) ? -1 : 1)
                    .ThenBy(z => z.Words.Any(x => x.Text.Equals(value, StringComparison.OrdinalIgnoreCase)) ? -1 : 1)
                    .ToArrayAsync();
                this.Things.Reset(sortedItems.Select(z => new ThingViewModel(z, category)));
                this.Words.Collection.Reset(this.Things.SelectMany(z => z.Words));
            }
            this.RefreshProperties();
        }

        public ObservableCollection<ThingViewModel> Things { get; } = new ObservableCollection<ThingViewModel>();

        public JasilyCollectionView<WordViewModel> Words { get; } = new JasilyCollectionView<WordViewModel>();

        public JasilyCollectionView<ValueReference<NameValuePair<SearchMode>>> SearchModes { get; }
            = new JasilyCollectionView<ValueReference<NameValuePair<SearchMode>>>();

        public JasilyCollectionView<string> ExistsCategorys { get; } = new JasilyCollectionView<string>();

        // ReSharper disable once CollectionNeverQueried.Global
        public ObservableCollection<IWordBuilder> Builders { get; } = new ObservableCollection<IWordBuilder>();

        public ObservableCollection<IWordCopyer> Copyers { get; } = new ObservableCollection<IWordCopyer>();

        public async Task CommitAddThingAsnyc()
        {
            var value = this.FooterContent;
            this.FooterContent = string.Empty;
            if (string.IsNullOrWhiteSpace(value)) return;
            value = value.Trim();
            var thing = new Thing();
            thing.Id = Guid.NewGuid().ToString().ToUpper();
            thing.Words.Add(new Word
            {
                Text = value
            });
            await App.Current.ThingSetAccessor.UpdateAsync(thing);
            await this.LoadAsync();
        }

        public ThingEditorViewModel Editing
        {
            get { return this.editing; }
            set { this.SetPropertyRef(ref this.editing, value); }
        }

        public void Remove(WordViewModel word)
        {
            Debug.Assert(word.Thing.MajorWord != word);
            this.Words.Collection.Remove(word);
            word.Thing.Words.Remove(word);
            word.Thing.Source.Words.Remove(word.Source);
            word.Thing.Update();
        }

        public void Build(WordViewModel word, IWordBuilder builder)
        {
            Debug.Assert(word != null);
            Debug.Assert(builder != null);

            foreach (var retWord in builder.Build(word.Thing, word))
            {
                word.Thing.Source.Words.Add(retWord);
                var pinyinModel = new WordViewModel(word.Thing, retWord);
                word.Thing.Words.Add(pinyinModel);
                this.Words.Collection.Insert(this.Words.Collection.IndexOf(word) + 1, pinyinModel);
            }
            word.Thing.Update();
        }

        public void RemoveField(ThingViewModel thing, FieldViewModel field)
        {
            Debug.Assert(thing.Fields.Contains(field));

            if (!thing.Fields.Remove(field))
            {
                Debug.Assert(false);
            }

            if (thing.Source.Fields != null)
            {
                thing.Source.Fields.Remove(field.Source);
                if (thing.Source.Fields.Count == 0) thing.Source.Fields = null;
            }

            thing.Update();
        }

        public ObservableCollection<string> ExistsLanguages { get; } = new ObservableCollection<string>();

        #region header

        [NotifyPropertyChanged]
        public abstract string WindowTitle { get; }

        #endregion

        #region footer

        public string FooterHeader
        {
            get { return this.footerHeader; }
            protected set { this.SetPropertyRef(ref this.footerHeader, value); }
        }

        public string FooterContent
        {
            get { return this.footerContent; }
            set { this.SetPropertyRef(ref this.footerContent, value); }
        }

        /// <summary>
        /// return true if need close current window.
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> CommitFooterInputAsnyc();

        #endregion
    }
}
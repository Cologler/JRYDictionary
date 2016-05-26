using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jasily;
using Jasily.Chinese.PinYin;
using Jasily.ComponentModel;
using Jasily.Windows.Data;
using JryDictionary.Builders;
using JryDictionary.Controls.ThingEditor;
using JryDictionary.Models;
using MongoDB.Driver;

namespace JryDictionary
{
    public class MainViewModel : JasilyViewModel
    {
        private string searchText;
        private string newThing;
        private string newWord;
        private ThingEditorViewModel editing;
        private PinYinManager pinYinManager;

        public MainViewModel()
        {
            this.SearchModes.Collection.AddRange(EnumCache<SearchMode>.Default.All()
                .Select(z => new Boxing<NameValuePair<SearchMode>>(
                    new NameValuePair<SearchMode>(EnumCache<SearchMode>.Default.ToString(z), z))));
            this.SearchModes.Selected = this.SearchModes.Collection[0];
        }

        public string SearchText
        {
            get { return this.searchText; }
            set
            {
                if (this.SetPropertyRef(ref this.searchText, value))
                    this.BeginDelaySearch();
            }
        }

        public async void BeginDelaySearch()
        {
            var text = this.SearchText ?? string.Empty;
            await Task.Delay(400);
            if (text == this.searchText)
            {
                await this.LoadAsync();
            }
        }

        private IMongoCollection<Thing> GetThingsSet() => App.Current.ThingSetAccessor.Collection;

        public async Task InitializeAsync()
        {
            this.Builders.Add(new AbbreviationWordBuilder());
            this.Builders.Add(new PinYinWordBuilder());

            var categorys = (await App.Current.ThingSetAccessor.GroupCategorysAsync()).Insert(0, string.Empty).ToArray();
            this.SearchCategorys.Collection.Reset(categorys);
            if (this.SearchCategorys.Selected == null) this.SearchCategorys.Selected = string.Empty;
            App.Current.ThingSetAccessor.SavedNewCategory += this.ThingSetAccessor_SavedNewCategory;
        }

        private void ThingSetAccessor_SavedNewCategory(object sender, string e) => this.SearchCategorys.Collection.Add(e);

        public async Task LoadAsync()
        {
            var value = this.searchText;
            if (string.IsNullOrWhiteSpace(value))
            {
                this.Things.Clear();
                this.Words.Clear();
            }
            else
            {
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
                if (this.SearchCategorys.Selected != string.Empty)
                {
                    category = this.SearchCategorys.Selected;
                    filter = builder.And(filter, builder.AnyEq(z => z.Categorys, this.SearchCategorys.Selected));
                }
                var col = this.GetThingsSet();
                var items = await (await col.FindAsync(filter, new FindOptions<Thing, Thing>
                {
                    Limit = 21
                })).ToListAsync();
                if (items.Count == 101)
                {
                }
                this.Things.Reset(items.Take(20)
                    .Select(z => new ThingViewModel(z, category ?? string.Join(", ", z.Categorys ?? Empty<string>.Enumerable))));
                this.Words.Reset(this.Things.SelectMany(z => z.Words));
            }
        }

        public ObservableCollection<ThingViewModel> Things { get; } = new ObservableCollection<ThingViewModel>();

        public ObservableCollection<WordViewModel> Words { get; } = new ObservableCollection<WordViewModel>();

        public JasilyCollectionView<Boxing<NameValuePair<SearchMode>>> SearchModes { get; }
            = new JasilyCollectionView<Boxing<NameValuePair<SearchMode>>>();

        public JasilyCollectionView<string> SearchCategorys { get; } = new JasilyCollectionView<string>();

        public ObservableCollection<IWordBuilder> Builders { get; } = new ObservableCollection<IWordBuilder>();

        public string NewThing
        {
            get { return this.newThing; }
            set { this.SetPropertyRef(ref this.newThing, value); }
        }

        public string NewWord
        {
            get { return this.newWord; }
            set { this.SetPropertyRef(ref this.newWord, value); }
        }

        public async Task CommitAddThingAsnyc()
        {
            var value = this.NewThing;
            this.NewThing = string.Empty;
            if (string.IsNullOrWhiteSpace(value)) return;
            value = value.Trim();
            var thing = new Thing();
            thing.Id = Guid.NewGuid().ToString().ToUpper();
            thing.Words.Add(new Word
            {
                Text = value
            });
            var col = this.GetThingsSet();
            await col.InsertOneAsync(thing);
        }

        public ThingEditorViewModel Editing
        {
            get { return this.editing; }
            set { this.SetPropertyRef(ref this.editing, value); }
        }

        public void Remove(WordViewModel word)
        {
            Debug.Assert(word.Thing.MajorWord != word);
            this.Words.Remove(word);
            word.Thing.Words.Remove(word);
            word.Thing.Source.Words.Remove(word.Source);
            word.Thing.Update();
        }

        public void Build(WordViewModel word, IWordBuilder builder)
        {
            Debug.Assert(word != null);
            Debug.Assert(builder != null);

            var retWord = builder.Build(word.Thing, word);
            if (retWord == null) return;
            word.Thing.Source.Words.Add(retWord);
            word.Thing.Update();
            var pinyinModel = new WordViewModel(word.Thing, retWord);
            word.Thing.Words.Add(pinyinModel);
            this.Words.Insert(this.Words.IndexOf(word) + 1, pinyinModel);
        }
    }
}
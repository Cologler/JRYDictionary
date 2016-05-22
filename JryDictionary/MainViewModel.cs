using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Jasily;
using Jasily.ComponentModel;
using Jasily.Windows.Data;
using JryDictionary.Models;
using MongoDB.Driver;

namespace JryDictionary
{
    public class MainViewModel : JasilyViewModel
    {
        private string searchText = "ä“š£¤Î¥ì¥®¥ª¥¹";
        private string newThing;
        private string newWord;
        private ThingEditorViewModel editing;

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

        private IMongoCollection<Thing> GetThingsSet() => ((App)Application.Current).ThingCollection;

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
                        filter = builder.Regex(PropertySelector<Thing>.Start().SelectMany(z => z.Words).Select(z => z.Text).ToString(), Regex.Escape(value));
                        break;

                    case SearchMode.WholeWord:
                        filter = builder.Eq(PropertySelector<Thing>.Start().SelectMany(z => z.Words).Select(z => z.Text).ToString(), value);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var col = this.GetThingsSet();
                var items = await (await col.FindAsync(filter, new FindOptions<Thing, Thing>
                {
                    Limit = 21
                })).ToListAsync();
                if (items.Count == 101)
                {
                }
                this.Things.Reset(items.Take(20).Select(z => new ThingViewModel(z)));
                this.Words.Reset(this.Things.SelectMany(z => z.Words));
            }
        }

        public ObservableCollection<ThingViewModel> Things { get; } = new ObservableCollection<ThingViewModel>();

        public ObservableCollection<WordViewModel> Words { get; } = new ObservableCollection<WordViewModel>();

        public JasilyCollectionView<Boxing<NameValuePair<SearchMode>>> SearchModes { get; } = new JasilyCollectionView<Boxing<NameValuePair<SearchMode>>>();

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

        public async Task CommitAddAsnyc()
        {
            return;
            var thing = new Thing();
            thing.Id = Guid.NewGuid().ToString().ToUpper();
            thing.Words.Add(new Word
            {
                Text = "ä“š£¤Î¥ì¥®¥ª¥¹"
            });
            thing.Words.Add(new Word
            {
                Text = "¸Ö¿Ç¶¼ÊÐÀ×¼ªÅ·Ë¹"
            });
            var col = this.GetThingsSet();
            await col.InsertOneAsync(thing);
        }

        public ThingEditorViewModel Editing
        {
            get { return this.editing; }
            set { this.SetPropertyRef(ref this.editing, value); }
        }
    }
}
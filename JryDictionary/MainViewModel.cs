using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Jasily;
using Jasily.ComponentModel;
using Jasily.Windows.Data;
using JryDictionary.Models;
using MongoDB.Driver;

namespace JryDictionary
{
    public class MainViewModel : JasilyViewModel
    {
        private string searchText;
        private string newThing;
        private string newWord;

        public MainViewModel()
        {
            this.SearchModes.Collection.AddRange(EnumCache<SearchMode>.Default.All()
                .Select(z => new Boxing<NameValuePair<SearchMode>>(
                    new NameValuePair<SearchMode>(EnumCache<SearchMode>.Default.ToString(z), z))));
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
                //this.Items.Reset(((App)Application.Current).EfuFileManager
                //    .Search(text.Trim())
                //    .Select(z => new FileSystemRecordViewModel(z)));
            }
        }

        private IMongoCollection<Thing> GetThingsSet() => ((App)Application.Current).ThingCollection;

        public async Task LoadAsync()
        {
            var col = this.GetThingsSet();
            var items = await (await col.FindAsync(FilterDefinition<Thing>.Empty,
                options: new FindOptions<Thing, Thing>
                {
                    Limit = 21
                })).ToListAsync();
            if (items.Count == 101)
            {

            }
            this.Things.Reset(items.Take(20).Select(z => new ThingViewModel(z)));
            this.Words.Reset(this.Things.SelectMany(z => z.Words));
        }

        public ObservableCollection<ThingViewModel> Things { get; } = new ObservableCollection<ThingViewModel>();

        public ObservableCollection<WordViewModel> Words { get; } = new ObservableCollection<WordViewModel>();

        public JasilyCollectionView<Boxing<NameValuePair<SearchMode>>> SearchModes { get; }
            = new JasilyCollectionView<Boxing<NameValuePair<SearchMode>>>();

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
    }
}
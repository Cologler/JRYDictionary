using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Jasily.ComponentModel;
using JryDictionary.Models;
using MongoDB.Driver;

namespace JryDictionary
{
    public class ThingViewModel : JasilyViewModel<Thing>
    {
        public ThingViewModel(Thing source)
            : base(source)
        {
            this.Words = source.Words.Select(z => new WordViewModel(this, z)).ToList();
        }

        public List<WordViewModel> Words { get; }

        public WordViewModel MajorWord => this.Words[0];

        public async void Update()
        {
            await ((App)Application.Current).ThingCollection.ReplaceOneAsync(
                FilterDefinition<Thing>.Empty, this);
        }
    }
}
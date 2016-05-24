using System.Collections.Generic;
using System.Linq;
using Jasily.ComponentModel;
using JetBrains.Annotations;
using JryDictionary.Models;
using MongoDB.Driver;

namespace JryDictionary
{
    public class ThingViewModel : JasilyViewModel<Thing>
    {
        public ThingViewModel(Thing source, string category)
            : base(source)
        {
            this.Category = category;
            this.Words = source.Words.Select(z => new WordViewModel(this, z)).ToList();
        }

        [NotNull]
        public List<WordViewModel> Words { get; }

        public WordViewModel MajorWord => this.Words[0];

        public async void Update() => await App.Current.ThingSetAccessor.UpdateAsync(this.Source);

        public string Category { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Jasily.ComponentModel;
using JetBrains.Annotations;
using JryDictionary.Models;
using JryDictionary.Models.Parsers;
using MongoDB.Driver;

namespace JryDictionary
{
    public class ThingViewModel : JasilyViewModel<Thing>
    {
        public ThingViewModel(Thing source, string category = null)
            : base(source)
        {
            this.Category = category ?? string.Join(", ", source.Categorys ?? Empty<string>.Enumerable);
            this.Words = source.Words.Select(z => new WordViewModel(this, z)).ToList();
            if (source.Fields != null)
            {
                this.Fields.AddRange(source.Fields.Select(z => new FieldViewModel(z)));
            }
        }

        [NotNull]
        public List<WordViewModel> Words { get; }

        [NotNull]
        public WordViewModel MajorWord => this.Words[0];

        public async void Update() => await App.Current.ThingSetAccessor.UpdateAsync(this.Source);

        [NotNull]
        public string Category { get; }

        [NotNull]
        public ObservableCollection<FieldViewModel> Fields { get; } = new ObservableCollection<FieldViewModel>();

        [NotNull]
        public List<WordViewModel> Alias => this.Words.Skip(1).ToList();

        public Uri Icon => new ImageUriParser().TryParse(this.Source.Icon)?.Uri;
    }
}
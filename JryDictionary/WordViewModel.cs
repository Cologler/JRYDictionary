﻿using Jasily.ComponentModel;
using JryDictionary.Models;

namespace JryDictionary
{
    public class WordViewModel : JasilyViewModel<Word>
    {
        public WordViewModel(ThingViewModel thing, Word source)
            : base(source)
        {
            this.Thing = thing;
        }

        public ThingViewModel Thing { get; }

        public string Word
        {
            get { return this.Source.Text; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this.Source.Text = value.Trim();
                    this.Thing.Update();
                }
                this.NotifyPropertyChanged(nameof(this.Word));
            }
        }

        public string Language
        {
            get { return this.Source.Language; }
            set
            {
                this.Source.Language = value;
                this.Thing.Update();
                this.NotifyPropertyChanged(nameof(this.Language));
            }
        }

        public bool IsMajor => this.Thing.MajorWord == this;

        public bool CanRemove => !this.IsMajor;

        [NotifyPropertyChanged]
        public string WordWithLanguage
            => this.Source.Language == null ? $"{this.Source.Text}" : $"{this.Source.Text} ({this.Source.Language})";
    }
}
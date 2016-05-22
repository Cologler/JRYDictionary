using Jasily.ComponentModel;
using JryDictionary.Models;

namespace JryDictionary
{
    public class WordViewModel : JasilyViewModel<Word>
    {
        private readonly ThingViewModel thing;

        public WordViewModel(ThingViewModel thing, Word source)
            : base(source)
        {
            this.thing = thing;
        }

        public string Thing
        {
            get { return this.thing.Words[0].Word; }
            set { this.thing.Words[0].Word = value; }
        }

        public string Word
        {
            get { return this.Source.Text; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {

                }
                else
                {
                    this.Source.Text = value.Trim();
                    this.thing.Update();
                }
                this.NotifyPropertyChanged(nameof(this.Word));
            }
        }
    }
}
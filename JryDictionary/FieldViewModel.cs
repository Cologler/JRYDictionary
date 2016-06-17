using Jasily.ComponentModel;
using JryDictionary.Models;

namespace JryDictionary
{
    public sealed class FieldViewModel : JasilyViewModel<Field>
    {
        private string displayValue;
        private string thingName;

        public FieldViewModel(Field source)
            : base(source)
        {
            this.displayValue = source.Name;
        }

        public string DisplayValue
        {
            get
            {
                if (this.thingName == null)
                {
                    this.thingName = string.Empty; // empty to sure task was started.
                    this.BeginLoadThingName();
                }
                return this.displayValue;
            }
            private set { this.SetPropertyRef(ref this.displayValue, value); }
        }

        public async void BeginLoadThingName()
        {
            var thing = await App.Current.ThingSetAccessor.FindOneAsync(this.Source.TargetId);
            if (thing != null)
            {
                this.DisplayValue = $"{this.Source.Name} 「{thing.MajorWord().Text}」";
            }
        }
    }
}
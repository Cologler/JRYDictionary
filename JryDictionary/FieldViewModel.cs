using System.Diagnostics;
using Jasily.ComponentModel;
using JryDictionary.Models;

namespace JryDictionary
{
    public sealed class FieldViewModel : JasilyViewModel<Field>
    {
        private string displayValue;
        private string thingName;

        public FieldViewModel(Field source, string thingName = null)
            : base(source)
        {
            this.displayValue = source.Name;
            this.thingName = thingName;
        }

        public string DisplayValue
        {
            get
            {
                this.BeginLoadThingName();
                return this.displayValue;
            }
            private set { this.SetPropertyRef(ref this.displayValue, value); }
        }

        public string ThingName
        {
            get
            {
                this.BeginLoadThingName();
                return this.thingName;
            }
            private set { this.SetPropertyRef(ref this.thingName, value); }
        }

        public async void BeginLoadThingName()
        {
            if (this.thingName != null) return;
            this.thingName = string.Empty;
            Debug.WriteLine($"load field for [{this.Source.TargetId}]");
            var thing = await App.Current.ThingSetAccessor.FindOneAsync(this.Source.TargetId);
            if (thing != null)
            {
                this.ThingName = thing.MajorWord().Text;
                this.DisplayValue = $"{this.Source.Name} 「{thing.MajorWord().Text}」";
            }
        }
    }
}
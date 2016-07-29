using System;
using System.Diagnostics;
using Jasily.ComponentModel;
using JryDictionary.Controls.ThingPreview;
using JryDictionary.Models;
using JryDictionary.Models.Parsers;

namespace JryDictionary
{
    public class FieldViewModel : JasilyViewModel<Field>, IThingPreviewViewModel
    {
        private string displayValue;
        private string thingName;
        private Uri icon;
        private Uri cover;

        public FieldViewModel(Field source)
            : base(source)
        {
            this.displayValue = source.Name;
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

        public string Name
        {
            get
            {
                this.BeginLoadThingName();
                return this.thingName;
            }
            protected set { this.SetPropertyRef(ref this.thingName, value); }
        }

        public Uri Icon
        {
            get { return this.icon; }
            private set { this.SetPropertyRef(ref this.icon, value); }
        }

        public async void BeginLoadThingName()
        {
            if (this.thingName != null) return;
            this.thingName = string.Empty;
            Debug.WriteLine($"load field for [{this.TargetId}]");
            var thing = await App.Current.ThingSetAccessor.FindOneAsync(this.TargetId);
            if (thing != null)
            {
                this.Name = thing.MajorWord().Text;
                this.DisplayValue = $"{this.Source.Name} 「{thing.MajorWord().Text}」";
                var uriParser = new ImageUriParser();
                this.Icon = uriParser.TryParse(thing.Icon)?.Uri;
                this.Cover = uriParser.TryParse(thing.Cover)?.Uri;
            }
        }

        public virtual string TargetId => this.Source.TargetId;

        public Uri Cover
        {
            get { return this.cover; }
            private set { this.SetPropertyRef(ref this.cover, value); }
        }
    }
}
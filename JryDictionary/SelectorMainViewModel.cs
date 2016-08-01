using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JryDictionary.Models;

namespace JryDictionary
{
    public class SelectorMainViewModel : MainViewModel
    {
        private readonly string targetThingId;
        private Thing targetThing;

        public SelectorMainViewModel(string thingId)
        {
            this.targetThingId = thingId;
            this.FooterHeader = "field name".ToUpper();
        }

        [CanBeNull]
        public Thing TargetThing
        {
            get { return this.targetThing; }
            private set { this.SetPropertyRef(ref this.targetThing, value); }
        }

        public override MainViewModelType ViewModelType => MainViewModelType.Selector;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.TargetThing = await App.Current.ThingSetAccessor.FindOneAsync(this.targetThingId);
            if (this.TargetThing == null) return;

            this.RefreshProperties();
        }

        public override string WindowTitle
            => this.TargetThing != null
            ? $"select field for ¡¸{this.TargetThing.Words[0].Text}¡¹"
            : "missing select target";

        public override async Task<bool> CommitFooterInputAsnyc()
        {
            var value = this.FooterContent;
            if (string.IsNullOrWhiteSpace(value)) return false;

            var selected = this.Words.Selected?.Thing.Source;
            if (selected == null)
            {
                if (this.Things.Count == 1)
                {
                    selected = this.Things[0].Source;
                }
                else
                {
                    return false;
                }
            }
            this.FooterContent = string.Empty;

            var thing = await App.Current.ThingSetAccessor.FindOneAsync(this.targetThingId);
            if (thing == null) return false;

            if (thing.Fields == null) thing.Fields = new List<Field>();
            var field = new Field
            {
                Name = value,
                TargetId = selected.Id
            };
            thing.Fields.Add(field);
            await App.Current.ThingSetAccessor.UpdateAsync(thing);
            return true;
        }
    }
}
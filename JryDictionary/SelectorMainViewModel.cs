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
        }

        [CanBeNull]
        public Thing TargetThing
        {
            get { return this.targetThing; }
            private set { this.SetPropertyRef(ref this.targetThing, value); }
        }

        #region Overrides of MainViewModel

        public override MainViewModelType ViewModelType => MainViewModelType.Selector;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.TargetThing = await App.Current.ThingSetAccessor.FindOneAsync(this.targetThingId);
            if (this.TargetThing == null) return;
        }

        public override string WindowTitle => this.TargetThing != null ? $"select for ¡¸{this.TargetThing.Words[0].Text}¡¹" : "missing select target";

        #endregion
    }
}
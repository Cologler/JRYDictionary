using JryDictionary.Models;

namespace JryDictionary.Controls.ThingViewer
{
    public class FieldReverseViewModel : FieldViewModel
    {
        public Thing Thing { get; }

        public FieldReverseViewModel(Thing thing, Field field)
            : base(field)
        {
            this.Thing = thing;
            this.ThingName = thing.MajorWord().Text;
        }

        public override string TargetId => this.Thing.Id;
    }
}
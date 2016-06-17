using System.Collections.Generic;
using System.Linq;
using Jasily.Collections.Generic;

namespace JryDictionary.Controls.ThingViewer
{
    public class ThingViewerViewModel
    {
        public ThingViewModel ThingViewModel { get; }

        public ThingViewerViewModel(ThingViewModel thingViewModel)
        {
            this.ThingViewModel = thingViewModel;
            this.GroupedFields = this.ThingViewModel.Fields.GroupBy(z => z.Source.Name).Select(z => z.ToList()).ToList();
        }

        public List<GroupedList<string, FieldViewModel>> GroupedFields { get; }

        public static implicit operator ThingViewerViewModel(ThingViewModel thingViewModel)
            => new ThingViewerViewModel(thingViewModel);
    }
}
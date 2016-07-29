using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using Jasily.Collections.Generic;
using Jasily.ComponentModel;
using JryDictionary.Models;
using JryDictionary.Models.Parsers;
using JryDictionary.Modules.Copyer;

namespace JryDictionary.Controls.ThingViewer
{
    public class ThingViewerViewModel : JasilyViewModel
    {
        private readonly HashSet<string> existsFields = new HashSet<string>();

        public ThingViewModel ThingViewModel { get; }

        public ThingViewerViewModel(ThingViewModel thingViewModel)
        {
            var uriParser = new ImageUriParser();
            this.ThingViewModel = thingViewModel;
            this.Background = uriParser.TryParse(thingViewModel.Source.Background)?.Uri;
            this.Cover = uriParser.TryParse(thingViewModel.Source.Cover)?.Uri;

            this.GroupedFields = new ObservableCollection<GroupedList<string, FieldViewModel>>(
                this.ThingViewModel.Fields.GroupBy(z => z.Source.Name).Select(z => z.ToList()));
            this.existsFields.AddRange(this.ThingViewModel.Fields.Select(z => z.Source.TargetId));

            this.BeginGetFieldsReverse();

            this.Copyers.AddRange(App.Current.ModuleManager.Copyers);
        }

        public ObservableCollection<GroupedList<string, FieldViewModel>> GroupedFields { get; }

        public string Description => this.ThingViewModel.Source.Description;

        [NotifyPropertyChanged]
        public Uri Background { get; private set; }

        [NotifyPropertyChanged]
        public Uri Cover { get; private set; }

        public IEnumerable<Inline> BuildDescriptionInlines()
        {
            if (string.IsNullOrEmpty(this.Description)) return Empty<Inline>.Array;

            var desc = new DescriptionParser(this.Description).ParseBody();
            var uriParser = new ImageUriParser();
            this.Background = this.Background ?? uriParser.TryParse(desc.Background)?.Uri;
            this.Cover = this.Cover ?? uriParser.TryParse(desc.Cover)?.Uri;
            this.RefreshProperties();
            return desc.Inlines;
        }

        public async void BeginGetFieldsReverse()
        {
            var thisId = this.ThingViewModel.Source.Id;
            var items = await Task.Run(async () =>
            {
                return (await App.Current.ThingSetAccessor.FindFieldReverseAsync(this.ThingViewModel.Source))
                    .Where(z => !this.existsFields.Contains(z.Id))
                    .Select(z => new FieldReverseViewModel(z, new Field
                    {
                        Name = z.Fields?.First(x => x.TargetId == thisId).Name + " OF",
                        TargetId = thisId
                    }) as FieldViewModel)
                    .GroupBy(z => z.Source.Name)
                    .Select(z => z.ToList())
                    .ToArray();
            });
            this.GroupedFields.AddRange(items);
            if (items.Length > 0 && this.GroupedFields.Count == items.Length)
            {
                this.NotifyPropertyChanged(nameof(this.GroupedFields));
            }
        }

        public ObservableCollection<IWordCopyer> Copyers { get; } = new ObservableCollection<IWordCopyer>();
    }
}
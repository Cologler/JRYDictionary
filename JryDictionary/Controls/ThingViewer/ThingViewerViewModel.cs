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
using JryDictionary.Modules.Copyer;

namespace JryDictionary.Controls.ThingViewer
{
    public class ThingViewerViewModel : JasilyViewModel
    {
        private readonly HashSet<string> existsFields = new HashSet<string>();
        private string background;

        public ThingViewModel ThingViewModel { get; }

        public ThingViewerViewModel(ThingViewModel thingViewModel)
        {
            this.ThingViewModel = thingViewModel;
            this.GroupedFields = new ObservableCollection<GroupedList<string, FieldViewModel>>(
                this.ThingViewModel.Fields.GroupBy(z => z.Source.Name).Select(z => z.ToList()));
            this.existsFields.AddRange(this.ThingViewModel.Fields.Select(z => z.Source.TargetId));
            //this.Document = "[[12]]..[[2\\]]...[[4325]].";
            this.BeginGetFieldsReverse();

            this.Copyers.AddRange(App.Current.ModuleManager.Copyers);
        }

        public ObservableCollection<GroupedList<string, FieldViewModel>> GroupedFields { get; }

        public string Description => this.ThingViewModel.Source.Description;

        [NotifyPropertyChanged]
        public string Background { get; private set; }

        [NotifyPropertyChanged]
        public string Cover { get; private set; }

        public IEnumerable<Inline> BuildDescriptionInlines()
        {
            if (string.IsNullOrEmpty(this.Description)) return Empty<Inline>.Array;

            var desc = new Description(this.Description);
            this.Background = desc.Background;
            this.Cover = desc.Cover;
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
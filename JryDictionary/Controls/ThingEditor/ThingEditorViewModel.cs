using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jasily.ComponentModel.Editable;
using Jasily.Diagnostics;
using JryDictionary.Models;

namespace JryDictionary.Controls.ThingEditor
{
    public sealed class ThingEditorViewModel : JasilyEditableViewModel<Thing>
    {
        private readonly Thing source;
        private WordEditorViewModel majorWord;

        public ThingEditorViewModel(Thing source)
        {
            this.source = source;
            Debug.Assert(source != null);
            this.ReadFromObject(source);
        }

        public ObservableCollection<WordEditorViewModel> Words { get; } = new ObservableCollection<WordEditorViewModel>();

        public WordEditorViewModel MajorWord
        {
            get { return this.majorWord; }
            private set { this.SetPropertyRef(ref this.majorWord, value); }
        }

        #region Overrides of JasilyEditableViewModel<Thing>

        public override void ReadFromObject(Thing obj)
        {
            base.ReadFromObject(obj);
            this.Words.Reset(obj.Words.Select(z => new WordEditorViewModel(z)));
            this.SetMajor(this.Words[0]);
            this.AddNewViewModel();
            this.Categorys.Add(obj.Category);
        }

        public void AddNewViewModel()
        {
            var next = new WordEditorViewModel();
            this.Words.Add(next);
            next.ContentChanged += this.NewWord_ContentChanged;
        }

        private void NewWord_ContentChanged(WordEditorViewModel sender)
        {
            sender.ContentChanged -= this.NewWord_ContentChanged;
            sender.IsNew = false;
            this.AddNewViewModel();
        }

        public override void WriteToObject(Thing obj)
        {
            base.WriteToObject(obj);
            Debug.Assert(this.MajorWord.IsMajar = true);
            JasilyDebug.AssertForEach(this.Words.Where(z => z != this.MajorWord), z => !z.IsMajar);
            obj.Words.Clear();
            var dict = new Dictionary<string, Word>();
            var major = this.MajorWord.Flush();
            obj.Words.Add(major);
            dict.Add(major.Text, major);
            foreach (var word in this.Words.Where(z => z != this.MajorWord && !z.IsNew).Select(z => z.Flush()))
            {
                var exists = dict.GetValueOrDefault(word.Text);
                if (exists != null)
                {
                    // test can combine ?
                    Debug.Assert(exists.Text == word.Text);
                }
                else
                {
                    dict.Add(word.Text, word);
                    obj.Words.Add(word);
                }
            }
        }

        #endregion

        public Task CommitAsync()
        {
            this.WriteToObject(this.source);
            return App.Current.ThingSetAccessor.UpdateAsync(this.source);
        }

        public void Remove(WordEditorViewModel word)
        {
            Debug.Assert(word != this.MajorWord);
            this.Words.Remove(word);
        }

        public void SetMajor(WordEditorViewModel word)
        {
            Debug.Assert(word != this.MajorWord);
            if (this.MajorWord != null)
            {
                this.MajorWord.IsMajar = false;
            }
            this.MajorWord = word;
            this.MajorWord.IsMajar = true;
        }

        public async Task InitializeAsync()
        {
            this.ExistsLanguages.AddRange(await App.Current.ThingSetAccessor.GroupLanguagesAsync());
            this.ExistsCategorys.AddRange(await App.Current.ThingSetAccessor.GroupCategorysAsync());
        }

        public ObservableCollection<string> ExistsLanguages { get; } = new ObservableCollection<string>();

        [EditableField]
        [WriteToObjectIfNotNullOrWhiteSpace]
        public string Category { get; set; }

        public ObservableCollection<string> ExistsCategorys { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Categorys { get; } = new ObservableCollection<string>();
    }
}
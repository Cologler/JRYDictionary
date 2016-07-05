using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jasily.ComponentModel.Editable;
using Jasily.ComponentModel.Editable.Converters;
using JryDictionary.Models;

namespace JryDictionary.Controls.ThingEditor
{
    public sealed class ThingEditorViewModel : JasilyEditableViewModel<Thing>
    {
        private WordEditorViewModel majorWord;
        private ObservableCollection<Field> fields;
        private string description;

        public ThingEditorViewModel(Thing source)
        {
            Debug.Assert(source != null);
            this.ReadFromObject(source);
        }

        public ObservableCollection<WordEditorViewModel> Words { get; } = new ObservableCollection<WordEditorViewModel>();

        public WordEditorViewModel MajorWord
        {
            get { return this.majorWord; }
            private set { this.SetPropertyRef(ref this.majorWord, value); }
        }

        public override void ReadFromObject(Thing obj)
        {
            base.ReadFromObject(obj);

            // word
            this.Words.Reset(obj.Words.Select(z => new WordEditorViewModel(z) { Text = z.Text }));
            this.SetMajor(this.Words[0]);

            // category
            if (obj.Categorys != null) this.Categorys.AddRange(obj.Categorys);

            // next word
            this.AddNewViewModel();
        }

        private void AddNewViewModel()
        {
            var next = new WordEditorViewModel();
            this.Words.Add(next);
            next.ContentChanged += this.NewWord_ContentChanged;
        }

        private void NewWord_ContentChanged(WordEditorViewModel sender)
        {
            if (sender.Status != WordEditorStatus.New)
            {
                sender.ContentChanged -= this.NewWord_ContentChanged;
                this.AddNewViewModel();
            }
        }

        public override void WriteToObject(Thing obj)
        {
            base.WriteToObject(obj);

            // word
            // -- test
            Debug.Assert(this.Words.Single(z => z.Status == WordEditorStatus.Major) == this.MajorWord);
            // -- write
            obj.Words.Clear();
            var dict = new Dictionary<string, Word>();
            var major = this.MajorWord.Flush();
            major.Text = this.MajorWord.Text;
            Debug.Assert(!string.IsNullOrWhiteSpace(major.Text));
            obj.Words.Add(major);
            dict.Add(major.Text, major);
            foreach (var line in this.Words
                .Where(z => z.Status == WordEditorStatus.Value)
                .Select(z => new { S = z, V = z.Text.AsLines().Select(x => x.Trim()).Where(x => x.Length > 0).ToArray() }))
            {
                foreach (var v in line.V)
                {
                    var word = line.S.Flush();
                    word.Text = v;
                    Debug.Assert(word.Text != null);

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

            // category
            obj.Categorys = this.Categorys.Count > 0 ? this.Categorys.ToList() : null;
        }

        public Task CommitAsync()
        {
            this.WriteToObject(this.ReadCached);
            return App.Current.ThingSetAccessor.UpdateAsync(this.ReadCached);
        }

        public void Remove(WordEditorViewModel word)
        {
            Debug.Assert(word != this.MajorWord);
            this.Words.Remove(word);
        }

        public void SetMajor(WordEditorViewModel word)
        {
            Debug.Assert(word != this.MajorWord);
            if (string.IsNullOrWhiteSpace(word.Text) || word.Text.AsLines().Length > 1) return;
            if (this.MajorWord != null)
            {
                this.MajorWord.Status = WordEditorStatus.Value;
            }
            this.MajorWord = word;
            this.MajorWord.Status = WordEditorStatus.Major;
        }

        public async Task InitializeAsync()
        {
            this.ExistsLanguages.AddRange(await App.Current.ThingSetAccessor.GroupLanguagesAsync());
            this.ExistsCategorys.AddRange(await App.Current.ThingSetAccessor.GroupCategorysAsync());
        }

        public ObservableCollection<string> ExistsLanguages { get; } = new ObservableCollection<string>();

        public string CategoryInput { get; set; }

        public ObservableCollection<string> ExistsCategorys { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Categorys { get; } = new ObservableCollection<string>();

        public void AddCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return;
            category = category.Trim();
            if (this.Categorys.Contains(category)) return;
            this.Categorys.Add(category);
        }

        [EditableField(Converter = typeof(EmptyToNullCollectionConverter<List<Field>, ObservableCollection<Field>, Field>))]
        public ObservableCollection<Field> Fields
        {
            get { return this.fields; }
            private set { this.SetPropertyRef(ref this.fields, value); }
        }

        [EditableField(Converter = typeof(WhiteSpaceToNullOrTrimStringConverter))]
        public string Description
        {
            get { return this.description; }
            set { this.SetPropertyRef(ref this.description, value); }
        }
    }
}
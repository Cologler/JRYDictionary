using System;
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

            // word
            this.Words.Reset(obj.Words.Select(z => new WordEditorViewModel(z) { Text = z.Text }));
            this.SetMajor(this.Words[0]);

            // category
            if (obj.Categorys != null) this.Categorys.AddRange(obj.Categorys);

            // next word
            this.AddNewViewModel();
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

            // word
            // -- test
            Debug.Assert(this.MajorWord.IsMajar = true);
            JasilyDebug.AssertForEach(this.Words.Where(z => z != this.MajorWord), z => !z.IsMajar);
            // -- write
            obj.Words.Clear();
            var dict = new Dictionary<string, Word>();
            var major = this.MajorWord.Flush();
            major.Text = this.MajorWord.Text;
            Debug.Assert(!string.IsNullOrWhiteSpace(major.Text));
            obj.Words.Add(major);
            dict.Add(major.Text, major);
            foreach (var line in this.Words
                .Where(z => z != this.MajorWord && !z.IsNew && !string.IsNullOrWhiteSpace(z.Text))
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
            if (string.IsNullOrWhiteSpace(word.Text) || word.Text.AsLines().Length > 1) return;
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
    }
}
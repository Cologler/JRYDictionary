using System;
using System.Diagnostics;
using Jasily.ComponentModel.Editable;
using JryDictionary.Models;

namespace JryDictionary
{
    public sealed class WordEditorViewModel : JasilyEditableViewModel<Word>, IDisposable
    {
        private bool isMajar;
        private bool isNew;
        private string text;
        private string language;
        public event TypedEventHandler<WordEditorViewModel> ContentChanged;

        public WordEditorViewModel(Word source)
        {
            Debug.Assert(source != null);
            this.ReadFromObject(source);
        }

        public WordEditorViewModel()
        {
            this.isNew = true;
        }

        [EditableField]
        public string Language
        {
            get { return this.language; }
            set
            {
                value = value.IsNullOrWhiteSpace() ? null : value.Trim();

                if (this.SetPropertyRef(ref this.language, value))
                {
                    this.ContentChanged?.Invoke(this);
                }
            }
        }

        [EditableField]
        public string Text
        {
            get { return this.text; }
            set
            {
                value = value.IsNullOrWhiteSpace() ? null : value.Trim();
                if (value == null) return;

                if (this.SetPropertyRef(ref this.text, value))
                {
                    this.ContentChanged?.Invoke(this);
                }
            }
        }

        public Word Flush()
        {
            var obj = new Word();
            this.WriteToObject(obj);
            return obj;
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.WriteToObject(this.ReadCached);

        #endregion

        public bool IsMajar
        {
            get { return this.isMajar; }
            set
            {
                if (this.isMajar == value) return;
                this.isMajar = value;
                this.NotifyPropertyChanged(nameof(this.CanBeMajor));
                this.NotifyPropertyChanged(nameof(this.CanRemove));
            }
        }

        public bool IsNew
        {
            get { return this.isNew; }
            set
            {
                if (this.isNew == value) return;
                this.isNew = value;
                this.NotifyPropertyChanged(nameof(this.CanBeMajor));
                this.NotifyPropertyChanged(nameof(this.CanRemove));
            }
        }

        public bool CanBeMajor => !this.IsMajar && !this.IsNew;

        public bool CanRemove => !this.IsMajar && !this.IsNew;
    }
}
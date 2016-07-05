using System;
using System.Diagnostics;
using System.Windows.Media;
using Jasily.ComponentModel.Editable;
using JryDictionary.Models;

namespace JryDictionary.Controls.ThingEditor
{
    public sealed class WordEditorViewModel : JasilyEditableViewModel<Word>, IDisposable
    {
        private string text;
        private string language;
        private WordEditorStatus status;
        public event TypedEventHandler<WordEditorViewModel> ContentChanged;

        public WordEditorViewModel(Word source)
        {
            Debug.Assert(source != null);
            this.status = WordEditorStatus.Value;
            this.ReadFromObject(source);
        }

        public WordEditorViewModel()
        {
            this.status = WordEditorStatus.New;
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

        /// <summary>
        /// use \r\n to convert to mulit-word.
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set
            {
                var isWhiteSpace = string.IsNullOrWhiteSpace(value);

                if (this.Status == WordEditorStatus.Major)
                {
                    if (isWhiteSpace) return;
                    value = value.Trim(); // no mulit line.
                }

                switch (this.Status)
                {
                    case WordEditorStatus.Major:
                        if (isWhiteSpace) return;
                        value = value.Trim(); // no allowed mulit line.
                        break;

                    case WordEditorStatus.Value:
                    case WordEditorStatus.Empty:
                        this.Status = isWhiteSpace ? WordEditorStatus.Empty : WordEditorStatus.Value;
                        break;

                    case WordEditorStatus.New:
                        if (!isWhiteSpace) this.Status = WordEditorStatus.Value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (this.SetPropertyRef(ref this.text, value))
                {
                    this.ContentChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// return new word clone from this (without text).
        /// </summary>
        /// <returns></returns>
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

        public WordEditorStatus Status
        {
            get { return this.status; }
            set
            {
                if (this.status == value) return;
                this.status = value;
                this.NotifyPropertyChanged(nameof(this.StatusBrush));
                this.NotifyPropertyChanged(nameof(this.CanBeMajor));
                this.NotifyPropertyChanged(nameof(this.CanRemove));
            }
        }

        public Brush StatusBrush
        {
            get
            {
                switch (this.Status)
                {
                    case WordEditorStatus.Major: return Brushes.Red;
                    case WordEditorStatus.Value: return Brushes.BlueViolet;
                    case WordEditorStatus.Empty: return Brushes.DodgerBlue;
                    case WordEditorStatus.New: return Brushes.LimeGreen;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool CanBeMajor => this.Status == WordEditorStatus.Value;

        public bool CanRemove => this.Status != WordEditorStatus.Major && this.Status != WordEditorStatus.New;
    }
}
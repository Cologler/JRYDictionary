using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    public abstract class WordCopyer : IWordCopyer
    {
        public virtual string Name => this.GetType().Name.Replace("WordCopyer", "").ToLower();

        public abstract void Copy(Thing thing, Word word);

        protected static async void CopyToClipboard(string text)
        {
            var time = 0;
            while (time < 5)
            {
                try
                {
                    Clipboard.SetText(text);
                    return;
                }
                catch
                {
                    // ignored
                }
                time++;
                await Task.Delay(50);
            }
            Debug.WriteLine("copy failed.");
        }

        protected static string[] SplitTextAsWords(string text)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text)) return text.IntoArray();

            /*
             * Ab_c   => Ab, c
             * ABC    => ABC
             * ABC_D  => ABC, D
             * AbCd   => Ab, Cd
             */
            var texts = text.Split(new[] { " ", "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (texts.Length == 0) return texts;
            if (texts.Length == 1)
            {
                if (text.All(char.IsUpper)) // abbr.
                {
                    return texts;
                }
                else
                {
                    return texts[0].SplitWhen(char.IsUpper, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            else
            {
                return texts;
            }
        }
    }
}
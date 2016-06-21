using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    public abstract class WordCopyer : IWordCopyer
    {
        public abstract string Name { get; }

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
    }
}
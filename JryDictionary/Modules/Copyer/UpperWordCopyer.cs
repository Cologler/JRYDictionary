using System.ComponentModel.Composition;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    [Export(typeof(IWordCopyer))]
    public class UpperClipboardWordCopyer : WordCopyer
    {
        public override string Name => "upper to clipboard";

        public override void Copy(Thing thing, Word word)
        {
            CopyToClipboard(word.Text.ToUpper());
        }
    }
}
using System.ComponentModel.Composition;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    [Export(typeof(IWordCopyer))]
    public class LowerClipboardWordCopyer : WordCopyer
    {
        public override string Name => "lower to clipboard";

        public override void Copy(Thing thing, Word word)
        {
            CopyToClipboard(word.Text.ToLower());
        }
    }
}
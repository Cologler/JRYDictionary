using System.ComponentModel.Composition;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    [Export(typeof(IWordCopyer))]
    public sealed class LowerWordCopyer : WordCopyer
    {
        public override string Name => "lower";

        public override void Copy(Thing thing, Word word) => CopyToClipboard(word.Text.ToLower());
    }
}
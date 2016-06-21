using System.ComponentModel.Composition;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    [Export(typeof(IWordCopyer))]
    public sealed class UpperWordCopyer : WordCopyer
    {
        public override string Name => "upper";

        public override void Copy(Thing thing, Word word) => CopyToClipboard(word.Text.ToUpper());
    }
}
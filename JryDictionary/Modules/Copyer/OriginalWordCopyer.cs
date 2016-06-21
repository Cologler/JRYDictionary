using System.ComponentModel.Composition;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    [Export(typeof(IWordCopyer))]
    public sealed class OriginalWordCopyer : WordCopyer, IOrderable
    {
        public override string Name => "original";

        public override void Copy(Thing thing, Word word) => CopyToClipboard(word.Text);

        public int GetOrderCode() => -3;
    }
}
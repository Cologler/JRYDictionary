using System.ComponentModel.Composition;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    [Export(typeof(IWordCopyer))]
    public sealed class ClipboardWordCopyer : WordCopyer, IOrderable
    {
        public override string Name => "to clipboard";

        public override void Copy(Thing thing, Word word) => CopyToClipboard(word.Text);

        public int GetOrderCode() => -3;
    }
}
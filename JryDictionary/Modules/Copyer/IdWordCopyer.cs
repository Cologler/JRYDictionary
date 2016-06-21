using System.ComponentModel.Composition;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    [Export(typeof(IWordCopyer))]
    public sealed class IdWordCopyer : WordCopyer, IOrderable
    {
        public override string Name => "Id";

        public override void Copy(Thing thing, Word word) => CopyToClipboard(thing.Id.ToUpper());

        public int GetOrderCode() => 4;
    }
}
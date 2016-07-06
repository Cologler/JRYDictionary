using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    /// <summary>
    /// ¥ÛÕ’∑Â
    /// </summary>
    [Export(typeof(IWordCopyer))]
    public sealed class PascalWordCopyer : WordCopyer, IOrderable
    {
        public override void Copy(Thing thing, Word word)
        {
            var words = SplitTextAsWords(word.Text).ToArray();
            for (var i = 0; i < words.Length; i++)
            {
                Debug.Assert(words[i].Length > 0);
                words[i] = words[i].ToLower().ReplaceChar(char.ToUpper(words[i][0]), 0);
            }
            CopyToClipboard(words.ConcatAsString());
        }


        public int GetOrderCode() => 101;
    }
}
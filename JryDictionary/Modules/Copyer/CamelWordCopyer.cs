using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    /// <summary>
    /// 小驼峰
    /// </summary>
    [Export(typeof(IWordCopyer))]
    public sealed class CamelWordCopyer : WordCopyer, IOrderable
    {
        public override void Copy(Thing thing, Word word)
        {
            var words = SplitTextAsWords(word.Text).ToArray();
            words[0] = words[0].ToLower();
            for (var i = 1; i < words.Length; i++)
            {
                Debug.Assert(words[i].Length > 0);
                words[i] = words[i].ToLower().ReplaceChar(char.ToUpper(words[i][0]), 0);
            }
            CopyToClipboard(words.ConcatAsString());
        }

        public int GetOrderCode() => 100;
    }
}
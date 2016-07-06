using System;
using System.ComponentModel.Composition;
using System.Linq;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    /// <summary>
    /// 句子
    /// </summary>
    [Export(typeof(IWordCopyer))]
    public sealed class SentenceWordCopyer : WordCopyer, IOrderable
    {
        public override string Name => "Sentence";

        public override void Copy(Thing thing, Word word)
        {
            var words = SplitTextAsWords(word.Text).ToArray();
            for (var i = 0; i < words.Length; i++) words[i] = words[i].ToLower();
            if (words.Length > 0)
            {
                words[0] = words[0].ToLower().ReplaceChar(char.ToUpper(words[0][0]), 0);
            }
            CopyToClipboard(words.JoinAsString(" "));
        }

        public int GetOrderCode() => 102;
    }
}
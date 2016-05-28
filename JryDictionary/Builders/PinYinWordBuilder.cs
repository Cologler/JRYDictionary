using System.Collections.Generic;
using System.ComponentModel.Composition;
using Jasily;
using Jasily.Chinese.PinYin;
using JryDictionary.Models;

namespace JryDictionary.Builders
{
    [Export(typeof(IWordBuilder))]
    public sealed class PinYinWordBuilder : IWordBuilder, IOrderable
    {
        private PinYinManager pinYinManager;

        #region Implementation of IWordBuilder

        public string Name => "PinYin";

        public IEnumerable<Word> Build(Thing thing, Word word)
        {
            if (this.pinYinManager == null) this.pinYinManager = PinYinManager.CreateInstance();

            var containPinyin = false;
            var chars = word.Text.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                Pinyin ret;
                if (this.pinYinManager.TryGetFirstPinYin(chars[i], out ret))
                {
                    chars[i] = ret.PinYin[0];
                    containPinyin = true;
                }
            }
            if (containPinyin)
            {
                yield return new Word
                {
                    Text = new string(chars),
                    Language = this.Name
                };
            }
        }

        #endregion

        #region Implementation of IOrderable

        public int GetOrderCode() => 10;

        #endregion
    }
}
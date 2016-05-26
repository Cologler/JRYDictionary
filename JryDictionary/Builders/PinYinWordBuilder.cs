using Jasily.Chinese.PinYin;
using JryDictionary.Models;

namespace JryDictionary.Builders
{
    public sealed class PinYinWordBuilder : IWordBuilder
    {
        private PinYinManager pinYinManager;

        #region Implementation of IWordBuilder

        public string Name => "PinYin";

        public Word Build(Thing thing, Word word)
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
            if (!containPinyin) return null;
            return new Word
            {
                Text = new string(chars),
                Language = this.Name
            };
        }

        #endregion
    }
}
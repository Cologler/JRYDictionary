using System.Collections.Generic;
using System.ComponentModel.Composition;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Builders
{
    [Export(typeof(IWordBuilder))]
    public sealed class UpperWordBuilder : IWordBuilder, IOrderable
    {
        #region Implementation of IWordBuilder

        public string Name => "Upper";

        /// <summary>
        /// return null if build failed.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public IEnumerable<Word> Build(Thing thing, Word word)
        {
            var ret = word.Text.ToUpper();
            if (ret != word.Text)
            {
                yield return new Word
                {
                    Text = ret,
                    Language = this.Name
                };
            }
        }

        #endregion

        #region Implementation of IOrderable

        public int GetOrderCode() => 3;

        #endregion
    }
}
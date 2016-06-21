using System.Collections.Generic;
using System.ComponentModel.Composition;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Modules.Builders
{
    [Export(typeof(IWordBuilder))]
    public sealed class LowerWordBuilder : IWordBuilder, IOrderable
    {
        #region Implementation of IWordBuilder

        public string Name => "Lower";

        /// <summary>
        /// return null if build failed.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public IEnumerable<Word> Build(Thing thing, Word word)
        {
            var ret = word.Text.ToLower();
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

        public int GetOrderCode() => 2;

        #endregion
    }
}
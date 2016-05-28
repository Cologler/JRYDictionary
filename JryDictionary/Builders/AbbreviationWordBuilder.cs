using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Jasily;
using JryDictionary.Models;

namespace JryDictionary.Builders
{
    [Export(typeof(IWordBuilder))]
    public sealed class AbbreviationWordBuilder : IWordBuilder, IOrderable
    {
        #region Implementation of IWordBuilder

        public string Name => "Abbr";

        /// <summary>
        /// return null if build failed.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public IEnumerable<Word> Build(Thing thing, Word word)
        {
            var ret = word.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(z => z[0]).GetString();
            if (ret.Length > 1)
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

        public int GetOrderCode() => 1;

        #endregion
    }
}
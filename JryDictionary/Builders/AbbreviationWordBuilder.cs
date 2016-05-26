using System;
using System.Linq;
using JryDictionary.Models;

namespace JryDictionary.Builders
{
    public sealed class AbbreviationWordBuilder : IWordBuilder
    {
        #region Implementation of IWordBuilder

        public string Name => "Abbr";

        /// <summary>
        /// return null if build failed.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public Word Build(Thing thing, Word word)
        {
            var ret = word.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(z => z[0]).GetString();
            if (ret.Length > 1)
            {
                return new Word
                {
                    Text = ret,
                    Language = this.Name
                };
            }
            return null;
        }

        #endregion
    }
}
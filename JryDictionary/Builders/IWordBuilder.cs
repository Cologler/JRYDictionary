using JetBrains.Annotations;
using JryDictionary.Models;

namespace JryDictionary.Builders
{
    public interface IWordBuilder
    {
        string Name { get; }

        /// <summary>
        /// return null if build failed.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        [CanBeNull]
        Word Build(Thing thing, Word word);
    }
}
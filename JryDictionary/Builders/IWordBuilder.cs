using System.Collections.Generic;
using JryDictionary.Models;

namespace JryDictionary.Builders
{
    public interface IWordBuilder
    {
        string Name { get; }

        IEnumerable<Word> Build(Thing thing, Word word);
    }
}
using System.Collections.Generic;
using JryDictionary.Models;

namespace JryDictionary.Modules.Builders
{
    public interface IWordBuilder
    {
        string Name { get; }

        IEnumerable<Word> Build(Thing thing, Word word);
    }
}
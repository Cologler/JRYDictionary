using JryDictionary.Models;

namespace JryDictionary.Modules.Copyer
{
    public interface IWordCopyer
    {
        string Name { get; }

        void Copy(Thing thing, Word word);
    }
}
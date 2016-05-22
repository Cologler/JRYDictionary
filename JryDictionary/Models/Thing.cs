using System.Collections.Generic;
using JetBrains.Annotations;

namespace JryDictionary.Models
{
    public sealed class Thing
    {
        public Thing()
        {
            this.Words = new List<Word>();
        }

        [NotNull]
        public string Id { get; set; }

        [NotNull]
        public List<Word> Words { get; set; }
    }
}

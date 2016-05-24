using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace JryDictionary.Models
{
    public sealed class Word
    {
        [CanBeNull]
        [BsonIgnoreIfDefault]
        public string Language { get; set; }

        [NotNull]
        public string Text { get; set; }
    }
}

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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() => this.Text;
    }
}

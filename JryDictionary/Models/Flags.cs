using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace JryDictionary.Models
{
    public sealed class Flags : SettingEntity
    {
        public Flags()
        {
            this.Id = "Flags";
        }

        [BsonIgnoreIfDefault]
        public List<string> Groups { get; set; }

        [BsonIgnoreIfDefault]
        public List<string> Languages { get; set; }
    }
}
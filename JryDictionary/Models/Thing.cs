﻿using System.Collections.Generic;
using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;

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

        [CanBeNull]
        [BsonIgnoreIfDefault]
        [ItemNotNull]
        public List<string> Categorys { get; set; }

        [CanBeNull]
        [BsonIgnoreIfDefault]
        [ItemNotNull]
        public List<Field> Fields { get; set; }
    }
}

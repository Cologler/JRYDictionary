﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jasily.ComponentModel;
using JryDictionary.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JryDictionary.DbAccessors
{
    public class ThingSetAccessor
    {
        private HashSet<string> languages;

        public ThingSetAccessor(IMongoDatabase db)
        {
            this.Collection = db.GetCollection<Thing>("Thing");
        }

        public IMongoCollection<Thing> Collection { get; }

        public async Task UpdateAsync(Thing thing)
        {
            Debug.Assert(thing.Id != null);

            thing.Words = thing.Words.Skip(1).OrderBy(z => z.Language).Insert(0, thing.Words[0]).ToList();
            var languages = thing.Words.Where(z => z.Language != null).Select(z => z.Language).Distinct().ToArray();
            if (languages.Length > 0)
            {
                if (this.languages == null)
                {
                    await this.GroupAsync();
                }
                Debug.Assert(this.languages != null);
                this.languages.AddRange(languages);
            }
            await this.Collection.ReplaceOneAsync(
                new FilterDefinitionBuilder<Thing>().Eq(z => z.Id, thing.Id),
                thing);
        }

        public void Initialize()
        {
            var index = new IndexKeysDefinitionBuilder<Thing>().Ascending(
                PropertySelector<Thing>.Start(z => z)
                    .SelectMany(z => z.Words)
                    .Select(z => z.Text)
                    .ToString());
            this.Collection.Indexes.CreateOne(index, new CreateIndexOptions
            {
                Version = 1,
                Background = true
            });
        }

        public async Task<string[]> GroupAsync()
        {
            if (this.languages == null)
            {
                PipelineDefinition<Thing, GroupResult> pipeline = new[]
                {
                    new BsonDocument
                    {
                        { "$unwind", "$Words" }
                    },
                    new BsonDocument
                    {
                        { "$group", new BsonDocument("_id", "$Words.Language") }
                    }
                };
                var ret = await (await this.Collection.AggregateAsync(pipeline)).ToListAsync();
                this.languages = new HashSet<string>(ret.Select(z => z.Id).Where(z => z != null));
            }

            return this.languages.ToArray();
        }

        private class GroupResult
        {
            public string Id { get; set; }
        }
    }
}
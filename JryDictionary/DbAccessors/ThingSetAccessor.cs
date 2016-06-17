using System;
using System.Collections.Generic;
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
        private HashSet<string> categorys;
        public event EventHandler<string> SavedNewCategory;

        public ThingSetAccessor(IMongoDatabase db)
        {
            this.Collection = db.GetCollection<Thing>("Thing");
        }

        private IMongoCollection<Thing> Collection { get; }

        private async Task PreCommitAsync(Thing thing)
        {
            Debug.Assert(thing.Id != null);

            thing.Words = thing.Words.Skip(1)
                .OrderBy(z => z.Language)
                .ThenBy(z => z.Text)
                .Insert(0, thing.Words[0]).ToList();

            // category
            if (thing.Categorys != null)
            {
                foreach (var category in thing.Categorys)
                {
                    if (this.categorys.Add(category)) this.SavedNewCategory?.Invoke(this, category);
                }
            }

            // language
            var languages = thing.Words.Where(z => z.Language != null).Select(z => z.Language).Distinct().ToArray();
            if (languages.Length > 0)
            {
                if (this.languages == null)
                {
                    await this.GroupLanguagesAsync();
                }
                Debug.Assert(this.languages != null);
                this.languages.AddRange(languages);
            }
        }

        public async Task UpdateAsync(Thing thing)
        {
            await this.PreCommitAsync(thing);
            await this.Collection.ReplaceOneAsync(
                new FilterDefinitionBuilder<Thing>().Eq(z => z.Id, thing.Id),
                thing, new UpdateOptions { IsUpsert = true });
        }

        public async Task<QueryResult<Thing>> FindAsync(FilterDefinition<Thing> filter, int count)
        {
            Debug.Assert(filter != null);
            Debug.Assert(count > 0);

            var items = await (await this.Collection.FindAsync(filter, new FindOptions<Thing, Thing>
            {
                Limit = count + 1
            })).ToListAsync();

            return items.Count == count + 1 ? new QueryResult<Thing>(true, items.Take(count)) : new QueryResult<Thing>(false, items);
        }

        public async Task<Thing> FindOneAsync(string thingId)
        {
            Debug.Assert(thingId != null);

            return (await (await this.Collection.FindAsync(new FilterDefinitionBuilder<Thing>().Eq(z => z.Id, thingId)))
                .ToListAsync()).FirstOrDefault();
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

        public async Task<string[]> GroupLanguagesAsync()
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

        public async Task<string[]> GroupCategorysAsync()
        {
            if (this.categorys == null)
            {
                PipelineDefinition<Thing, GroupResult> pipeline = new[]
                {
                    new BsonDocument
                    {
                        { "$unwind", "$Categorys" }
                    },
                    new BsonDocument
                    {
                        { "$group", new BsonDocument("_id", "$Categorys") }
                    }
                };
                var ret = await (await this.Collection.AggregateAsync(pipeline)).ToListAsync();
                this.categorys = new HashSet<string>(ret.Select(z => z.Id).Where(z => z != null));
            }

            return this.categorys.ToArray();
        }

        private class GroupResult
        {
            public string Id { get; set; }
        }
    }
}
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jasily.ComponentModel;
using JryDictionary.Models;
using MongoDB.Driver;

namespace JryDictionary.DbAccessors
{
    public class ThingSetAccessor
    {
        public ThingSetAccessor(IMongoDatabase db)
        {
            this.Collection = db.GetCollection<Thing>("Thing");
        }

        public IMongoCollection<Thing> Collection { get; }

        public async Task UpdateAsync(Thing thing)
        {
            Debug.Assert(thing.Id != null);

            thing.Words = thing.Words.Skip(1).OrderBy(z => z.Language).Insert(0, thing.Words[0]).ToList();

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
    }
}
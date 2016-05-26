using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JryDictionary.Models;
using MongoDB.Driver;

namespace JryDictionary.DbAccessors
{
    public class SettingSetAccessor
    {
        private const string CollectionName = "Settings";
        private readonly IMongoDatabase database;
        private readonly UpdateOptions updateOptions;

        public SettingSetAccessor(IMongoDatabase db)
        {
            this.database = db;
            this.updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
        }

        private static FilterDefinition<T> GetFilter<T>(T simple) where T : SettingEntity
            => Singleton.Instance<FilterDefinitionBuilder<T>>().Eq(z => z.Id, simple.Id);

        public async Task<T> GetSettingAsync<T>() where T : SettingEntity, new()
        {
            var simple = new T();
            var result = await (await this.database.GetCollection<T>(CollectionName).FindAsync(GetFilter(simple)))
                .ToListAsync();
            return result.Count > 0 ? result[0] : simple;
        }

        public Task SetSettingAsync<T>(T value) where T : SettingEntity, new()
        {
            Debug.Assert(value != null);
            return this.database.GetCollection<T>(CollectionName)
                .ReplaceOneAsync(GetFilter(value), value, this.updateOptions);
        }
    }
}
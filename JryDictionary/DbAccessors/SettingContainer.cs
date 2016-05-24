using System.Threading.Tasks;
using Jasily.Threading.Tasks;
using JryDictionary.Models;

namespace JryDictionary.DbAccessors
{
    public class SettingContainer<T> where T : SettingEntity, new()
    {
        private readonly SettingSetAccessor accessor;
        private readonly UniqueTask<T> value;

        public SettingContainer(SettingSetAccessor accessor)
        {
            this.accessor = accessor;
            this.value = new UniqueTask<T>(accessor.GetSettingAsync<T>);
        }

        public Task<T> ValueAsync() => this.value.Run();

        public async Task UpdateAsync() => await this.accessor.SetSettingAsync(await this.value.Run());
    }
}
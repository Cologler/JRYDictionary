using JetBrains.Annotations;

namespace JryDictionary.Models
{
    public abstract class SettingEntity
    {
        [NotNull]
        public string Id { get; set; }
    }
}
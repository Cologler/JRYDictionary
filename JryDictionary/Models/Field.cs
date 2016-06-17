using JetBrains.Annotations;

namespace JryDictionary.Models
{
    public sealed class Field
    {
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string TargetId { get; set; }
    }
}
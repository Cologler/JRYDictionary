using System.Collections.Generic;
using JetBrains.Annotations;

namespace JryDictionary.Models
{
    public sealed class Flags : SettingEntity
    {
        public Flags()
        {
            this.Id = "Flags";
            this.Groups = new List<string>();
        }

        [NotNull]
        public List<string> Groups { get; set; }

        [NotNull]
        public List<string> Languages { get; set; }
    }
}
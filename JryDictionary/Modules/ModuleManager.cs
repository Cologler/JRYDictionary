using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using Jasily;
using JryDictionary.Modules.Builders;
using JryDictionary.Modules.Copyer;

namespace JryDictionary.Modules
{
    public class ModuleManager
    {
        private CompositionContainer compositionContainer;
#pragma warning disable 649
        [ImportMany]
        private IEnumerable<IWordBuilder> builders;

        [ImportMany]
        private IEnumerable<IWordCopyer> copyers;
#pragma warning restore 649

        public void Initialize()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(this.GetType().Assembly));
            this.compositionContainer = new CompositionContainer(catalog);
            this.compositionContainer.ComposeParts(this);
            Debug.Assert(this.builders != null);
            this.Builders.AddRange(this.builders.OrderBy(z => z.AsOrderable().GetOrderCode()));
            this.Copyers.AddRange(this.copyers.OrderBy(z => z.AsOrderable().GetOrderCode()));
        }

        public List<IWordBuilder> Builders { get; } = new List<IWordBuilder>();

        public List<IWordCopyer> Copyers { get; } = new List<IWordCopyer>();
    }
}
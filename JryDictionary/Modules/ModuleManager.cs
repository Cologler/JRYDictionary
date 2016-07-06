using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using Jasily;
using JryDictionary.Models;
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
            this.Assert();
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

        [Conditional("DEBUG")]
        public void Assert() => new WordCopyerTester().Test();

        private class WordCopyerTester : WordCopyer
        {
            public void Test()
            {
                this.Test("Ab_c", "Ab", "c");
                this.Test("ABC", "ABC");
                this.Test("ABC_D", "ABC", "D");
                this.Test("AbCd", "Ab", "Cd");
            }

            private void Test(string value, params string[] array)
            {
                var ret = SplitTextAsWords(value);
                if (!ret.SequenceEqual(array))
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Debug.Assert(false);
                }
            }

            public override void Copy(Thing thing, Word word) { }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace JryDictionary.Models.DocPlugins
{
    public interface IDocPlugin : IDisposable
    {
        IEnumerable<Inline> ParseLine(string[] line);
    }
}
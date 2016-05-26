using System.Collections.Generic;

namespace JryDictionary.Controls.DbAccessors
{
    public class QueryResult<T>
    {
        public bool HasNext { get; }

        public IEnumerable<T> Items { get; }

        public QueryResult(bool hasNext, IEnumerable<T> items)
        {
            this.HasNext = hasNext;
            this.Items = items;
        }
    }
}
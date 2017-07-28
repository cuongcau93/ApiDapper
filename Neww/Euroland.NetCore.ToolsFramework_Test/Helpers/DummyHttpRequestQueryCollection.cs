using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Test.Helpers
{
    public class DummyHttpRequestQueryCollection : IQueryCollection
    {
        private readonly IDictionary<string, StringValues> _queries;
        public DummyHttpRequestQueryCollection(IDictionary<string, StringValues> queries)
        {
            _queries = queries;
        }
        public StringValues this[string key] => _queries.ContainsKey(key) ? _queries[key] : StringValues.Empty;

        public int Count => _queries.Count;

        public ICollection<string> Keys => _queries.Keys;

        public bool ContainsKey(string key)
        {
            return _queries.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            return _queries.GetEnumerator();
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            if (_queries.ContainsKey(key))
            {
                value = _queries[key];
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queries.GetEnumerator();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// An implementation of <see cref="IResourceNameCache"/>
    /// </summary>
    public class JsonResourceNameCache : IResourceNameCache
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, IList<string>> _cache = new System.Collections.Concurrent.ConcurrentDictionary<string, IList<string>>();

        /// <inheritdoc />
        public IList<string> GetOrAdd(string name, Func<string, IList<string>> valueFactory)
        {
            return _cache.GetOrAdd(name, valueFactory(name));
        }
    }
}

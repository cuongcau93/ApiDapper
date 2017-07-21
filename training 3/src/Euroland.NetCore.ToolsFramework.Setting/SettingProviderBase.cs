using Euroland.NetCore.ToolsFramework.Setting.Abstractions;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public abstract class SettingProviderBase : ISettingProvider
    {
        private SettingReloadToken _reloadToken = new SettingReloadToken();
        public SettingProviderBase()
        {
            //Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Data = new System.Collections.Concurrent.ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        protected IDictionary<string, string> Data { get; set; }

        public bool Updatable { get; set; } = false;

        public bool IsLoaded { get; internal set; } = false;

        public abstract void Load();

        public void Set(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Data[name] = value;
        }

        public bool TryGet(string name, out string value)
        {
            return Data.TryGetValue(name, out value);
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> otherChildNames, string parentPath)
        {
            parentPath = parentPath ?? string.Empty;

            return Data.Keys
                 .Where(key => key.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase))
                 .Select(key => SettingFlatterningKeyUtil.GetClosestSettingName(key, parentPath))
                 .Concat(otherChildNames);
        }

        /// <summary>
        /// Gets the <see cref="IChangeToken"/> so that outside can listen to this provider is reloaded
        /// </summary>
        /// <returns></returns>
        public IChangeToken GetReloadToken()
        {
            return _reloadToken;
        }

        /// <summary>
        /// Triggers the reload token and creates new one
        /// </summary>
        protected void OnReload()
        {
            var prevChangeToken = System.Threading.Interlocked.Exchange<SettingReloadToken>(ref _reloadToken, new SettingReloadToken());
            prevChangeToken.OnReload();
        }

        protected abstract string SerializeData();

        public string WriteAsString()
        {
            return this.SerializeData();
        }

        public void WriteAsString(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8))
            {
                writer.Write(this.SerializeData());
            }
        }

        public Task<string> WriteAsStringAsync()
        {
            return Task.FromResult<string>(this.SerializeData());
        }

        public Task WriteAsStringAsync(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8))
            {
                return writer.WriteAsync(this.SerializeData());
            }
        }
    }
}

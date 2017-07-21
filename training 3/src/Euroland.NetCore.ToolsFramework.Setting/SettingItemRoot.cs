using Euroland.NetCore.ToolsFramework.Setting.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// An implmentation of <see cref="ISettingItemRoot"/>
    /// </summary>
    public class SettingItemRoot : ISettingItemRoot
    {
        private IEnumerable<ISettingProvider> _settingProviders;
        /// <summary>
        /// Create a new <see cref="SettingItemRoot"/> with a list of <see cref="ISettingProvider"/>
        /// </summary>
        /// <param name="settingProviders">The list of <see cref="ISettingProvider"/> for this setting</param>
        public SettingItemRoot(IEnumerable<ISettingProvider> settingProviders)
        {
            if (settingProviders == null)
                throw new ArgumentNullException(nameof(settingProviders));

            // TODO: not always 
            _settingProviders = settingProviders;
            foreach (var provider in _settingProviders)
            {
                provider.Load();
            }
        }

        public string this[string name]
        {
            get
            {
                foreach (var provider in _settingProviders)
                {
                    string value = null;
                    if (provider.TryGet(name, out value))
                    {
                        return value;
                    }
                }

                return null;
            }
            set
            {
                if (!_settingProviders.Any())
                    throw new InvalidOperationException("Not found any registered ISettingProvider");

                foreach (var provider in _settingProviders)
                {
                    provider.Set(name, value);
                }
            }
        }

        public IEnumerable<ISettingProvider> Providers => _settingProviders;

        public bool Updatable { get; set; }

        public ISettingItem GetChild(string name)
        {
            return new SettingItem(this, name);
        }

        public IEnumerable<ISettingItem> GetChildren()
        {
            return GetImmediateChildren(null);
        }

        //TODO: reload on file changes

        public void Reload()
        {
            foreach (var provider in _settingProviders)
            {
                provider.Load();
            }
        }

        internal IEnumerable<ISettingItem> GetImmediateChildren(string path)
        {
            return _settingProviders
                .Aggregate(
                    Enumerable.Empty<string>(),
                    (seed, provider) => provider.GetChildKeys(seed, path)
                )
                .Distinct()
                .Select(key => GetChild(path == null ? key : SettingFlatterningKeyUtil.Combine(path, key)));
        }
    }
}

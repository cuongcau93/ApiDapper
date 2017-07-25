using Euroland.NetCore.ToolsFramework.Setting.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// An implementation of <see cref="ISettingManager"/>
    /// </summary>
    public class SettingManager : ISettingManager
    {
        /// <summary>
        /// List of <see cref="ISettingProviderFactory"/>
        /// </summary>
        private List<ISettingProviderFactory> _settingProviderFactories = new List<ISettingProviderFactory>();
        private List<IRequestSettingFinder> _requestSettingProviders = new List<IRequestSettingFinder>();

        public IEnumerable<ISettingProviderFactory> SettingProviderFactories => _settingProviderFactories.ToList();

        public IEnumerable<IRequestSettingFinder> RequestSettingProvider => _requestSettingProviders.ToList();

        /// <summary>
        /// Add a new <see cref="ISettingProviderFactory"/>. The later added one will merge and replace
        /// the keys of earlier
        /// </summary>
        /// <param name="factory">The <see cref="ISettingProviderFactory"/></param>
        /// <returns>The <see cref="ISettingManager"/> as a chain for reusing factory</returns>
        public ISettingManager Accept(ISettingProviderFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _settingProviderFactories.Add(factory);
            return this;
        }

        /// <summary>
        /// Add a new <see cref="ISettingProviderFactory"/>
        /// </summary>
        /// <param name="factory">The <see cref="ISettingProviderFactory"/></param>
        /// <param name="request"></param>
        /// <returns>The <see cref="ISettingManager"/> as a chain for reusing factory</returns>
        public ISettingManager Accept(ISettingProviderFactory factory, IRequestSettingFinder request)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if(request == null)
                throw new ArgumentNullException(nameof(request));


            _settingProviderFactories.Add(factory);
            _requestSettingProviders.Add(request);
            return this;
        }

        /// <summary>
        /// Creates the root <see cref="ISettingItemRoot"/> of application setting from 
        /// the set of setting provider factories registered in <see cref="SettingProviderFactories"/>
        /// </summary>
        /// <returns>An <see cref="ISettingItemRoot"/></returns>
        public ISettingItemRoot Create()
        {
            List<ISettingProvider> settingProviders = new List<ISettingProvider>();

            _settingProviderFactories.Reverse();
            // Reverse the list to prioritize overwriting the later added SettingProvider
            foreach (var provider in _settingProviderFactories)
            {
                settingProviders.Add(provider.Create(this));
            }

            var root = new SettingItemRoot(settingProviders);

            // TODO: add MemoryCache by injecting IMemoryCache 
            // (https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory#caching-basics)
            // to cache up setting. And listen reloading cache as well when
            // the setting file has been changed using root.GetSettingChangeToken();

            return root;
        }
    }
}

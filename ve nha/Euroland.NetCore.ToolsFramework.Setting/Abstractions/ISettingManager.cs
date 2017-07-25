using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting.Abstractions
{
    /// <summary>
    /// Represents a type used to create a <see cref="ISettingItemRoot"/>
    /// </summary>
    public interface ISettingManager
    {
        /// <summary>
        /// Gets the factories that used to obtain <see cref="ISettingProvider"/>s
        /// </summary>
        IEnumerable<ISettingProviderFactory> SettingProviderFactories { get; }

        /// <summary>
        /// Add a new <see cref="ISettingProviderFactory"/>
        /// </summary>
        /// <param name="factory">The <see cref="ISettingProviderFactory"/></param>
        /// <returns>The <see cref="ISettingManager"/> as a chain for reusing factory</returns>
        ISettingManager Accept(ISettingProviderFactory factory);

        /// <summary>
        /// Creates the root <see cref="ISettingItemRoot"/> of application setting from 
        /// the set of setting provider factories registered in <see cref="SettingProviderFactories"/>
        /// </summary>
        /// <returns>An <see cref="ISettingItemRoot"/></returns>
        ISettingItemRoot Create();
    }
}

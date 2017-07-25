using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting.Abstractions
{
    /// <summary>
    /// Represents the root item of an <see cref="ISetting"/> hierarchy
    /// </summary>
    public interface ISettingItemRoot: ISetting
    {
        /// <summary>
        /// Gets or sets value to indicate that this setting is updatable value
        /// </summary>
        bool Updatable { get; set; }

        /// <summary>
        /// Gets the <see cref="ISettingProvider"/>s for this setting
        /// </summary>
        IEnumerable<ISettingProvider> Providers { get; }
    }
}

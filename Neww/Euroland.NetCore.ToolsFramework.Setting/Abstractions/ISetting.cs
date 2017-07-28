using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting.Abstractions
{
    /// <summary>
    /// Represents a set of key/value setting
    /// </summary>
    public interface ISetting
    {
        /// <summary>
        /// Gets or sets a setting value
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <returns>The setting value</returns>
        string this[string name] { get;set; }

        /// <summary>
        /// Gets a child setting with the specified name
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <returns>The <see cref="ISettingItem"/> </returns>
        /// <remarks>
        ///     This method will never return <c>null</c>. If no matching child setting is found,
        ///     an empty <see cref="ISettingItem"/> will be returned.
        /// </remarks>
        ISettingItem GetChild(string name);

        /// <summary>
        /// Gets the immediate children of this setting
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISettingItem> GetChildren();
    }
}

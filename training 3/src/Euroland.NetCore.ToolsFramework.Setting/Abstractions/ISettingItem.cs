using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting.Abstractions
{
    /// <summary>
    /// Represents an item of setting values
    /// </summary>
    public interface ISettingItem: ISetting
    {
        /// <summary>
        /// Gets the name of this item occupies in its parent 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full path to this item within the <see cref="ISetting"/>
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets or sets the item value
        /// </summary>
        string Value { get; set; }
    }
}

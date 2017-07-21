using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// Types of setting resource should hangled by system
    /// </summary>
    public enum SettingResourceType
    {
        /// <summary>
        /// Handles JSON format resource only
        /// </summary>
        Json,
        /// <summary>
        /// Handles XML format resource only
        /// </summary>
        Xml,
        /// <summary>
        /// Detects type of format of resource automatically 
        /// </summary>
        Json_Xml
    }
}

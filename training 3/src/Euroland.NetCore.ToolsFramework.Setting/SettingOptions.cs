using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// Class contains the options for <see cref="RequestSettingMiddleware"/>
    /// </summary>
    public class SettingOptions
    {
        /// <summary>
        /// Gets or sets the name of application.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets absolute path of directory where where the general setting files are located.
        /// </summary>
        /// <example>
        /// <c>1. wwwroot/Tools/Config</c>
        /// </example>
        public string GeneralSettingRootPath { get; set; }

        /// <summary>
        /// Gets or sets absolute path of directory where where the setting files of a specified application are located.
        /// For Euroland's tools, have two root directory for a application.
        /// </summary>
        /// <example>
        /// <c>2. wwwroot/Tools/{APPLICATION}/Config</c>
        /// </example>
        public string ToolSettingRootPath { get; set; }

        /// <summary>
        /// Gets value to indicate that whether the setting should be reloaded if the file changes.
        /// </summary>
        public bool ReloadOnChange { get; set; } = true;

        /// <summary>
        /// Gets or sets the format of setting resource
        /// </summary>
        public SettingResourceType ResourceType { get; set; } = SettingResourceType.Json;
    }
}

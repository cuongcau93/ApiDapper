using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// Details about the setting resource obtained from <see cref="Abstractions.IRequestSettingFinder"/>
    /// </summary>
    public class SettingResourceResult
    {
        /// <summary>
        /// Gets or sets the company code
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// Gets or sets the version setting of company
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the request language
        /// </summary>
        public string Language { get; set; }
    }
}

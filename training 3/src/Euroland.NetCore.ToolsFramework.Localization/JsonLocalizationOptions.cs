using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Provides programmatic configuration for JSON localization
    /// </summary>
    public class JsonLocalizationOptions
    {
        /// <summary>
        /// Gets or sets the relative path under application root where json-translation files are located
        /// </summary>
        public string ResourcePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the relative path where the XML file "LanguageMap.xml" or JSON file "LanguageMap.json" are located
        /// </summary>
        public string LanguageToCultureMappingPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the <see cref="ILanguageToCultureProvider"/>. This will override the <see cref="LanguageToCultureMappingPath"/>
        /// </summary>
        public ILanguageToCultureProvider LanguageToCultureProvider { get; set; } = null;
    }
}

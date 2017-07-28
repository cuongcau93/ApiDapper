using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    /// <summary>
    /// Class contains optionals for <see cref="TranslationGenerator"/> and <see cref="TranslationService"/>
    /// </summary>
    public class TranslationOptions
    {
        /// <summary>
        /// Gets or sets the path under application root where json-translation files and <see cref="FileName"/> are located.
        /// Default is "Translations" located under the root of application
        /// </summary>
        public string ResourcePath { get; set; } = "Translations";

        /// <summary>
        /// Gets or sets the file name of xml translation. Default is "Translation.xml"
        /// </summary>
        public string FileName { get; set; } = "Translations.xml";

        /// <summary>
        /// Gets or sets the connection string of translation database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the username in order to be able to run generating json file
        /// </summary>
        public string GeneratorUserName { get; set; }

        /// <summary>
        /// Gets or sets the password of user in order to be able to run generating json file
        /// </summary>
        public string GeneratorPassword { get; set; }
    }
}

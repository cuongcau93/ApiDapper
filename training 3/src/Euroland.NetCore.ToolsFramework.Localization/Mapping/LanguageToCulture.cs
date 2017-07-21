using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Class to map supported Euroland's language to real cultures which are supported by .NET.
    /// Euroland system is now supporting only language formed by two letter of abbriviation of language
    /// which are not supported by .NET Framework natively.
    /// </summary>
    public class LanguageToCulture
    {
        /// <summary>
        /// Creates a new <see cref="LanguageToCulture"/>
        /// </summary>
        public LanguageToCulture() { }

        /// <summary>
        /// Creates a new <see cref="LanguageToCulture"/>
        /// </summary>
        /// <param name="twoLetterAbbriviationLanguage">Two letter of language: <c>EN, VI, TW</c></param>
        public LanguageToCulture(string twoLetterAbbriviationLanguage)
        {
            if (string.IsNullOrEmpty(twoLetterAbbriviationLanguage) || string.IsNullOrWhiteSpace(twoLetterAbbriviationLanguage))
                throw new ArgumentNullException(nameof(twoLetterAbbriviationLanguage));

            TwoLetterOfLanguage = twoLetterAbbriviationLanguage;
        }

        /// <summary>
        /// Gets or sets abbriviation of language supported by euroland
        /// </summary>
        public string TwoLetterOfLanguage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets list of <see cref="System.Globalization.CultureInfo"/>
        /// </summary>
        public List<System.Globalization.CultureInfo> SupportedCultures { get; set; } = new List<System.Globalization.CultureInfo>(0);
    }
}

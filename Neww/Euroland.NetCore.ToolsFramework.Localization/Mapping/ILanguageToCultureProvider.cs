using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Provides language-culture mapping for an application as well as list of <see cref="System.Globalization.CultureInfo"/> 
    /// that supported by euroland system
    /// </summary>
    public interface ILanguageToCultureProvider
    {
        /// <summary>
        /// Gets the state of loading resource
        /// </summary>
        bool Loaded { get; }

        /// <summary>
        /// Gets two-letter language of all supported languages
        /// </summary>
        IEnumerable<string> SupportedLanguages { get; }
        
        /// <summary>
        /// Reads and loads the language-culture mapping resource 
        /// </summary>
        void Load(bool reload = false);

        /// <summary>
        /// Gets all supported <see cref="System.Globalization.CultureInfo"/>
        /// </summary>
        IEnumerable<System.Globalization.CultureInfo> AllSupportedCultures { get; }

        /// <summary>
        /// Gets a <see cref="LanguageToCulture"/>
        /// </summary>
        /// <param name="cultureCode">The code (name) of a specific culture</param>
        /// <returns>A <see cref="LanguageToCulture"/> or <c>Null</c> if not found</returns>
        LanguageToCulture GetLanguage(string cultureCode);

        /// <summary>
        /// Get a <see cref="LanguageToCulture"/>
        /// </summary>
        /// <param name="cultureInfo">The <see cref="System.Globalization.CultureInfo"/></param>
        /// <returns>A <see cref="LanguageToCulture"/> or <c>Null</c> if not found</returns>
        LanguageToCulture GetLanguage(System.Globalization.CultureInfo cultureInfo);

        /// <summary>
        /// Gets the <see cref="System.Globalization.CultureInfo"/> which is bound to supported language
        /// </summary>
        /// <param name="twoLetterLanguage">Two-letter of language</param>
        /// <returns>The list of <see cref="System.Globalization.CultureInfo"/></returns>
        IEnumerable<System.Globalization.CultureInfo> GetCultures(string twoLetterLanguage);
    }
}

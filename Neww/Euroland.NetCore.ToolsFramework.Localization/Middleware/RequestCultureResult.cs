using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Class contains the detail request culture optained from <see cref="IRequestCultureFinder"/>
    /// </summary>
    public class RequestCultureResult
    {
        /// <summary>
        /// Creates a new <see cref="RequestCultureResult"/> object that has its <see cref="Culture"/> and
        /// <see cref="UICulture"/> properties set to the same value
        /// </summary>
        /// <param name="culture">The culture to be used for formatting, text, language</param>
        public RequestCultureResult(CultureInfo culture)
            : this(culture, culture)
        {

        }

        /// <summary>
        /// Creates a new <see cref="RequestCultureResult"/> object with provided <see cref="Culture"/> and <see cref="UICulture"/>
        /// </summary>
        /// <param name="culture">The culture for formatting</param>
        /// <param name="uiCulture">The culture for text</param>
        public RequestCultureResult(CultureInfo culture, CultureInfo uiCulture)
        {
            Culture = culture;
            UICulture = uiCulture;
        }

        /// <summary>
        /// Gets the culture to be used for formatting
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets the culture to be used for text, i.e. language
        /// </summary>
        public CultureInfo UICulture { get; }
    }
}

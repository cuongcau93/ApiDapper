using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.Concurrent;
using Microsoft.Extensions.FileProviders;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// An <see cref="IStringLocalizer"/> that uses <see cref="Newtonsoft.Json.JsonTextReader"/>
    /// to provide localized string for a specific <see cref="CultureInfo"/>
    /// </summary>
    public class JsonManagerWithCultureStringLocalizer: JsonManagerStringLocalizer
    {
        private readonly CultureInfo _culture;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="logger"></param>
        /// <param name="culture"></param>
        /// <param name="language2CultureProvider"></param>
        /// <param name="resourceNameCache"></param>
        public JsonManagerWithCultureStringLocalizer(
            string baseName,
            IFileProvider resourceFileProvider,
            ILogger logger, 
            CultureInfo culture, 
            ILanguageToCultureProvider language2CultureProvider)
            : base(baseName, resourceFileProvider, logger, language2CultureProvider)
        {
            _culture = culture;
        }

        public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.GetAllStrings(includeParentCultures, _culture);
        }

        public override LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var value = this.GetLocalizationStringSafely(name, _culture);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        /// <inheritdoc />
        public override LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var format = this.GetLocalizationStringSafely(name, _culture);
                var value = string.Format(format ?? name, arguments);

                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }
    }
}

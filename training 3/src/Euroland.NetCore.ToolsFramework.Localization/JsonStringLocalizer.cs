using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Localization;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Provides strings for application and injectable for dependency injection
    /// </summary>
    public class JsonStringLocalizer<TResource> : IStringLocalizer<TResource>
    {
        /// <summary>
        /// Localizer factory to create string
        /// </summary>
        private readonly IStringLocalizer _localizer;

        /// <summary>
        /// Creates new <see cref="JsonStringLocalizer{TResource}"/>
        /// </summary>
        /// <param name="localizerFactory">
        /// The <see cref="IStringLocalizerFactory"/> to create <see cref="IStringLocalizer"/>.
        /// This should obtains via dependency injection
        /// </param>
        public JsonStringLocalizer(IStringLocalizerFactory localizerFactory)
        {
            if (localizerFactory == null)
                throw new ArgumentNullException(nameof(localizerFactory));

            _localizer = localizerFactory.Create(typeof(TResource));
        }

        /// <inheritdoc />
        public LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                return _localizer[name];
            }
        }

        /// <inheritdoc />
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                return _localizer[name, arguments];
            }
        }

        /// <inheritdoc />
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }

        /// <inheritdoc />
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return _localizer.WithCulture(culture);
        }
    }
}

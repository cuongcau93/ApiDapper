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
    /// to provide localized string
    /// </summary>
    /// <remarks>This type is thread-safe.</remarks>
    public class JsonManagerStringLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, Lazy<JObject>> _stringLocalizerCache = new ConcurrentDictionary<string, Lazy<JObject>>();
        private readonly string _baseName;
        private readonly ILogger _logger;
        private readonly ILanguageToCultureProvider _language2CultureProvider;
        private readonly IFileProvider _fileProvider;

        /// <summary>
        /// Creates a new <see cref="JsonManagerStringLocalizer"/>
        /// </summary>
        /// <param name="baseName">The base name of the embedded resource that contains the strings.</param>
        /// <param name="resourceFileProvider"></param>
        /// <param name="logger">The <see cref="ILogger"/></param>
        /// <param name="language2CultureProvider">The <see cref="ILanguageToCultureProvider"/></param>
        public JsonManagerStringLocalizer(
            string baseName,
            IFileProvider resourceFileProvider,
            ILogger logger,
            ILanguageToCultureProvider language2CultureProvider)
        {
            if (baseName == null)
                throw new ArgumentNullException(nameof(baseName));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (language2CultureProvider == null)
                throw new ArgumentNullException(nameof(language2CultureProvider));

            if (resourceFileProvider == null)
                throw new ArgumentNullException(nameof(resourceFileProvider));

            this._baseName = baseName;
            this._logger = logger;
            this._language2CultureProvider = language2CultureProvider;
            this._fileProvider = resourceFileProvider;
        }

        /// <inheritdoc />
        public virtual LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var value = this.GetLocalizationStringSafely(name, null);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        /// <inheritdoc />
        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if(name == null)
                    throw new ArgumentNullException(nameof(name));

                var format = this.GetLocalizationStringSafely(name, null);
                var value = string.Format(format ?? name, arguments);

                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);
        }

        /// <inheritdoc />
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            if (culture == null)
            {
                return new JsonManagerStringLocalizer(
                    _baseName, 
                    _fileProvider,
                    _logger, 
                    _language2CultureProvider);
            }
            else
            {
                return new JsonManagerWithCultureStringLocalizer(
                    _baseName, 
                    _fileProvider,
                    _logger, 
                    culture, 
                    _language2CultureProvider);
            }
        }

        /// <summary>
        /// Gets a resource string from Json file and returns <c>null</c>
        /// instead of throwing exception if a match isn't found.
        /// </summary>
        /// <param name="name">The name of the string resource</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to get the string for.</param>
        /// <returns>The resource string, or <c>null</c> if none was found.</returns>
        protected string GetLocalizationStringSafely(string name, CultureInfo culture)
        {
            culture = culture ?? CultureInfo.CurrentUICulture;

            var jObject = this.GetJsonResource(culture);

            if (jObject != null)
            {
                JToken token = null;
                if (jObject.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out token))
                {
                    return token.ToString();
                }
            }

            return null;
        }

        protected IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            var jObject = this.GetJsonResource(culture) ?? new JObject();

            foreach (var prop in jObject.Properties())
            {
                yield return new LocalizedString(prop.Name, prop.Value.ToString());
            }
        }

        private JObject GetJsonResource(CultureInfo culture)
        {
            var language2Culture = _language2CultureProvider.GetLanguage(culture);

            if(language2Culture == null)
            {
                _logger.LogError($"LanguageToCulture is not containing a definition for culture: {culture.Name}");
                return null;
            }

            string resourceName = language2Culture.TwoLetterOfLanguage;

            // EN.json, VI.json, FR.json, ...
            var resourceFileName = $"{resourceName}.json";
            //var resourceFullPath = System.IO.Path.Combine(_resourcePath, resourceFileName);
            //var defaultResourceFileName = "EN.json"; // Default to English
            var resourceFileInfo = _fileProvider.GetFileInfo(resourceFileName);

            // If specified resource is not found, let get from default resource
            //if (!resourceFileInfo.Exists)
            //{
            //    resourceFileName = defaultResourceFileName;
            //    resourceFileInfo = _fileProvider.GetFileInfo(defaultResourceFileName);
            //}

            if (!resourceFileInfo.Exists)
            {
                _logger.LogError($"Resource not found for {resourceFileInfo.PhysicalPath}");
                return null;
            }

            // TODO: Register *.json file watcher by using IFileProvider.Watch() to notify to application
            // should reload the JSON file if has any change of file detected

            var lazyJObject = _stringLocalizerCache.GetOrAdd(resourceFileName, this.GetLazyJsonResource(resourceFileInfo));
            
            return lazyJObject.Value;
        }

        private Lazy<JObject> GetLazyJsonResource(IFileInfo resourceFileInfo)
        {
            return new Lazy<JObject>(() =>
            {
                try
                {
                    using (var resourceStream = resourceFileInfo.CreateReadStream())
                    using (var jsonReader = new Newtonsoft.Json.JsonTextReader(new System.IO.StreamReader(resourceStream)))
                    {
                        jsonReader.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;

                        return JObject.Load(jsonReader);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occurred while reading JSON from resource file {resourceFileInfo.PhysicalPath}: {ex}");

                    return null;
                }
            }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }
    }
}

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// An implementation of <see cref="ILanguageToCultureProvider"/>
    /// </summary>
    public class JsonLanguageToCultureProvider : ILanguageToCultureProvider
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, LanguageToCulture> _supportedLanguages;
        private readonly IFileInfo _languageToCultureFile;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private bool _loaded = false;

        /// <summary>
        /// Creates a new <see cref="JsonLanguageToCultureProvider"/>
        /// </summary>
        public JsonLanguageToCultureProvider(IFileInfo languageToCultureFile, Microsoft.Extensions.Logging.ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (languageToCultureFile == null)
                throw new ArgumentNullException(nameof(languageToCultureFile));

            if (!languageToCultureFile.Exists)
                throw new FileNotFoundException(string.Format("LanguageToCulture resource {0} not found.", languageToCultureFile.Name), languageToCultureFile.PhysicalPath);

            _languageToCultureFile = languageToCultureFile;
            _supportedLanguages = new System.Collections.Concurrent.ConcurrentDictionary<string, LanguageToCulture>(StringComparer.OrdinalIgnoreCase);
            _logger = logger;
        }

        /// <inheritdoc />
        public IEnumerable<string> SupportedLanguages => _supportedLanguages.Keys.ToArray();

        /// <inheritdoc />
        public IEnumerable<CultureInfo> AllSupportedCultures => GetAllCultureInfo();

        public bool Loaded => _loaded;

        /// <inheritdoc />
        public IEnumerable<CultureInfo> GetCultures(string twoLetterLanguage)
        {
            if (string.IsNullOrWhiteSpace(twoLetterLanguage))
                return null;
            LanguageToCulture lang = null;

            this._supportedLanguages.TryGetValue(twoLetterLanguage, out lang);

            return lang?.SupportedCultures;
        }

        /// <inheritdoc />
        public LanguageToCulture GetLanguage(string cultureCode)
        {
            foreach (var lang in _supportedLanguages.Keys)
            {
                if (_supportedLanguages[lang].SupportedCultures.Any(culture => culture.Name.ToLower() == cultureCode.ToLower()))
                    return _supportedLanguages[lang];
            }
            return null;
        }

        /// <inheritdoc />
        public LanguageToCulture GetLanguage(CultureInfo cultureInfo)
        {
            return this.GetLanguage(cultureInfo.Name);
        }

        public void Load(bool reload = false)
        {
            if (!reload && _loaded)
                return;

            if (reload)
            {
                _supportedLanguages.Clear();
            }

            var jsonContentStream = _languageToCultureFile.CreateReadStream();
            try
            {
                using (jsonContentStream)
                using (var jsonReader = new JsonTextReader(new StreamReader(jsonContentStream)))
                {
                    jsonReader.DateParseHandling = DateParseHandling.None;

                    var jsonArray = Newtonsoft.Json.Linq.JArray.Load(jsonReader);
                    this.VisitArray(jsonArray);
                }
                _loaded = true;
            }
            catch (Exception ex)
            {
                if (ex is Newtonsoft.Json.JsonReaderException)
                {
                    throw JsonFormatExceptionUtils.GetException(ex as Newtonsoft.Json.JsonReaderException, jsonContentStream);
                }
                else
                {
                    _logger.LogError($"[LanguageToCulture] An error occurred while reading resource file. Exception: {ex}");
                    throw ex;
                }
            }
        }

        private void VisitArray(Newtonsoft.Json.Linq.JArray jArray)
        {
            System.Text.StringBuilder error = new System.Text.StringBuilder();
            foreach (var obj in jArray)
            {
                if(obj.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                {
                    Newtonsoft.Json.Linq.JValue nameToken = obj.Value<Newtonsoft.Json.Linq.JValue>("name");
                    Newtonsoft.Json.Linq.JArray nativeCulturesToken = obj.Value<Newtonsoft.Json.Linq.JArray>("nativeCultures");
                    if(nameToken != null && nativeCulturesToken != null)
                    {
                        var lang2Culture = new LanguageToCulture() { TwoLetterOfLanguage=nameToken.ToString().Trim() };
                        lang2Culture.SupportedCultures = new List<CultureInfo>();
                        foreach (var nativeCulture in nativeCulturesToken)
                        {
                            var cultureName = nativeCulture.ToString();
                            if(!lang2Culture.SupportedCultures.Any(c=>string.Equals(cultureName, c.Name, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                try
                                {
                                    if (!lang2Culture.SupportedCultures.Any(c => string.Equals(c, cultureName)))
                                    {
                                        var cultureInfo = new System.Globalization.CultureInfo(cultureName);
                                        lang2Culture.SupportedCultures.Add(cultureInfo);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error.AppendLine($"[LanguageToCulture] An error occurred while trying to create culture {cultureName}. Exception: {ex}");
                                }
                            }

                        }
                        _supportedLanguages.TryAdd(lang2Culture.TwoLetterOfLanguage, lang2Culture);
                    }
                }
            }

            if(error.Length > 0)
            {
                _logger.LogError(error.ToString());
            }
        }

        
        private IEnumerable<CultureInfo> GetAllCultureInfo()
        {
            foreach (var key in _supportedLanguages.Keys)
            {
                foreach (var culture in _supportedLanguages[key].SupportedCultures)
                {
                    yield return culture;
                }
            }
        }
    }
}

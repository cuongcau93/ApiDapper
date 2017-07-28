using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    /// <summary>
    /// Class do generating content from Translation.xml to resource file "Translation.resx"
    /// </summary>
    public class TranslationGenerator
    {
        private readonly TranslationOptions _options;
        private IFileProvider _translationFileProvider;
        private string _rootDirectory;
        private readonly Dictionary<int, string> _phrases = new Dictionary<int, string>();
        private readonly ITranslationService _translationService;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _env;

        /// <summary>
        /// Instantiate an instance of <see cref="TranslationGenerator"/>
        /// </summary>
        /// <param name="translationPath">The path to Translation.xml. Path must be physical file system</param>
        public TranslationGenerator(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ITranslationService translationService, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory, IOptions<TranslationOptions> options)
        {
            if (env == null)
                throw new ArgumentNullException(nameof(env));

            if (translationService == null)
                throw new ArgumentNullException(nameof(translationService));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _env = env;
            _logger = loggerFactory.CreateLogger("TranslationGenerator");
            _options = options.Value;
            _translationService = translationService;
            EnsureFileProvider();
        }

        /// <summary>
        /// Instantiate an instance of <see cref="TranslationGenerator"/>
        /// </summary>
        /// <param name="translationStream">Stream of translation</param>
        public TranslationGenerator(Stream translationStream)
        {
            if (translationStream == null)
                throw new ArgumentNullException("translationPath");
            if (!translationStream.CanRead || !translationStream.CanSeek || translationStream.Length == 0)
                throw new InvalidOperationException("Stream must not be empty and be readable");

        }

        /// <summary>
        /// Starts converting and saving translation to *.json file.
        /// </summary>
        /// <param name="language">
        ///     The specified two-letter language (e.g EN, VI). If language is specified, 
        ///     only translation phrases of that language going to be generated to JSOn content.
        ///     Default is null means all languages
        /// </param>
        /// <returns>True if generation action is successfully. Otherwise False</returns>
        public bool Generate(string language = null)
        {
            bool generateStatus = false;
            try
            {
                this.ReadXmlTranslation();
                var translationList = this.GetTranslationFromDb();
                if (translationList.Count() <= 0)
                {
                    generateStatus = false;
                    _logger.Log<string>(LogLevel.Information, 0, "No data available to generate translation", null, null);
                }

                var fileNames = translationList.First().TranslationMap.Keys;
                if (!string.IsNullOrWhiteSpace(language))
                {
                    var isSupportedLanguage = fileNames.Any(f => string.Equals(f.Trim(), language.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (isSupportedLanguage)
                    {
                        WriteData2JsonFormat(translationList, language);
                    }
                }
                else
                {
                    foreach (var lang in fileNames)
                    {
                        WriteData2JsonFormat(translationList, lang);
                    }
                }
                generateStatus = true;
            }
            /*catch (JsonException ex)
            {
                _logger.Log<string>(Microsoft.Extensions.Logging.LogLevel.Error, 0, ex.Message, ex, null);
            }*/
            catch (Exception ex)
            {
                generateStatus = false;
                _logger.Log<string>(LogLevel.Error, 0, ex.Message, ex, null);
            }
            return generateStatus;
        }

        /// <summary>
        /// Starts converting and saving async translation to *.json file.
        /// </summary>
        /// <param name="language">
        ///     The specified two-letter language (e.g EN, VI). If language is specified, 
        ///     only translation phrases of that language going to be generated to JSOn content.
        ///     Default is null means all languages
        /// </param>
        /// <returns>True if generation action is successfully. Otherwise False</returns>
        public async Task<bool> GenerateAsync(string language = null)
        {
            bool generateStatus = false;
            try
            {
                this.ReadXmlTranslation();
                var translationList = this.GetTranslationFromDb();
                if (translationList.Count() <= 0)
                {
                    generateStatus = false;
                    _logger.Log<string>(LogLevel.Information, 0, "No data available to generate translation", null, null);
                }

                var fileNames = translationList.First().TranslationMap.Keys;
                if (!string.IsNullOrWhiteSpace(language))
                {
                    var isSupportedLanguage = fileNames.Any(f => string.Equals(f.Trim(), language.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (isSupportedLanguage)
                    {
                        await WriteData2JsonFormatAsync(translationList, language);
                    }
                }
                else
                {
                    foreach (var lang in fileNames)
                    {
                        await WriteData2JsonFormatAsync(translationList, lang);
                    }
                }
                generateStatus = true;
            }
            /*catch (JsonException ex)
            {
                _logger.Log<string>(Microsoft.Extensions.Logging.LogLevel.Error, 0, ex.Message, ex, null);
            }*/
            catch (Exception ex)
            {
                generateStatus = false;
                _logger.Log<string>(LogLevel.Error, 0, ex.Message, ex, null);
            }
            return generateStatus;
        }

        /// <summary>
        /// Collects the name, dbid attributes from specified translation xml file
        /// </summary>
        private void ReadXmlTranslation()
        {
            var xmlFileInfo = _translationFileProvider.GetFileInfo(_options.FileName);
            if (!xmlFileInfo.Exists)
            {
                throw new FileNotFoundException("Not found XML translation file", xmlFileInfo.PhysicalPath);
            }

            using (var stream = xmlFileInfo.CreateReadStream())
            {
                XDocument doc = XDocument.Load(stream);
                int dbid = -1;
                if (doc.Root.HasElements)
                {
                    var definedPhrases = doc.Root.Elements();
                    foreach (var phr in definedPhrases)
                    {
                        var dbidAttr = phr.Attributes().Where(attr => attr.Name.LocalName.Equals("dbid", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        var nameAttr = phr.Attributes().Where(attr => attr.Name.LocalName.Equals("name", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (dbidAttr != null
                            && nameAttr != null
                            && !string.IsNullOrWhiteSpace(nameAttr.Value))
                        {
                            if (int.TryParse(dbidAttr.Value, out dbid) && dbid > 0)
                            {
                                if (!_phrases.ContainsKey(dbid))
                                {
                                    _phrases.Add(dbid, nameAttr.Value);
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Duplicated phrase has DbId={dbid}");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all phrases from database with provided phrase's ID
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Translation> GetTranslationFromDb()
        {
            var ids = _phrases.Keys;

            if (ids.Count > 0)
            {
                return _translationService.GetTranslation(ids);
            }
            return new Translation[0];
        }

        /// <summary>
        /// Ensure the file content provider (directory) which can be reservable xml translation, JSON files.
        /// </summary>
        private void EnsureFileProvider()
        {
            if(string.IsNullOrWhiteSpace(_options.ResourcePath))
            {
                throw new ArgumentException("The path of Translation's resource is not valid. The path must be a none-empty relative/absolute path");
            }
            if(_options.ResourcePath.Intersect(Path.GetInvalidFileNameChars()).Any()
                || _options.ResourcePath.Intersect(Path.GetInvalidPathChars()).Any())
            {
                throw new IOException($"Path {_options.ResourcePath} contains invalid character(s)");
            }

            string resourceAbsolutePath = _options.ResourcePath;

            if (!Path.IsPathRooted(resourceAbsolutePath))
            {
                resourceAbsolutePath = Path.Combine(_env.WebRootPath, resourceAbsolutePath);
            }

            _translationFileProvider = new PhysicalFileProvider(resourceAbsolutePath);
            _rootDirectory = resourceAbsolutePath;
        }

        /// <summary>
        /// Writes the list of translation phrases to JSON
        /// </summary>
        /// <param name="translationList">List of translation</param>
        /// <param name="lang">Two-letter language</param>
        private void WriteData2JsonFormat(IEnumerable<Translation> translationList, string lang)
        {
            string fileName = Path.Combine(_rootDirectory, lang.Trim().ToUpper());
            fileName = Path.ChangeExtension(fileName, "json");

            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new StreamWriter(fileStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.WriteStartObject();

                foreach (var tran in translationList)
                {
                    var phraseName = _phrases[tran.Id];
                    var phraseValue = tran.TranslationMap[lang];
                    jsonWriter.WritePropertyName(phraseName);
                    jsonWriter.WriteValue(phraseValue);
                }
                jsonWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Writes async the list of translation phrases to JSON
        /// </summary>
        /// <param name="translationList">List of translation</param>
        /// <param name="lang">Two-letter language</param>
        private async Task WriteData2JsonFormatAsync(IEnumerable<Translation> translationList, string lang)
        {
            string fileName = Path.Combine(_rootDirectory, lang.ToUpper());
            fileName = Path.ChangeExtension(fileName, "json");

            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new StreamWriter(fileStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                await jsonWriter.WriteStartObjectAsync();

                foreach (var tran in translationList)
                {
                    var phraseName = _phrases[tran.Id];
                    await jsonWriter.WritePropertyNameAsync(phraseName);
                    await jsonWriter.WriteValueAsync(tran.TranslationMap[lang]);
                }
                await jsonWriter.WriteEndObjectAsync();
            }
        }
    }
}

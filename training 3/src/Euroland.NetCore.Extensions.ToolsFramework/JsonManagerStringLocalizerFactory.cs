using System;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    public class JsonManagerStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ConcurrentDictionary<string, JsonManagerStringLocalizer> _localizerCache = 
            new ConcurrentDictionary<string, JsonManagerStringLocalizer>();
        //private readonly IJsonResourceNameCache _cache = new JsonResourceNameCache();
        private readonly IHostingEnvironment _appHostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILanguageToCultureProvider _languageToCultureProvider;
        private readonly string _languageToCultureResourcePath;
        private readonly string _jsonResourceRelativePath;

        /// <summary>
        /// Creates a new <see cref="JsonManagerStringLocalizerFactory"/>
        /// </summary>
        public JsonManagerStringLocalizerFactory(
            IHostingEnvironment appHostingEnvironment, 
            ILoggerFactory loggerFactory,
            IOptions<JsonLocalizationOptions> localizationOptions)
        {
            if (appHostingEnvironment == null)
                throw new ArgumentNullException(nameof(appHostingEnvironment));

            if (localizationOptions == null)
                throw new ArgumentNullException(nameof(localizationOptions));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            JsonLocalizationOptions options = localizationOptions?.Value;

            this._appHostingEnvironment = appHostingEnvironment;
            this._jsonResourceRelativePath = options?.ResourcePath ?? "Translation";
            this._languageToCultureResourcePath = options?.LanguageToCultureMappingPath;
            this._languageToCultureProvider = options?.CustomLanguageToCultureProvider;
            this._loggerFactory = loggerFactory;
        }
        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            var typeInfo = resourceSource.GetTypeInfo();
            var assembly = typeInfo.Assembly;
            var assemblyName = new AssemblyName(assembly.FullName);
            var resourcePath = GetResourcePath(assembly);

            var baseName =  GetResourcePrefix(typeInfo, assemblyName.Name, resourcePath);

            return _localizerCache.GetOrAdd(baseName, key => CreateJsonManagerStringLocalizer(baseName));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            baseName = GetResourcePrefix(baseName, location);

            return _localizerCache.GetOrAdd(baseName, key => CreateJsonManagerStringLocalizer(baseName));
        }

        /// <summary>
        /// Gets the resource prefix used to look up the resource.
        /// </summary>
        /// <param name="typeInfo">The type of the resource to be looked up.</param>
        /// <param name="baseNamespace">The base namespace of the application.</param>
        /// <param name="resourcesRelativePath">The folder containing all resources.</param>
        /// <returns>The prefix for resource lookup.</returns>
        /// <remarks>
        /// For the type "Sample.Controllers.Home" if there's a resourceRelativePath return
        /// "Sample.Resourcepath.Controllers.Home" if there isn't one then it would return "Sample.Controllers.Home".
        /// </remarks>
        private string GetResourcePrefix(TypeInfo typeInfo, string baseNamespace, string resourcesRelativePath)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            if (string.IsNullOrEmpty(baseNamespace))
            {
                throw new ArgumentNullException(nameof(baseNamespace));
            }

            return string.IsNullOrEmpty(resourcesRelativePath)
                ? typeInfo.FullName
                : baseNamespace + "." + resourcesRelativePath + TrimPrefix(typeInfo.FullName, baseNamespace + ".");
        }

        /// <summary>
        /// Gets the resource prefix used to look up the resource.
        /// </summary>
        /// <param name="baseResourceName">The name of the resource to be looked up</param>
        /// <param name="baseNamespace">The base namespace of the application.</param>
        /// <returns>The prefix for resource lookup.</returns>
        private string GetResourcePrefix(string baseResourceName, string baseNamespace)
        {
            if (string.IsNullOrEmpty(baseResourceName))
            {
                throw new ArgumentNullException(nameof(baseResourceName));
            }

            if (string.IsNullOrEmpty(baseNamespace))
            {
                throw new ArgumentNullException(nameof(baseNamespace));
            }

            var assemblyName = new AssemblyName(baseNamespace);
            var assembly = Assembly.Load(assemblyName);
            var resourceLocation = GetResourcePath(assembly);
            var locationPath = baseNamespace + "." + resourceLocation;

            baseResourceName = locationPath + TrimPrefix(baseResourceName, baseNamespace + ".");

            return baseResourceName;
        }

        private ILanguageToCultureProvider CreateLanguageToCulture()
        {
            // If ILanguageToCultureProvider is provided via dependency injection, just use it
            if (_languageToCultureProvider != null)
                return _languageToCultureProvider;

            // Or try to load ILanguageToCultureProvider from provided path
            Microsoft.Extensions.FileProviders.IFileInfo fileInfo = null;

            if (!string.IsNullOrWhiteSpace(_languageToCultureResourcePath))
            {
                var physicalFileInfo = new System.IO.FileInfo(System.IO.Path.Combine(_appHostingEnvironment.ContentRootPath, _languageToCultureResourcePath));
                string[] supportedExtension = new string[2] { ".xml", ".json" };

                if (supportedExtension.Any(ext => ext.ToLower() == physicalFileInfo.Extension.ToLower()) 
                    && physicalFileInfo.Exists)
                {
                    fileInfo = new Microsoft.Extensions.FileProviders.Physical.PhysicalFileInfo(physicalFileInfo);
                }
            }

            // Otherwise, load ILanguageToCultureProvider from default resource named "DefaultLanguageToCultureConfiguration.json"
            // in Assembly
            if (fileInfo == null)
            {
                var type = typeof(JsonManagerStringLocalizerFactory);
                var assembly = type.GetTypeInfo().Assembly;
                var namespace_ = type.Namespace;

                var resourceName = string.Concat(namespace_, ".", "DefaultLanguageToCultureConfiguration.json");
                fileInfo = new AssemblyStreamFileInfo(assembly, resourceName);
            }

            string extension = System.IO.Path.GetExtension(fileInfo.Name).ToLower();

            ILanguageToCultureProvider provider = null;
            if(extension == ".json")
                provider = new JsonLanguageToCultureProvider(fileInfo, _loggerFactory.CreateLogger<JsonManagerStringLocalizer>());
            else
                provider = new XmlLanguageToCultureProvider();

            provider.Load();

            return provider;
        }

        private JsonManagerStringLocalizer CreateJsonManagerStringLocalizer(string baseName)
        {
            var jsonResourceFullPath = System.IO.Path.Combine(_appHostingEnvironment.ContentRootPath, _jsonResourceRelativePath);

            return this._localizerCache.GetOrAdd(baseName, (key) => new JsonManagerStringLocalizer(
                baseName,
                new Microsoft.Extensions.FileProviders.PhysicalFileProvider(jsonResourceFullPath),
                _loggerFactory.CreateLogger<JsonManagerStringLocalizer>(),
                this.CreateLanguageToCulture())
            );
        }

        private string GetResourcePath(Assembly assembly)
        {
            var resourceLocation = _jsonResourceRelativePath
                .Replace(System.IO.Path.DirectorySeparatorChar, '.')
                .Replace(System.IO.Path.AltDirectorySeparatorChar, '.');

            return resourceLocation;
        }

        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name.Substring(prefix.Length);
            }

            return name;
        }
    }
}

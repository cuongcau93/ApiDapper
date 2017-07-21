using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// Represents a factory to create <see cref="Abstractions.ISettingProvider"/> 
    /// based on <see cref="Abstractions.IRequestSettingFinder"/> to process resource content for JSON or XML format
    /// </summary>
    public class RequestSettingProviderFactory : PhysicalSettingProviderFactoryBase
    {
        /// <summary>
        /// List of <see cref="Abstractions.IRequestSettingFinder"/>
        /// </summary>
        public IEnumerable<Abstractions.IRequestSettingFinder> RequestFinders
        {
            get;
            set;
        }

        public Microsoft.AspNetCore.Http.HttpContext HttpContext
        {
            get;
        }

        /// <summary>
        /// Gets or sets type of format of setting resource
        /// </summary>
        public SettingResourceType ResourceType { get; set; }

        /// <summary>
        /// Create a <see cref="RequestSettingProviderFactory"/>
        /// </summary>
        /// <param name="httpContext">The context of <see cref="Microsoft.AspNetCore.Http.HttpRequest"/></param>
        public RequestSettingProviderFactory(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            HttpContext = httpContext;
            EnsureFileProvider();
        }

        /// <summary>
        /// Create a <see cref="RequestSettingProviderFactory"/>
        /// </summary>
        /// <param name="httpContext">The context of <see cref="Microsoft.AspNetCore.Http.HttpRequest"/></param>
        /// <param name="settingRootPath">The absolute physical path to the root directory where the setting files are located</param>
        public RequestSettingProviderFactory(Microsoft.AspNetCore.Http.HttpContext httpContext, string settingRootPath)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            if (settingRootPath == null)
                throw new ArgumentNullException(nameof(settingRootPath));

            HttpContext = httpContext;
            Path = settingRootPath;
            EnsureFileProvider();
        }

        /// <summary>
        /// Create a new <see cref="RequestSettingProviderFactory"/>
        /// </summary>
        /// <param name="httpContext">The context of <see cref="Microsoft.AspNetCore.Http.HttpRequest"/></param>
        /// <param name="settingRootPath">The root directory contains the setting resources</param>
        /// <param name="requestFinders">List of <see cref="Abstractions.IRequestSettingFinder"/></param>
        public RequestSettingProviderFactory(
            Microsoft.AspNetCore.Http.HttpContext httpContext, 
            string settingRootPath,
            IEnumerable<Abstractions.IRequestSettingFinder> requestFinders)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (requestFinders == null)
                throw new ArgumentNullException(nameof(requestFinders));

            if (settingRootPath == null || string.IsNullOrEmpty(settingRootPath))
                throw new ArgumentNullException(nameof(settingRootPath));
            HttpContext = httpContext;
            Path = settingRootPath;
            RequestFinders = requestFinders;
            EnsureFileProvider();
        }
        public override IFileInfo GetSettingFileInfo()
        {
            var resource = GetRequestSettingResult();
            if ((resource == null || string.IsNullOrWhiteSpace(resource.CompanyCode)))
            {
                if (Optional)
                    return new NotFoundFileInfo("RequestResource");
                else
                    throw new SettingFileNotFoundException("Missing request CompanyCode");
            }

            if (resource.CompanyCode.ToCharArray().Intersect(System.IO.Path.GetInvalidFileNameChars()).Any())
            {
                if(!Optional)
                    throw new SettingInvalidFileNameException("Resource name contains invalid characters", resource.CompanyCode);
                else
                    return new NotFoundFileInfo("RequestResource");
            }

            IFileInfo fileInfo = null;

            switch (ResourceType)
            {
                case SettingResourceType.Json:
                    fileInfo = GetJsonResource(resource);
                    break;
                case SettingResourceType.Xml:
                    fileInfo = GetXmlResource(resource);
                    break;
                case SettingResourceType.Json_Xml:
                    fileInfo = GetJsonResource(resource);
                    if (!fileInfo.Exists)
                        fileInfo = GetXmlResource(resource);
                    break;
            }

            return fileInfo ?? new NotFoundFileInfo("RequestResource");
        }

        private IFileInfo GetJsonResource(Abstractions.SettingResourceResult resourceResult)
        {
            return GetFileInfo(resourceResult, "json");
        }

        private IFileInfo GetXmlResource(Abstractions.SettingResourceResult resourceResult)
        {
            return GetFileInfo(resourceResult, "xml");
        }

        private IFileInfo GetFileInfo(Abstractions.SettingResourceResult resourceResult, string extension)
        {
            var fileName = System.IO.Path.ChangeExtension(resourceResult.CompanyCode, extension);
            var error = new System.Text.StringBuilder();
            IFileInfo fileInfo = null;

            if (!string.IsNullOrWhiteSpace(resourceResult.Version))
            {
                // Version file is always located inside company-code directory
                fileName = System.IO.Path.Combine(resourceResult.CompanyCode, System.IO.Path.ChangeExtension(resourceResult.Version, extension));
                fileInfo = FileProvider.GetFileInfo(fileName);
            }
            else
            {
                fileName = System.IO.Path.Combine(resourceResult.CompanyCode, fileName);
                fileInfo = FileProvider.GetFileInfo(fileName);

                // If not found resource located in company-code directory
                // Try find one more at the root
                if (!fileInfo.Exists)
                {
                    fileName = System.IO.Path.ChangeExtension(resourceResult.CompanyCode, extension);
                    fileInfo = FileProvider.GetFileInfo(fileName);
                }
            }

            return fileInfo;
        }

        private Abstractions.SettingResourceResult GetRequestSettingResult()
        {
            Abstractions.SettingResourceResult result = null;

            foreach (var finder in RequestFinders)
            {
                result = finder.DetermineProviderSettingResourceResult(HttpContext);

                if (result != null)
                    break;
            }

            return result;
        }
    }
}

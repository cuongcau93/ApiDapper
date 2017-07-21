using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// Determines the information for a request via values in the query string
    /// </summary>
    public class QueryStringRequestSettingFinder : Abstractions.IRequestSettingFinder
    {
        /// <summary>
        /// Gets or sets key that contains the company code value
        /// </summary>
        public string CompanyCodeQueryStringKey { get; set; } = "companycode";

        /// <summary>
        /// Gets or sets key that contains the version of setting. If version is specified,
        /// the <see cref="IServiceProvider"/> will use version as a main setting
        /// </summary>
        public string VersionQueryStringKey { get; set; } = "v";

        /// <summary>
        /// Gets or sets key that contains the current language
        /// </summary>
        public string LanguageQueryStringKey { get; set; } = "lang";

        public Abstractions.SettingResourceResult DetermineProviderSettingResourceResult(HttpContext httpContext)
        {
            if(httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var request = httpContext.Request;
            if (!request.QueryString.HasValue)
            {
                return null;
            }

            string companyCode = null;
            string version = null;
            string lang = null;

            if(!string.IsNullOrWhiteSpace(CompanyCodeQueryStringKey))
            {
                companyCode = request.Query[CompanyCodeQueryStringKey];
            }

            if(!string.IsNullOrWhiteSpace(VersionQueryStringKey))
            {
                version = request.Query[VersionQueryStringKey];
            }

            if(!string.IsNullOrWhiteSpace(LanguageQueryStringKey))
            {
                lang = request.Query[LanguageQueryStringKey];
            }

            if(!string.IsNullOrWhiteSpace(companyCode))
            {
                return new Abstractions.SettingResourceResult()
                {
                    CompanyCode = companyCode,
                    Version = version,
                    Language = string.IsNullOrWhiteSpace(lang) ? "en-GB" : lang
                };
            }

            return null;
        }
    }
}

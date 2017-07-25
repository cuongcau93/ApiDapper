using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// Determines the information for a request via values in the form
    /// </summary>
    public class FormRequestSettingFinder : Abstractions.IRequestSettingFinder
    {
        private readonly HttpContext _httpContext;
        public FormRequestSettingFinder(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            _httpContext = httpContext;
        }
        /// <summary>
        /// Gets or sets key that contains the company code value
        /// </summary>
        public string CompanyCodeFormKey { get; set; } = "companycode";

        /// <summary>
        /// Gets or sets key that contains the version of setting. If version is specified,
        /// the <see cref="IServiceProvider"/> will use version as a main setting
        /// </summary>
        public string VersionFormKey { get; set; } = "v";

        /// <summary>
        /// Gets or sets key that contains the current language
        /// </summary>
        public string LanguageFormKey { get; set; } = "lang";

        public SettingResourceResult DetermineProviderSettingResourceResult()
        {
            if (_httpContext == null)
            {
                throw new ArgumentNullException(nameof(_httpContext));
            }

            var request = _httpContext.Request;
            if (!request.HasFormContentType || request.Form.Count == 0)
            {
                return null;
            }

            string companyCode = null;
            string version = null;
            string lang = null;

            if (!string.IsNullOrWhiteSpace(CompanyCodeFormKey))
            {
                companyCode = request.Form[CompanyCodeFormKey].FirstOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(VersionFormKey))
            {
                version = request.Form[VersionFormKey].FirstOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(LanguageFormKey))
            {
                lang = request.Form[LanguageFormKey].FirstOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(companyCode))
            {
                return new SettingResourceResult()
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

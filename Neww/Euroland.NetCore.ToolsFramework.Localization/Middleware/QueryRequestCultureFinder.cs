using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Implementation of <see cref="IRequestCultureFinder"/> to determine
    /// culture of given request via QueryString and Form
    /// </summary>
    public class QueryRequestCultureFinder : IRequestCultureFinder
    {
        /// <summary>
        /// Gets/sets the key that contains the culture name
        /// Default to "lang"
        /// </summary>
        public string QueryStringKey { get; set; } = "lang";

        public Task<RequestCultureResult> GetRequestCulture(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            string queryCulture = null;

            var request = httpContext.Request;
            if (!request.QueryString.HasValue)
            {
                queryCulture = "en-US";
            }

            if (!string.IsNullOrWhiteSpace(QueryStringKey))
            {
                queryCulture = request.Query[QueryStringKey];
                if(string.IsNullOrWhiteSpace(queryCulture) && request.HasFormContentType)
                    queryCulture = request.Form[QueryStringKey];
            }
            
            
            System.Globalization.CultureInfo culture = null;
            if (!string.IsNullOrWhiteSpace(queryCulture))
            {
                try
                {
                    culture = new System.Globalization.CultureInfo(queryCulture);
                    return Task.FromResult(new RequestCultureResult(culture, culture));
                }
                catch (System.Globalization.CultureNotFoundException ex)
                {
             
                }
            }

            return Task.FromResult(default(RequestCultureResult));
        }
    }
}

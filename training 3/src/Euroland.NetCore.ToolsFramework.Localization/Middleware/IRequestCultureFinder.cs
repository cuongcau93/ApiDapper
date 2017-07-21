using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Represents a provider for determining the culture of <see cref="HttpRequest"/>
    /// </summary>
    public interface IRequestCultureFinder
    {
        /// <summary>
        /// Implements the provider to determine the culture of given request
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> for the request</param>
        /// <returns>
        ///     The determined <see cref="RequestCultureResult"/>. Returns <c>null</c> 
        ///     if the provider couldn't determine a <see cref="RequestCultureResult"/>
        /// </returns>
        Task<RequestCultureResult> GetRequestCulture(HttpContext httpContext);
    }
}

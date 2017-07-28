using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Euroland.NetCore.ToolsFramework.Setting.Abstractions
{
    /// <summary>
    /// Represents a provider for determining the setting resource of an <see cref="HttpRequest"/>
    /// </summary>
    public interface IRequestSettingFinder
    {
        /// <summary>
        /// Implements the provider to determine the setting resource of the given request
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/> for the request</param>
        /// <returns></returns>
        SettingResourceResult DetermineProviderSettingResourceResult();
    }
}

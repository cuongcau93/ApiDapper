using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Enables automatic setting of the culture for <see cref="HttpRequest"/>s based on information
    /// sent by the client in headers and logic provided by the application.
    /// </summary>
    public class RequestLocalizationMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly RequestLocalizationOptions _options;

        public RequestLocalizationMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> options)
        {
            if(next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _nextDelegate = next;
            _options = (options ?? Options.Create<RequestLocalizationOptions>(new RequestLocalizationOptions())).Value;
        }

        public async System.Threading.Tasks.Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            RequestCultureResult result = null;
            IRequestCultureFinder requestFinder = null;

            if(_options.RequestCultureFinders != null)
            {
                foreach (var cultureFinder in _options.RequestCultureFinders)
                {
                    result = await cultureFinder.GetRequestCulture(httpContext);
                    if(result != null)
                    {
                        requestFinder = cultureFinder;
                        break;
                    }
                }
            }

            System.Globalization.CultureInfo culture = null;
            System.Globalization.CultureInfo uiCulture = null;

            if (result != null)
            {
                culture = result.Culture;
                uiCulture = result.UICulture;
            }
            else
            {
                culture = uiCulture = _options.DefaultCulture;
            }

            httpContext.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(culture, uiCulture, requestFinder));

            System.Globalization.CultureInfo.CurrentCulture = culture;
            System.Globalization.CultureInfo.CurrentUICulture = uiCulture;

            await _nextDelegate(httpContext);
        }
    }
}

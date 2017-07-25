using Euroland.NetCore.ToolsFramework.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="RequestLocalizationMiddleware"/> to automatically set culture information for
        /// requests based on information provided by the client. This should add right before <c>app.UseMvc()</c> method
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseEurolandRequestLocalization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<RequestLocalizationMiddleware>();
        }

        /// <summary>
        /// Adds the <see cref="RequestLocalizationMiddleware"/> to automatically set culture information for
        /// requests based on information provided by the client.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="options">The <see cref="RequestLocalizationOptions"/> to configure the middleware with.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseEurolandRequestLocalization(
            this IApplicationBuilder app,
            RequestLocalizationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<RequestLocalizationMiddleware>(Options.Create(options));
        }
    }
}

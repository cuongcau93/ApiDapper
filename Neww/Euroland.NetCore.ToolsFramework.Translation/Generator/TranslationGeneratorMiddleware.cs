using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    /// <summary>
    /// Enable to generate the translation xml resources to JSON file format used for Localization
    /// </summary>
    public class TranslationGeneratorMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _nextRequest;
        private readonly TranslationOptions _options;
        private readonly TranslationGenerator _generator;
        public TranslationGeneratorMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next, 
            TranslationGenerator generator,
            Microsoft.Extensions.Options.IOptions<TranslationOptions> options)
        {
            if(next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if(generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            _nextRequest = next;
            _generator = generator;
            _options = options.Value;
        }

        public async System.Threading.Tasks.Task Invoke(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var request = httpContext.Request;
            var response = httpContext.Response;
           
            string lang = null;
            if (request.QueryString.HasValue)
            {
                lang = request.Query["lang"];
            }

            DateTime date = DateTime.UtcNow;

            var val = await _generator.GenerateAsync(lang);

            TimeSpan executionTime = DateTime.UtcNow.Subtract(date);

            StringBuilder content = new StringBuilder();
            content.Append($"<p>Translation generation result:</p>");
            content.Append($"<p>Status: <strong>{val}</strong></p>");
            content.Append($"<p>Executed in: <strong>{executionTime.TotalSeconds}s</strong></p>");
            if (lang != null)
                content.Append($"<p>Language: <strong>{lang}</strong></p>");

            response.StatusCode = 200;
            response.ContentType = "text/html";

            // Write content to response stream and finish the middleware pipeline
            await response.WriteAsync(content.ToString());
        }
    }
}

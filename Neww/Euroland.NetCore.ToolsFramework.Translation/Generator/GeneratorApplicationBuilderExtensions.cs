using Microsoft.AspNetCore.Builder;
using System;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    public static class GeneratorApplicationBuilderExtensions
    {
        /// <summary>
        /// Add the <see cref="TranslationGeneratorMiddleware"/> to middleware pipline to 
        /// handle generating translation manually to JSON format
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        /// <param name="options">The options for <see cref="TranslationGeneratorMiddleware"/></param>
        /// <returns>The <see cref="IApplicationBuilder"/></returns>
        public static IApplicationBuilder UseEurolandTranslationGenerator(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var generator = app.ApplicationServices.GetService(typeof(TranslationGenerator));

            if (generator == null)
                throw new InvalidOperationException($"The DI could not resolve for {nameof(Euroland.NetCore.ToolsFramework.Translation.TranslationGenerator)}");

            app.Map("/TranslationGeneration", (appBuilder) => appBuilder.UseMiddleware<TranslationGeneratorMiddleware>());
            
            return app;
        }
    }
}

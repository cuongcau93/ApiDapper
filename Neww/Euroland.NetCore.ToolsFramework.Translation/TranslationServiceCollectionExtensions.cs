using Euroland.NetCore.ToolsFramework.Translation;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TranslationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds singleton <see cref="ITranslationDataContext"/>
        /// and <see cref="ITranslationService"/> to <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/></param>
        /// <param name="translationConnectionString">The connection string of Translation database</param>
        /// <returns>The <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddEurolandTranslationService(this IServiceCollection services, Action<TranslationOptions> options)
        {
            services.AddSingleton<ITranslationDataContext, TranslationDataContext>();

            services.AddSingleton<ITranslationService, TranslationService>();

            services.AddSingleton<TranslationGenerator>();

            services.Configure(options);

            return services;
        }
    }
}

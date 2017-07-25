using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Euroland.NetCore.ToolsFramework.Localization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonLocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for application localization by using JSON resources
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained</returns>
        public static IServiceCollection AddEurolandJsonLocalization(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddOptions();

            AddLocalizationServices(services);

            return services;
        }

        /// <summary>
        /// Adds services required for application localization by using JSON resources
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to</param>
        /// <param name="setupAction">
        ///     An <see cref="Action{JsonLocalizationOptions}"/> to confgure the <see cref="JsonLocalizationOptions"/>
        /// </param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained</returns>
        public static IServiceCollection AddEurolandJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptions> setupAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));
            
            AddLocalizationServices(services, setupAction);
            return services;
        }

        internal static void AddLocalizationServices(IServiceCollection services)
        {
            services.TryAddSingleton<IStringLocalizerFactory, JsonManagerStringLocalizerFactory>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(JsonStringLocalizer<>));
        }

        internal static void AddLocalizationServices(
            IServiceCollection services,
            Action<JsonLocalizationOptions> setupAction)
        {
            AddLocalizationServices(services);
            services.Configure(setupAction);
        }
    }
}

using Euroland.NetCore.ToolsFramework.Setting;
using Euroland.NetCore.ToolsFramework.Setting.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    // References: https://dotnetliberty.com/index.php/2016/04/11/asp-net-core-custom-service-based-on-request/

    /// <summary>
    /// Extension methods for custom service based on Reqquest
    /// </summary>
    public static class SettingServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientEurolandSetting(this IServiceCollection services, Action<SettingOptions> options)
        {
            // Service Lifetimes reference: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection#service-lifetimes-and-registration-options

            services.AddScoped<ISettingItemRoot>((serviceProvider) =>
            {
                var opts = new SettingOptions();
                options(opts);

                if(string.IsNullOrWhiteSpace(opts.ApplicationName))
                    throw new ArgumentNullException(nameof(opts.ApplicationName));

                if (string.IsNullOrWhiteSpace(opts.ToolSettingRootPath))
                    throw new ArgumentNullException(nameof(opts.ToolSettingRootPath));


                var settingFactory = new SettingManager();
                var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                var supportedRequestSettingFinders = new IRequestSettingFinder[]
                {
                    new QueryStringRequestSettingFinder(httpContext),
                    new FormRequestSettingFinder(httpContext)
                };

                var companySettingRootPath = !string.IsNullOrWhiteSpace(opts.GeneralSettingRootPath)
                        ? Path.Combine(opts.GeneralSettingRootPath, "Company")
                        : null;
                var generalSettingFileName = !string.IsNullOrWhiteSpace(opts.GeneralSettingRootPath)
                        ? Path.Combine(opts.GeneralSettingRootPath, "settings.json")
                        : null;

                var applicationSettingFileName = Path.Combine(opts.ToolSettingRootPath, Path.ChangeExtension(opts.ApplicationName, ".json"));
                var toolCompanySettingRootPath = Path.Combine(opts.ToolSettingRootPath, "Company");

                if(opts.ResourceType == SettingResourceType.Xml)
                {
                    applicationSettingFileName = Path.ChangeExtension(applicationSettingFileName, "xml");
                }
                else if(opts.ResourceType == SettingResourceType.Json_Xml)
                {
                    var fileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(opts.ToolSettingRootPath);
                    if (!fileProvider.GetFileInfo(applicationSettingFileName).Exists)
                    {
                        applicationSettingFileName = Path.ChangeExtension(applicationSettingFileName, "xml");
                    }
                }

                if (generalSettingFileName != null)
                {
                    settingFactory.Accept(new StaticFileSettingProviderFactory(generalSettingFileName));
                }

                settingFactory.Accept(new StaticFileSettingProviderFactory(applicationSettingFileName));

                if (generalSettingFileName != null)
                {
                    settingFactory.Accept(new RequestSettingProviderFactory(companySettingRootPath, supportedRequestSettingFinders));
                }

                var toolCompanySettingProviderFactory = new RequestSettingProviderFactory(toolCompanySettingRootPath, supportedRequestSettingFinders);
                toolCompanySettingProviderFactory.ReloadOnChange = true;

                settingFactory.Accept(toolCompanySettingProviderFactory);

                return settingFactory.Create();
            });

            return services; // Chainable
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Euroland.NetCore.ToolsFramework.Localization;
using Euroland.NetCore.ToolsFramework.WebSample.Data;
using Euroland.NetCore.ToolsFramework.Data;

namespace Euroland.NetCore.ToolsFramework.WebSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            AppRoot = env.ContentRootPath;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public string AppRoot { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTransientEurolandSetting(options =>
                {
                    options.ApplicationName = "WebSample";
                    options.ResourceType = Setting.SettingResourceType.Json;
                    options.ToolSettingRootPath = System.IO.Path.Combine(AppRoot, "Config");
                })
                .AddEurolandJsonLocalization(options => options.ResourcePath = "Translations");
            //the first way
            services.AddSingleton<IDatabaseContext>(new DapperDatabaseContext(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IMailQueueRepository1, MailQueueRepository1>();

            /*//the second way
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddTransient<IMailQueueRepository2, MailQueueRepository2>();

            //the third way
            services.AddTransient<IMailQueueRepository3, MailQueueRepository3>((serviceProvider) => new MailQueueRepository3(Configuration.GetConnectionString("DefaultConnection")));

            //the four way
            services.AddTransient<IMailQueueRepository4, MailQueueRepository4>((serviceProvider) => new MailQueueRepository4(new DapperDatabaseContext(Configuration.GetConnectionString("DefaultConnection"))));
            */
            // Add framework services and view localization
            services.AddMvc().AddViewLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Determine up request culture for Euroland's application
            app.UseEurolandRequestLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

Description
=============
The application setting API provides a way of setting for a company on a list of name-value pairs.
The setting is create for each request from multiple sources. The name-value pairs can be grouped into a multi-level hierarchy
There are available setting providers for:
	* JSON format
	* XML format

This project is part of Euroland's Tools Framework based on Microsoft.Extensions.Configuration

** Dev: 
	binh.nguyen@euroland.com

** Dependencies:
	Microsoft.Extensions.FileProviders.Abstractions 1.1.2
	Microsoft.Extensions.FileProviders.Physical 1.1.1
	Microsoft.Extensions.DependencyInjection.Abstractions 1.1.1
	Microsoft.AspNetCore.Http.Abstractions 1.1.1
	Newtonsoft.Json 10.0.3


** Lastest version:
	1.0.0

** How to use:

Startup.cs

public void ConfigureServices(IServiceCollection services)
{
	services
        .AddTransientEurolandSetting(options => {
            options.ApplicationName = "WebSample";
            options.ResourceType = Setting.SettingResourceType.Json;
            options.ToolSettingRootPath = System.IO.Path.Combine(AppRoot, "Config");
        })
        .AddEurolandJsonLocalization(options => options.ResourcePath = "Translations");

    // Add framework services and view localization
    services.AddMvc().AddViewLocalization();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
	app.UseStaticFiles();

	// After
	app.UseEurolandRequestLocalization();
	// And before
	
	app.UseMvc();
}

To access setting from Controller:

public class HomeController: IController
{
	private ISettingItemRoot _setting;
	public HomeController(ISettingItemRoot setting) 
	{
		_setting = setting;
	}
}

To access setting from View:
@using Euroland.NetCore.ToolsFramework.Setting.Abstractions
@inject ISettingItemRoot setting

@setting["TimeZone"]

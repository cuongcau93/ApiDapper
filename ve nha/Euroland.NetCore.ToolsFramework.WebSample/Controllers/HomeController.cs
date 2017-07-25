using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Euroland.NetCore.ToolsFramework.Setting.Abstractions;

namespace Euroland.NetCore.ToolsFramework.WebSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly Euroland.NetCore.ToolsFramework.Localization.JsonManagerStringLocalizerFactory _localizerFactory;
        private readonly ISettingItemRoot _setting;

        public HomeController(ISettingItemRoot setting, IStringLocalizer<HomeController> localizer, IStringLocalizerFactory localizerFactory)
        {
            _setting = setting;
            _localizer = localizer;
            _localizerFactory = localizerFactory as Euroland.NetCore.ToolsFramework.Localization.JsonManagerStringLocalizerFactory;
        }
        public IActionResult Index()
        {
            var supportCultureString = "<div>" 
                + string.Join(
                    @"</div><div>", 
                    _localizerFactory.LanguageToCultureProvider.AllSupportedCultures.Select(
                        culture => $"Name: <b>{culture.DisplayName}</b>. Culture: <b>{culture.Name}</b>")
                    ) 
                + "</div>";
            ViewData["SupportedCulture"] = supportCultureString;
            return View();
        }
    }
}

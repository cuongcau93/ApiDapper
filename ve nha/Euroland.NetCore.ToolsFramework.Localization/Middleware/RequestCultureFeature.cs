using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    public class RequestCultureFeature : IRequestCultureFeature
    {
        public RequestCultureFeature(CultureInfo culture, CultureInfo uiCulture, IRequestCultureFinder requestFinder)
        {
            Culture = culture;
            UICulture = uiCulture;
            RequestFinder = requestFinder;
        }
        public CultureInfo Culture { get;}
        public CultureInfo UICulture { get; }
        public IRequestCultureFinder RequestFinder { get; }
    }
}

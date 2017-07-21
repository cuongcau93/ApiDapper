using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    public interface IRequestCultureFeature
    {
        System.Globalization.CultureInfo Culture { get; }

        System.Globalization.CultureInfo UICulture { get; }

        IRequestCultureFinder RequestFinder { get; }
    }
}

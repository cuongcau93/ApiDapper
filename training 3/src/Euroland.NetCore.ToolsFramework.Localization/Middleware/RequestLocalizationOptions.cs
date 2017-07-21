using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// Specifies options for the <see cref="RequestLocalizationMiddleware"/>
    /// </summary>
    public class RequestLocalizationOptions
    {
        /// <summary>
        /// Gets or sets the default <see cref="System.Globalization.CultureInfo"/> if providers couldn't 
        /// determine any culture
        /// </summary>
        public System.Globalization.CultureInfo DefaultCulture { get; set; } = new System.Globalization.CultureInfo("en-US");

        /// <summary>
        /// Gets or sets an ordered list of providers used to determine the culture for a request. 
        /// </summary>
        public IList<IRequestCultureFinder> RequestCultureFinders { get; set; }

        public RequestLocalizationOptions()
        {
            RequestCultureFinders = new List<IRequestCultureFinder>() { new QueryRequestCultureFinder() };
        }
    }
}

using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting.Abstractions
{
    /// <summary>
    /// Represents a type used to create a <see cref="ISettingProvider"/>
    /// </summary>
    public interface ISettingProviderFactory
    {
        /// <summary>
        /// Gets or sets the value to specify the setting in this root directory
        /// is optional. The one is optional means that, application will not
        /// throw an exception if not found a particular setting or an error
        /// occurs at runtime.
        /// </summary>
        bool Optional { get; set; }

        /// <summary>
        /// Creates the <see cref="ISettingProvider"/>
        /// </summary>
        /// <param name="settingFactory"></param>
        /// <returns></returns>
        ISettingProvider Create(ISettingManager settingFactory);
    }
}

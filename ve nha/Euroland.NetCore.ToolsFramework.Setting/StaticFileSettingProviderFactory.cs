using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// Implementation of <see cref="PhysicalSettingProviderFactoryBase"/> 
    /// to create <see cref="Abstractions.ISettingProvider"/> for static physical file
    /// </summary>
    public class StaticFileSettingProviderFactory : PhysicalSettingProviderFactoryBase
    {
        /// <summary>
        /// Instantiates the <see cref="StaticFileSettingProviderFactory"/>
        /// </summary>
        public StaticFileSettingProviderFactory()
        {
            EnsureFileProvider();
        }
        /// <summary>
        /// Instantiates the <see cref="StaticFileSettingProviderFactory"/>
        /// </summary>
        /// <param name="staticFilePath">The absolute path to a specified physical file</param>
        public StaticFileSettingProviderFactory(string staticFilePath)
            : this(staticFilePath, false)
        {
        }

        /// <summary>
        /// Instantiates the <see cref="StaticFileSettingProviderFactory"/>
        /// </summary>
        /// <param name="staticFilePath">The absolute path to a specified physical file</param>
        /// <param name="optional">Whether this resource is optional</param>
        public StaticFileSettingProviderFactory(string staticFilePath, bool optional)
        {
            if (staticFilePath == null)
                throw new ArgumentNullException(nameof(staticFilePath));

            Path = staticFilePath;
            Optional = optional;

            EnsureFileProvider();
        }
        public override IFileInfo GetSettingFileInfo()
        {
            return FileProvider.GetFileInfo(Path);
        }
    }
}

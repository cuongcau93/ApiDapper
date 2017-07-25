using Euroland.NetCore.ToolsFramework.Setting.Abstractions;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public abstract class PhysicalSettingProviderFactoryBase : ISettingProviderFactory
    {
        /// <summary>
        /// Gets or sets an <see cref="IFileProvider"/> for the root contains the setting file
        /// </summary>
        public IFileProvider FileProvider { get; set; }

        /// <summary>
        /// Gets or sets the value to specify the setting in this root directory
        /// is optional. The one is optional means that, application will not
        /// throw an exception if not found a particular setting or an error
        /// occurs at runtime.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Determines whether the setting should be reloaded if the underlying setting file changes
        /// </summary>
        public bool ReloadOnChange { get; set; } = false;

        /// <summary>
        /// Gets or sets path to setting file directory
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Creates the <see cref="ISettingProvider"/>
        /// </summary>
        /// <param name="settingFactory"></param>
        /// <returns></returns>
        public virtual ISettingProvider Create(ISettingManager settingFactory)
        {
            return this.CreateSmartlySettingProvider();
        }


        /// <summary>
        /// Gets an <see cref="IFileInfo"/> to be processed in <see cref="ISettingProvider"/>
        /// </summary>
        /// <returns>The <see cref="IFileInfo"/></returns>
        public abstract IFileInfo GetSettingFileInfo();


        /// <summary>
        /// Creates a <see cref="ISettingProvider"/> based on the type of resource file (.JSON or .XML)
        /// </summary>
        /// <returns>
        /// An <see cref="ISettingProvider"/>. 
        /// Returns <c>null</c> if no matching provider with file's extension is found.
        /// </returns>
        protected virtual ISettingProvider CreateSmartlySettingProvider()
        {
            var fileInfo = this.GetSettingFileInfo();
            var fileExtension = System.IO.Path.GetExtension(fileInfo.Name).ToLower();
            ISettingProvider settingProvider = null;

            if(".json" == fileExtension)
            {
                settingProvider = new Json.JsonSettingProvider(this);
            }
            else if(".xml" == fileExtension)
            {
                settingProvider = new Xml.XmlSettingProvider(this);
            }

            return settingProvider;
        }

        protected void EnsureFileProvider()
        {
            if (FileProvider == null
                && !string.IsNullOrEmpty(Path)
                && System.IO.Path.IsPathRooted(Path))
            {
                if (!string.IsNullOrWhiteSpace(System.IO.Path.GetExtension(Path)))
                {
                    var directory = System.IO.Path.GetDirectoryName(Path);
                    var pathToFile = System.IO.Path.GetFileName(Path);
                    while (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
                    {
                        pathToFile = System.IO.Path.Combine(System.IO.Path.GetFileName(directory), pathToFile);
                        directory = System.IO.Path.GetDirectoryName(directory);
                    }
                    if (System.IO.Directory.Exists(directory))
                    {
                        FileProvider = new PhysicalFileProvider(directory);
                        Path = pathToFile;
                    }
                }
                else
                {
                    FileProvider = new PhysicalFileProvider(Path);
                }
            }
        }
    }
}

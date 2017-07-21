using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public abstract class PhysicalFileSettingProvider : SettingProviderBase
    {
        private readonly PhysicalSettingProviderFactoryBase _settingProviderFactory;

        public PhysicalFileSettingProvider(PhysicalSettingProviderFactoryBase settingProviderFactory): base()
        {
            if (settingProviderFactory == null)
                throw new ArgumentNullException(nameof(settingProviderFactory));

            _settingProviderFactory = settingProviderFactory;
            
            //TODO: reload on file changes

            //if(_settingProviderFactory.ReloadOnChange && _settingProviderFactory.FileProvider != null)
            //{
            //    Microsoft.Extensions.Primitives.ChangeToken.OnChange(
            //        () => _settingProviderFactory.FileProvider.Watch(""),
                    
            //    );
            //}
        }

        public override void Load()
        {
            Load(false);
        }

        public abstract void Load(System.IO.Stream stream);

        private void Load(bool reload)
        {
            var settingFile = _settingProviderFactory.GetSettingFileInfo();

            if(settingFile == null || !settingFile.Exists)
            {
                if(_settingProviderFactory.Optional || reload)
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    var error = new StringBuilder($"The setting file '{settingFile?.Name}' was not found and is not optional.");
                    if (!string.IsNullOrEmpty(settingFile?.PhysicalPath))
                    {
                        error.Append($" The physical path is '{settingFile.PhysicalPath}'.");
                    }
                    throw new SettingFileNotFoundException(error.ToString());
                }
            }
            else
            {
                if(reload)
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                try
                {
                    using (var fileStream = settingFile.CreateReadStream())
                    {
                        Load(fileStream);
                    }
                }
                catch (System.IO.IOException ex)
                {
                    throw new SettingFileLoadException(
                        "There's error while trying to load setting stream.",
                        settingFile?.PhysicalPath,
                        ex);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public class SettingFileLoadException: System.IO.FileLoadException
    {
        public SettingFileLoadException(): base() { }

        public SettingFileLoadException(string message): base(message) { }

        public SettingFileLoadException(string message, Exception innerException) : base(message, innerException) { }

        public SettingFileLoadException(string message, string filePath) : base(message, filePath) { }

        public SettingFileLoadException(string message, string filePath, Exception innerException) : base(message, filePath, innerException) { }
    }
}

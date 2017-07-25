using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public class SettingFileNotFoundException: System.IO.FileNotFoundException
    {
        public SettingFileNotFoundException(): base() { }

        public SettingFileNotFoundException(string message): base(message) { }

        public SettingFileNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        public SettingFileNotFoundException(string message, string filePath) : base(message, filePath) { }

        public SettingFileNotFoundException(string message, string filePath, Exception innerException) : base(message, filePath, innerException) { }
    }
}

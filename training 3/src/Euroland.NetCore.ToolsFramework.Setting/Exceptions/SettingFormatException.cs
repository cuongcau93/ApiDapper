using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public class SettingFormatException: FormatException
    {
        public SettingFormatException(): base() { }

        public SettingFormatException(string message): base(message) { }

        public SettingFormatException(string message, Exception innerException) : base(message, innerException) { }
    }
}

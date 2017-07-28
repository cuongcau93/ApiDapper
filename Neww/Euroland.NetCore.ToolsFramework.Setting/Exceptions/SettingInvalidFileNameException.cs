using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public class SettingInvalidFileNameException : ArgumentException
    {
        public SettingInvalidFileNameException(): base() { }

        public SettingInvalidFileNameException(string message): base(message) { }

        public SettingInvalidFileNameException(string message, Exception innerException) : base(message, innerException) { }

        public SettingInvalidFileNameException(string message, string paramName) : base(message, paramName) { }

        public SettingInvalidFileNameException(string message, string paramName, Exception innerException) : base(message, paramName, innerException) { }
    }
}

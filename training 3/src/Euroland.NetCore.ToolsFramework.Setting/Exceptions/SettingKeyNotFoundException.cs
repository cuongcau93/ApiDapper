using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public class SettingKeyNotFoundException: System.Collections.Generic.KeyNotFoundException
    {
        public SettingKeyNotFoundException(): base() { }

        public SettingKeyNotFoundException(string message): base(message) { }

        public SettingKeyNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}

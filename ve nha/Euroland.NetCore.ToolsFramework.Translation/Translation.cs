using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    public class Translation
    {
        public int Id { get; set; }
        public IDictionary<string, string> TranslationMap { get; set; }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Test.Localization
{
    public class MockObj
    {
        public static Microsoft.Extensions.FileProviders.Physical.PhysicalFileInfo GetFile()
        {
            var resourcePath = Path.Combine(TestUtils.CurrentRootDirectory, "Localization", "DefaultLanguageToCultureConfiguration.json");

            var fileInfo = new System.IO.FileInfo(resourcePath);
            return new Microsoft.Extensions.FileProviders.Physical.PhysicalFileInfo(fileInfo);
        }

        public static ILogger GetLogger()
        {
            return new TestLogger();
        }
    }
}

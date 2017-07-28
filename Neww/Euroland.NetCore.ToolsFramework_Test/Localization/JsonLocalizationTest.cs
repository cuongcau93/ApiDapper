using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Euroland.NetCore.ToolsFramework.Localization;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Euroland.NetCore.ToolsFramework.Test.Localization
{
    public class TestJsonManagerStringLocalizerFactory : JsonManagerStringLocalizerFactory
    {
        public string BaseName { get; private set; }

        public TestJsonManagerStringLocalizerFactory(
            IHostingEnvironment appHostingEnvironment,
            ILoggerFactory loggerFactory,
            IOptions<JsonLocalizationOptions> localizationOptions)
            : base(appHostingEnvironment, loggerFactory, localizationOptions)
        {
        }
    }

    public class NoneExistFileInfo : IFileInfo
    {
        public bool Exists => false;

        public long Length => -1;

        public string PhysicalPath => "test";

        public string Name => "test";

        public DateTimeOffset LastModified => DateTime.MinValue;

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            return null;
        }
    }

    public class JsonLocalizationTest
    {
        
        [Fact]
        public void Can_Instantiate_Factory_With_Default_Language2Culture()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(host => host.ApplicationName).Returns("Test_Application");
            hostingEnvironment.Setup(host => host.ContentRootPath).Returns(System.IO.Directory.GetCurrentDirectory());

            var options = new Mock<IOptions<JsonLocalizationOptions>>();
            options.Setup(opts => opts.Value).Returns(new JsonLocalizationOptions()
            {
                ResourcePath = @"Localization\Translations"
            });

            var loggerFactory = new TestLoggerFactory();
            //var languageToCultureFactory = new JsonLanguageToCultureProvider(new NoneExistFileInfo(), loggerFactory.CreateLogger("Test"));

            var typeFactory = new TestJsonManagerStringLocalizerFactory(
                hostingEnvironment.Object,
                loggerFactory,
                options.Object
            );

            var stringFactory = new TestJsonManagerStringLocalizerFactory(
                hostingEnvironment.Object,
                loggerFactory,
                options.Object
            );

            var type = typeof(TestJsonManagerStringLocalizerFactory);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);

            var typeLocalizer = typeFactory.Create(type);
            var stringLocalizer = stringFactory.Create(type.Name, assemblyName.Name);

            string a = typeLocalizer["TestLabel"];
            string b = stringLocalizer["TestLabel"];

            Assert.Equal(a,b);
        }

        [Fact]
        public void Can_Get_Translation_With_Key_And_CaseInsensitive()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(host => host.ApplicationName).Returns("Test_Application");
            hostingEnvironment.Setup(host => host.ContentRootPath).Returns(System.IO.Directory.GetCurrentDirectory());

            var options = new Mock<IOptions<JsonLocalizationOptions>>();
            options.Setup(opts => opts.Value).Returns(new JsonLocalizationOptions()
            {
                ResourcePath = @"Localization\Translations"
            });

            var loggerFactory = new TestLoggerFactory();
            //var languageToCultureFactory = new JsonLanguageToCultureProvider(new NoneExistFileInfo(), loggerFactory.CreateLogger("Test"));

            var typeFactory = new TestJsonManagerStringLocalizerFactory(
               hostingEnvironment.Object,
               loggerFactory,
               options.Object
           );

            var stringFactory = new TestJsonManagerStringLocalizerFactory(
                hostingEnvironment.Object,
                loggerFactory,
                options.Object
            );

            var type = typeof(TestJsonManagerStringLocalizerFactory);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);

            var typeLocalizer = typeFactory.Create(type);
            var stringLocalizer = stringFactory.Create(type.Name, assemblyName.Name);

            string a = typeLocalizer["testLabel"];
            string b = stringLocalizer["Testlabel"];

            Assert.Equal(a, b);
        }

        [Fact]
        public void Can_Get_Translation_With_Specified_VIVN_CultureInfo()
        {
            var viVN = new System.Globalization.CultureInfo("vi-VN");
            System.Globalization.CultureInfo.CurrentCulture = viVN;
            System.Globalization.CultureInfo.CurrentUICulture = viVN;

            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(host => host.ApplicationName).Returns("Test_Application");
            hostingEnvironment.Setup(host => host.ContentRootPath).Returns(System.IO.Directory.GetCurrentDirectory());

            var options = new Mock<IOptions<JsonLocalizationOptions>>();
            options.Setup(opts => opts.Value).Returns(new JsonLocalizationOptions()
            {
                ResourcePath = @"Localization\Translations"
            });

            var loggerFactory = new TestLoggerFactory();
            //var languageToCultureFactory = new JsonLanguageToCultureProvider(new NoneExistFileInfo(), loggerFactory.CreateLogger("Test"));

            var typeFactory = new TestJsonManagerStringLocalizerFactory(
                hostingEnvironment.Object,
                loggerFactory,
                options.Object
            );

            var type = typeof(TestJsonManagerStringLocalizerFactory);

            var typeLocalizer = typeFactory.Create(type);
            

            Assert.Equal("Vietnamese test label", typeLocalizer["testLabel"]);
        }

        [Fact]
        public void Can_Get_Translation_With_Configuring_Manually_Language2Culture_Resource()
        {
            // The culture "en-US" is not specified in /Localization/DefaultLanguageToCultureConfiguration.json
            var enUS = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.CurrentCulture = enUS;
            System.Globalization.CultureInfo.CurrentUICulture = enUS;

            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(host => host.ApplicationName).Returns("Test_Application");
            hostingEnvironment.Setup(host => host.ContentRootPath).Returns(System.IO.Directory.GetCurrentDirectory());

            var options = new Mock<IOptions<JsonLocalizationOptions>>();
            options.Setup(opts => opts.Value).Returns(new JsonLocalizationOptions()
            {
                ResourcePath = @"Localization\Translations",
                LanguageToCultureMappingPath = Path.Combine(TestUtils.CurrentRootDirectory, "Localization", "DefaultLanguageToCultureConfiguration.json")
            });

            var loggerFactory = new TestLoggerFactory();

            var typeFactory = new TestJsonManagerStringLocalizerFactory(
                hostingEnvironment.Object,
                loggerFactory,
                options.Object
            );

            var type = typeof(TestJsonManagerStringLocalizerFactory);

            var typeLocalizer = typeFactory.Create(type);

            string label = typeLocalizer["testLabel"];

            Assert.NotEqual("English Test label", label);
        }

        [Fact]
        public void Return_name_of_lable_if_not_found_a_specified_culture()
        {
            var enUS = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.CurrentCulture = enUS;
            System.Globalization.CultureInfo.CurrentUICulture = enUS;

            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(host => host.ApplicationName).Returns("Test_Application");
            hostingEnvironment.Setup(host => host.ContentRootPath).Returns(System.IO.Directory.GetCurrentDirectory());

            var options = new Mock<IOptions<JsonLocalizationOptions>>();
            options.Setup(opts => opts.Value).Returns(new JsonLocalizationOptions()
            {
                ResourcePath = @"Localization\Translations",
                LanguageToCultureMappingPath = Path.Combine(TestUtils.CurrentRootDirectory, "Localization", "DefaultLanguageToCultureConfiguration.json")
            });

            var loggerFactory = new TestLoggerFactory();

            var typeFactory = new TestJsonManagerStringLocalizerFactory(
                hostingEnvironment.Object,
                loggerFactory,
                options.Object
            );

            var type = typeof(TestJsonManagerStringLocalizerFactory);

            var typeLocalizer = typeFactory.Create(type);


            Assert.Equal("testLabel", typeLocalizer["testLabel"]);
        }
    }

    
}

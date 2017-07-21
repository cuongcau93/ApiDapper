using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Euroland.NetCore.ToolsFramework.Setting;
using Euroland.NetCore.ToolsFramework.Test.Helpers;
using Euroland.NetCore.ToolsFramework.Setting.Abstractions;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections;

namespace Euroland.NetCore.ToolsFramework.Test.Setting
{
    public class RequestSettingTest
    {
        [Fact]
        public void CanHandleSettingFromRequestQueryString()
        {
            var settingFactory = new SettingManager();
           
            var supportedRequestSettingFinders = new IRequestSettingFinder[]
            {
                new QueryStringRequestSettingFinder()
            };
            
            var rootPath = Path.Combine(TestUtils.CurrentRootDirectory, @"tools\ToolTest\config\company");
            var requestSettingProvider = new RequestSettingProviderFactory(HttpUtils.MockHttpContext(), rootPath, supportedRequestSettingFinders);
            requestSettingProvider.ResourceType = SettingResourceType.Json_Xml;
            settingFactory.Accept(requestSettingProvider);
            var root = settingFactory.Create();
            Assert.NotNull(root);
        }

        [Fact]
        public void CanHandleSettingFromFormCollection()
        {
            var settingFactory = new SettingManager();

            var supportedRequestSettingFinders = new IRequestSettingFinder[]
            {
                new FormRequestSettingFinder()
            };

            var rootPath = Path.Combine(TestUtils.CurrentRootDirectory, @"tools\ToolTest\config\company");

            var requestSettingProvider = new RequestSettingProviderFactory(HttpUtils.MockHttpContext(), rootPath, supportedRequestSettingFinders);
            requestSettingProvider.ResourceType = SettingResourceType.Json_Xml;
            settingFactory.Accept(requestSettingProvider);

            var root = settingFactory.Create();
            Assert.NotNull(root);
        }

        [Fact]
        public void ImplicitReplacement_and_Merge_4_Levels_setting()
        {
            var settingFactory = new SettingManager();
            var httpContext = HttpUtils.MockHttpContext();
            var rootPath = Path.Combine(TestUtils.CurrentRootDirectory, @"tools\ToolTest\config\company");
            
            var supportedRequestSettingFinders = new IRequestSettingFinder[]
            {
                    new QueryStringRequestSettingFinder(),
                    new FormRequestSettingFinder()
            };

            var generalSettingFileName = Path.Combine(TestUtils.CurrentRootDirectory, @"tools\config\generalSetting.xml");
            var companySettingRootPath = Path.Combine(TestUtils.CurrentRootDirectory, @"tools\config\company");
            var applicationSettingFileName = Path.Combine(TestUtils.CurrentRootDirectory, @"tools\ToolTest\config\ToolTest.xml");
            var toolCompanySettingRootPath = Path.Combine(TestUtils.CurrentRootDirectory, @"tools\ToolTest\config\company");


            settingFactory.Accept(new StaticFileSettingProviderFactory(generalSettingFileName));
            settingFactory.Accept(new StaticFileSettingProviderFactory(applicationSettingFileName));
            settingFactory.Accept(new RequestSettingProviderFactory(httpContext, companySettingRootPath, supportedRequestSettingFinders) { ResourceType = SettingResourceType.Json_Xml });
            settingFactory.Accept(new RequestSettingProviderFactory(httpContext, toolCompanySettingRootPath, supportedRequestSettingFinders) { ResourceType = SettingResourceType.Json_Xml });

            var root = settingFactory.Create();
            var child = root.GetChild("format");

            Assert.Equal("Level4", root["level"]);
            Assert.Equal("MM-dd-yyyy", root["format:en-gb:ShortDate"]);
            Assert.Equal("hh:mm:ss", root["format:en-gb:TimeFormat"]);
        }
    }
}

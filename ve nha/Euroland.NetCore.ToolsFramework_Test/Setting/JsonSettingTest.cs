using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Euroland.NetCore.ToolsFramework.Setting;
using Euroland.NetCore.ToolsFramework.Setting.Json;
using Euroland.NetCore.ToolsFramework.Test.Helpers;

namespace Euroland.NetCore.ToolsFramework.Test.Setting
{
    public class JsonSettingTest
    {
        [Fact]
        public void PrimitiveArrayAreConvertedToKeyValuePairs()
        {
            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory("test", true);
            JsonSettingProvider converter = new JsonSettingProvider(settingProviderFactory);
            string jsonArray = @"{
                'test': [
                    '1',
                    '2',
                    '3',
                    '4'
                ]
            }";

            converter.Load(StreamHelper.StringToStream(jsonArray));

            string value1 = null, value2 = null, value3 = null, value4 = null;
            bool converted = converter.TryGet("test:0", out value1);

            converter.TryGet("test:1", out value2);
            converter.TryGet("test:2", out value3);
            converter.TryGet("test:3", out value4);


            Assert.True(converted);
            Assert.Equal("1", value1);
            Assert.Equal("2", value2);
            Assert.Equal("3", value3);
            Assert.Equal("4", value4);
        }

        [Fact]
        public void ArrayOfObjectAreConvertedToKeyValuePair()
        {
            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory("test", true);
            JsonSettingProvider converter = new JsonSettingProvider(settingProviderFactory);
            string jsonArray = @"{
                'culture': [
                    { 'dateFormat': 'MMddyyyy', 'timeFormat': 'HH:mm' },
                    { 'dateFormat': 'ddMMyyyy', 'timeFormat': 'HH:mm' }
                ]
            }";

            converter.Load(StreamHelper.StringToStream(jsonArray));

            string dateFormat1 = null, dateFormat2 = null;
            bool converted = converter.TryGet("culture:0:dateFormat", out dateFormat1);
            converter.TryGet("culture:1:dateFormat", out dateFormat2);

            Assert.True(converted);
            Assert.Equal("MMddyyyy", dateFormat1);
            Assert.Equal("ddMMyyyy", dateFormat2);
        }

        [Fact]
        public void CanOverwriteSettingValue()
        {
            string jsonString = @"{
                'enabledSubscription': 'false'
            }";
            string jsonReplacementString = @"{
                'enabledSubscription': 'true'
            }";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory("test");
            settingProviderFactory.FileProvider = new MemoryFileProvider(jsonString, "test.json");

            StaticFileSettingProviderFactory replacementSettingProviderFactory = new StaticFileSettingProviderFactory("test");
            replacementSettingProviderFactory.FileProvider = new MemoryFileProvider(jsonReplacementString, "test.json");

            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);
            manager.Accept(replacementSettingProviderFactory);

            var root = manager.Create();

            Assert.Equal("true", root["enabledSubscription"]);
        }

        [Fact]
        public void CanMergeSetting()
        {
            string jsonString = @"{
                'format': {
                    'fontFamily': 'Tahoma',
                    'fontSize': 20
                }
            }";
            string jsonReplacementString = @"{
                'enabledSubscription': 'true'
            }";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(jsonString, "test.json")
            };

            StaticFileSettingProviderFactory replacementSettingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(jsonReplacementString, "test.json")
            };

            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);
            manager.Accept(replacementSettingProviderFactory);

            var root = manager.Create();

            Assert.Equal("true", root["enabledSubscription"]);
            Assert.Equal("Tahoma", root["format:fontFamily"]);
            Assert.Equal("20", root["format:fontSize"]);
        }

        [Fact]
        public void CanHandleEmptySettingValue()
        {
            string jsonString = @"{
                'format': {
                    'fontFamily': ''
                }
            }";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(jsonString, "test.json")
            };

            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var root = manager.Create();

            Assert.Equal("", root["format:fontFamily"]);
        }

        [Fact]
        public void ThrowExceptionForMissingSettingFile()
        {
            string file = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "nonExisting.json");
            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory(file, false);
            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var exception = Assert.Throws<SettingFileNotFoundException>(() => manager.Create());
            Assert.NotNull(exception);
        }

        [Fact]
        public void ThrowFileLoadException()
        {
            string errorJsonString = @"{
                'format': {
                    'fontFamily':
                }
            }";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(errorJsonString, "test.json")
            };

            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var exception = Assert.Throws<SettingFormatException>(() => manager.Create());

            Assert.NotNull(exception);
        }

        [Fact]
        public void DoesNotThrownExceptionOnOptionalSetting()
        {
            string file = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "nonExisting.json");
            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory(file, true);
            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var root = manager.Create();
        }
    }
}

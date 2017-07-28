using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Euroland.NetCore.ToolsFramework.Setting;
using Euroland.NetCore.ToolsFramework.Setting.Json;
using Euroland.NetCore.ToolsFramework.Test.Helpers;
using Euroland.NetCore.ToolsFramework.Setting.Xml;

namespace Euroland.NetCore.ToolsFramework.Test.Setting
{
    public class XmlSettingTest
    {
        [Fact]
        public void SupportXmlCDATAValue()
        {
            string CDATA_value = "><&!hihihehe";
            string xml = $"<settings><format><![CDATA[{CDATA_value}]]></format></settings>";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory();
            XmlSettingProvider converter = new XmlSettingProvider(settingProviderFactory);

            converter.Load(StreamHelper.StringToStream(xml));

            string value = null;
            converter.TryGet("format", out value);

            Assert.Equal(CDATA_value, value);
        }

        [Fact]
        public void IgnoreXmlCommentNode()
        {
            string xml = $"<settings><format><!--<comment>Comment Text</comment>-->Real Value</format></settings>";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory();
            XmlSettingProvider converter = new XmlSettingProvider(settingProviderFactory);

            converter.Load(StreamHelper.StringToStream(xml));

            string value = null;
            converter.TryGet("format", out value);

            Assert.Equal("Real Value", value);
        }

        [Fact]
        public void PrimitiveArrayAreConvertedToKeyValuePairs()
        {
            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory();
            XmlSettingProvider converter = new XmlSettingProvider(settingProviderFactory);
            string xml = @"
                <settings>
                    <font>
                        <name>Arial</name>
                        <name>Tahoma</name>
                    </font>
                </settings>";

            converter.Load(StreamHelper.StringToStream(xml));

            string value1 = null, value2 = null;

            converter.TryGet("font:name:0", out value1);
            converter.TryGet("font:name:1", out value2);
            


            Assert.Equal("Arial", value1);
            Assert.Equal("Tahoma", value2);
        }

        [Fact]
        public void CanOverwriteSettingValue()
        {
            string jsonString = @"
                <settings>
                    <enabledSubscription>false</enabledSubscription>
                </settings>";
            string jsonReplacementString = @"
                <settings>
                    <enabledSubscription>true</enabledSubscription>
                </settings>";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory();
            settingProviderFactory.FileProvider = new MemoryFileProvider(jsonString, "test.xml");

            StaticFileSettingProviderFactory replacementSettingProviderFactory = new StaticFileSettingProviderFactory();
            replacementSettingProviderFactory.FileProvider = new MemoryFileProvider(jsonReplacementString, "test.xml");

            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);
            manager.Accept(replacementSettingProviderFactory);

            var root = manager.Create();

            Assert.Equal("true", root["enabledSubscription"]);
        }

        [Fact]
        public void CanMergeSetting()
        {
            string jsonString = @"
                <settings>
                    <format>
                        <fontFamily>Tahoma</fontFamily>
                        <fontSize>20</fontSize>
                    </format>
                </settings>";
            string jsonReplacementString = @"
                <settings>
                    <enabledSubscription>true</enabledSubscription>
                </settings>";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(jsonString, "test.xml")
            };

            StaticFileSettingProviderFactory replacementSettingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(jsonReplacementString, "test.xml")
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
            string jsonString = @"
                <settings>
                    <format>
                        <fontFamily/>
                    </format>
                </settings>";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(jsonString, "test.xml")
            };

            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var root = manager.Create();

            Assert.Equal("", root["format:fontFamily"]);
        }

        [Fact]
        public void ThrowExceptionForMissingSettingFile()
        {
            string file = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "nonExisting.xml");
            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory(file, false);
            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var exception = Assert.Throws<SettingFileNotFoundException>(() => manager.Create());
            Assert.NotNull(exception);
        }

        [Fact]
        public void ThrowFileLoadException()
        {
            string errorXmlString = @"
                <settings>
                    <format>
                        <fontFamily
                    </format>
                </settings>";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(errorXmlString, "test.xml")
            };

            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var exception = Assert.Throws<SettingFormatException>(() => manager.Create());

            Assert.NotNull(exception);
        }

        [Fact]
        public void DoesNotThrownExceptionOnOptionalSetting()
        {
            string file = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "nonExisting.xml");
            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory(file, true);
            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var root = manager.Create();
        }

        [Fact]
        public void SupportXmlAttribute()
        {
            string jsonString = @"
                <settings>
                    <format>
                        <fontFamily name=""Tahoma""/>
                    </format>
                </settings>";

            StaticFileSettingProviderFactory settingProviderFactory = new StaticFileSettingProviderFactory()
            {
                FileProvider = new MemoryFileProvider(jsonString, "test.xml")
            };

            SettingManager manager = new SettingManager();
            manager.Accept(settingProviderFactory);

            var root = manager.Create();

            Assert.Equal("Tahoma", root["format:fontFamily:@name"]);
        }

        
    }
}

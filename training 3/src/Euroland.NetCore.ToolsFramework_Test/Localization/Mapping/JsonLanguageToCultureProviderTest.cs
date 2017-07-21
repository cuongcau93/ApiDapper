using System;
using System.Collections.Generic;
using System.Text;
using Euroland.NetCore.ToolsFramework.Localization;
using Xunit;
using Moq;
using System.IO;

namespace Euroland.NetCore.ToolsFramework.Test.Localization
{
    public class JsonLanguageToCultureProviderTest
    {
        [Fact]
        public void CanLoadFromPhysicalJsonFile()
        {
            var languageToCultureProvider = new JsonLanguageToCultureProvider(MockObj.GetFile(), MockObj.GetLogger());
            languageToCultureProvider.Load();
            Assert.NotEmpty(languageToCultureProvider.SupportedLanguages);
        }

        [Fact]
        public void CanGetCorrectLanguage2CultureWithSpecifyTwoLetterLanguage()
        {
            var languageToCultureProvider = new JsonLanguageToCultureProvider(MockObj.GetFile(), MockObj.GetLogger());
            languageToCultureProvider.Load();
            var culture2Language = languageToCultureProvider.GetLanguage("en-GB");

            Assert.NotNull(culture2Language);
            Assert.Equal("EN".ToLower(), culture2Language.TwoLetterOfLanguage.ToLower());
            Assert.NotEmpty(culture2Language.SupportedCultures);
        }

        [Fact]
        public void CanGetCorrectLanguage2CultureWithSpecifyTwoLetterLanguageCaseInsensitively()
        {
            
            var languageToCultureProvider = new JsonLanguageToCultureProvider(MockObj.GetFile(), MockObj.GetLogger());
            languageToCultureProvider.Load();
            var culture2Language = languageToCultureProvider.GetLanguage("vi-vn");

            Assert.NotNull(culture2Language);
            Assert.Equal("VI".ToLower(), culture2Language.TwoLetterOfLanguage.ToLower());
            Assert.NotEmpty(culture2Language.SupportedCultures);
        }
    }
}

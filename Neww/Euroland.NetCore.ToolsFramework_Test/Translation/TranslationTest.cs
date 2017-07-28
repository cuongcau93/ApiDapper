using Euroland.NetCore.ToolsFramework.Translation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Euroland.NetCore.ToolsFramework.Test.Translation
{
    public class TranslationTest
    {
        private readonly TranslationService tranService;
        private readonly TranslationCollection translationEnumerator;
        private readonly TranslationGenerator translationGenerator;
        string serverName = "server";
        string databaseName = "Language";
        string userID = "Test";
        string password = "123456";
        string connectionString => $"Server={serverName};Database={databaseName};User ID={userID};Password={password};Trusted_Connection=False;MultipleActiveResultSets=true";

        public TranslationTest()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(host => host.ApplicationName).Returns("Test_Translation_Application");
            hostingEnvironment.Setup(host => host.WebRootPath).Returns(Directory.GetCurrentDirectory());

            var options = new Mock<IOptions<TranslationOptions>>();
            options.Setup(opts => opts.Value).Returns(new TranslationOptions()
            {
                ResourcePath = @"Translations",
                ConnectionString = connectionString
            });

            tranService = new TranslationService(new TranslationDataContext(options.Object));
            translationEnumerator = new TranslationCollection(hostingEnvironment.Object, new TranslationDataContext(options.Object), options.Object);
            translationGenerator = new TranslationGenerator(hostingEnvironment.Object, tranService, new LoggerFactory(), options.Object);
        }

        //[Fact]
        //public void CanReturnTranslationData()
        //{
        //    var data = tranService.GetTranslation(new List<int>() { 1106, 2171, 2172 });
        //    Assert.True(data.Count() > 0);
        //}

        //[Fact]
        //public void CanGenerateTranslationJsonFile()
        //{
        //    bool isGenerated = translationGenerator.Generate();
        //    Assert.True(isGenerated);
        //}

        //[Fact]
        //public async void CanGenerateTranslationJsonFileAsync()
        //{
        //    bool isGenerated = await translationGenerator.GenerateAsync();
        //    Assert.True(isGenerated);
        //}

        //[Fact]
        //public void CanGenerateTranslationJsonFileByLang()
        //{
        //    bool isGenerated = translationGenerator.Generate("en");
        //    Assert.True(isGenerated);
        //}

        //[Fact]
        //public async void CanGenerateTranslationJsonFileByLangAsync()
        //{
        //    bool isGenerated = await translationGenerator.GenerateAsync("de");
        //    Assert.True(isGenerated);
        //}

        [Fact]
        public void CanGenerateTranslationXmlFile()
        {
            var trans = translationEnumerator.EnumerateTranslationInfo();
            Assert.True(trans.Count() > 0);
        }
    }
}

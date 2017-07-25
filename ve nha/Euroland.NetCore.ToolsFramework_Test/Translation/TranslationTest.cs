using Euroland.NetCore.ToolsFramework.Localization;
using Euroland.NetCore.ToolsFramework.Test.Localization;
using Euroland.NetCore.ToolsFramework.Translation;
using Euroland.NetCore.ToolsFramework.Translation.Generator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Euroland.NetCore.ToolsFramework.Test.Translation
{
    public class TranslationTest
    {
        private readonly TranslationService tranService;
        string serverName = "server";
        string databaseName = "Language";
        string userID = "Test";
        string password = "123456";
        string connectionString => $"Server={serverName};Database={databaseName};User ID={userID};Password={password};Trusted_Connection=False;MultipleActiveResultSets=true";
        public TranslationTest()
        {
            tranService = new TranslationService(() => { return "en-GB"; }, connectionString);
        }

        [Fact]
        public void CanGenerateJSONTranslationFile()
        {
            var data = tranService.GetTranslation(new List<int>() { 1106, 2171, 2172 });
            Generate(data);
            Assert.True(data.Count() > 0);
        }

        public void Generate(IEnumerable<ToolsFramework.Translation.Translation> data)
        {
            if (data != null && data.Count() > 0)
            {
                var directory = Path.Combine(TestUtils.CurrentRootDirectory, @"Translations");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                foreach (var item in data)
                {
                    foreach (var file in item.TranslationMap)
                    {
                        string jsonFilePath = Path.Combine(directory, file.Key + ".json");
                        string jsonData = JsonConvert.SerializeObject(file.Value);
                        File.WriteAllText(jsonFilePath, jsonData);
                    }
                }
            }
        }
    }
}

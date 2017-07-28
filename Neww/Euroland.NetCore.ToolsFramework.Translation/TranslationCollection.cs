using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    public class TranslationCollection
    {
        private string _rootDirectory;
        private readonly TranslationService tranService;
        private readonly TranslationOptions _options;
        private IFileProvider _translationFileProvider;
        private readonly IHostingEnvironment _env;

        public TranslationCollection(IHostingEnvironment env, TranslationDataContext tranDbContext, IOptions<TranslationOptions> options)
        {
            _env = env;
            tranService = new TranslationService(tranDbContext);
            _options = options.Value;
            EnsureFileProvider();
        }

        public IEnumerable<TranslationInfo> EnumerateTranslationInfo()
        {
            var xmlFileInfo = _translationFileProvider.GetFileInfo(_options.FileName);
            if (!xmlFileInfo.Exists)
            {
                throw new FileNotFoundException("Not found XML translation file", xmlFileInfo.PhysicalPath);
            }
            using (var stream = xmlFileInfo.CreateReadStream())
            {
                var xDoc = XDocument.Load(stream);
                var elements = xDoc.Root.Elements();
                foreach (var element in elements)
                {
                    if (element.NodeType == XmlNodeType.Comment)
                    {
                        if (element.Value.Trim().StartsWith("There's no translation with the dbid=", StringComparison.OrdinalIgnoreCase))
                        {
                            element.Remove();
                        }
                        continue;
                    }

                    Phrases phrase = null;
                    // * Add "description" attribute to describe the current translation node
                    // * Add "preserve" to support a special case (avoid the generator tool modify "dbid" attribute): 
                    //   the tools are using the Translation talbe in the Shark database. 
                    //   This table, however, is the result of combination a lot of tables
                    //   from Language database (and more :)) while developers work only with HtmlPhrases table
                    //   and generate the translation from the HtmlPhrases table. To avoid redundant translation
                    //   developer will check the translations from the Translation table and get translation's ID
                    //   from there. The problem is, sometimes there are translations which are already available in
                    //   the Translation table, but not in the HtmlPhrases table. Therefore they want to keep the value
                    //   of "dbid" attribute as same as the value which they inserted before.
                    //   
                    if (!element.Attributes().Any(x => x.Name.LocalName.ToLower() == "preserve")
                       || element.Attribute("preserve").Value.ToLower() != "true")
                    {
                        int id = 0;
                        string englishValue = element.Value.Trim();
                        string description = null;

                        if (element.Attributes().Any(x => x.Name.LocalName.ToLower() == "dbid")
                            && !string.IsNullOrEmpty(element.Attribute("dbid").Value))
                            id = int.Parse(element.Attribute("dbid").Value);

                        if (element.Attributes().Any(x => x.Name.LocalName.ToLower() == "description"))
                        {
                            description = element.Attribute("description").Value;
                        }

                        phrase = tranService.InsertOrSelectById(id, englishValue, description);

                        if (phrase != null)
                        {
                            // Found the translation in the table HtmplPhrases of the Language DB 
                            // (already available, or a new insertion)

                            if (element.Attributes().Any(x => x.Name.LocalName.ToLower() == "dbid"))
                                element.Attribute("dbid").Value = phrase.id.ToString();
                            else
                                element.Add(new XAttribute("dbid", phrase.id));

                            element.Value = phrase.EN;
                        }
                        else
                        {
                            // Not found translation in DB, add a note to to xml to show to developer
                            // know that there's no that translation in DB
                            string gNote = "";
                            if (id > 0)
                            {
                                // Developer has supplied the value of "dbid" but this value
                                // was not existing in the DB
                                gNote += "There's no translation with the dbid=\"" + id.ToString() + "\" in the HtmlPhrases table";
                                gNote += "\t\r\nSet dbid's value to empty to insert new translation, or add the attribute preserve=\"true\" to";
                                gNote += "\t\r\ntell generator tool that it should ignore this node";
                            }
                            else
                            {
                                gNote += "Not found in the HtmlPhrases table";
                            }

                            element.AddBeforeSelf(new XComment(gNote));
                            element.Add(
                                new XAttribute("preserve", "true")
                            );
                        }
                    }
                    yield return new TranslationInfo()
                    {
                        PropertyName = element.Attribute("name").Value,
                        Phrases = phrase ?? new Phrases()
                        {
                            EN = element.Value
                        }
                    };

                }
                //Save data to that XML file again
                FileStream writer = new FileStream(xmlFileInfo.PhysicalPath, FileMode.Open, FileAccess.ReadWrite);
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };
                using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                {
                    xDoc.Save(xmlWriter);
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Ensure the file content provider (directory) which can be reservable xml translation, JSON files.
        /// </summary>
        private void EnsureFileProvider()
        {
            if (string.IsNullOrWhiteSpace(_options.ResourcePath))
            {
                throw new ArgumentException("The path of Translation's resource is not valid. The path must be a none-empty relative/absolute path");
            }
            
            if (_options.ResourcePath.Intersect(Path.GetInvalidFileNameChars()).Any()
                || _options.ResourcePath.Intersect(Path.GetInvalidPathChars()).Any())
            {
                throw new IOException($"Path {_options.ResourcePath} contains invalid character(s)");
            }

            string resourceAbsolutePath = _options.ResourcePath;

            if (!Path.IsPathRooted(resourceAbsolutePath))
            {
                resourceAbsolutePath = Path.Combine(_env.WebRootPath, resourceAbsolutePath);
            }

            _translationFileProvider = new PhysicalFileProvider(resourceAbsolutePath);
            _rootDirectory = resourceAbsolutePath;
        }
    }
}

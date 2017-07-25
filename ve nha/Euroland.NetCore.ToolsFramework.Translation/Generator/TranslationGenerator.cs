using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Euroland.NetCore.ToolsFramework.Translation.Generator
{
    /// <summary>
    /// Class do generating content from Translation.xml to resource file "Translation.resx"
    /// </summary>
    public class TranslationGenerator
    {
        private System.Resources.ResourceWriter resourceWritter;
        public Stream TranslationStream { get; private set; }

        /// <summary>
        /// Instantiate an instance of <see cref="TranslationGenerator"/>
        /// </summary>
        /// <param name="translationPath">The path to Translation.xml. Path must be physical file system</param>
        public TranslationGenerator(string translationPath)
            : this(new FileInfo(translationPath))
        {
        }

        /// <summary>
        /// Instantiate an instance of <see cref="TranslationGenerator"/>
        /// </summary>
        /// <param name="fileInfo">
        ///     Object of type <see cref="System.IO.FileSystemInfo"/> 
        ///     contains information of translation file
        /// </param>
        public TranslationGenerator(FileSystemInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");
            if (!fileInfo.Exists)
                throw new System.IO.FileNotFoundException("Not found translation file", fileInfo.FullName);
            //if(System.IO.File.Exists())

        }

        /// <summary>
        /// Instantiate an instance of <see cref="TranslationGenerator"/>
        /// </summary>
        /// <param name="translationStream">Stream of translation</param>
        public TranslationGenerator(Stream translationStream)
        {
            if (translationStream == null)
                throw new ArgumentNullException("translationPath");
            if (!translationStream.CanRead || !translationStream.CanSeek || translationStream.Length == 0)
                throw new InvalidOperationException("Stream must not be empty and be readable");

        }

        /// <summary>
        /// Starts converting and saving translation to *.resx file.
        /// Default file .resx file will be saved to the "bin" folder of project
        /// </summary>
        public void Generate()
        {

        }

        /// <summary>
        /// Starts converting and saving translation to *.resx file.
        /// </summary>
        /// <param name="resourceFilePath">The path of physical file system</param>
        public void Generate(string resourceFilePath)
        {

        }

        /// <summary>
        /// Starts converting and saving translation to *.resx file.
        /// </summary>
        /// <param name="resourceOutputStream">The output stream which the converted content is saved to</param>
        public void Generate(Stream resourceOutputStream)
        {
            //resourceWritter.Generate
        }

        public void Generate(string resourceFilePath, IEnumerable<Translation> data)
        {
            if (string.IsNullOrEmpty(resourceFilePath))
            {
                throw new ArgumentException(resourceFilePath);
            }
            if (data != null && data.Count() > 0)
            {
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllText(resourceFilePath, jsonData);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Translation.Generator
{
    /// <summary>
    /// Class do generating content from Translation.xml to resource file "Translation.resx"
    /// </summary>
    public class TranslationGenerator
    {
        private System.Resources.ResourceWriter resourceWritter;
        public System.IO.Stream TranslationStream { get; private set; }

        /// <summary>
        /// Instantiate an instance of <see cref="TranslationGenerator"/>
        /// </summary>
        /// <param name="translationPath">The path to Translation.xml. Path must be physical file system</param>
        public TranslationGenerator(string translationPath)
            : this(new System.IO.FileInfo(translationPath))
        {
        }

        /// <summary>
        /// Instantiate an instance of <see cref="TranslationGenerator"/>
        /// </summary>
        /// <param name="fileInfo">
        ///     Object of type <see cref="System.IO.FileSystemInfo"/> 
        ///     contains information of translation file
        /// </param>
        public TranslationGenerator(System.IO.FileSystemInfo fileInfo)
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
        public TranslationGenerator(System.IO.Stream translationStream)
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
        public void Generate(System.IO.Stream resourceOutputStream)
        {
            //resourceWritter.Generate
        }
    }
}

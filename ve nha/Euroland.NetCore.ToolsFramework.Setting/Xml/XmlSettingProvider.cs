using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Euroland.NetCore.ToolsFramework.Setting.Xml
{
    /// <summary>
    /// Class to flatten XML structure to a simple <see cref="IDictionary{string, string}"/>
    /// </summary>
    public class XmlSettingProvider : PhysicalFileSettingProvider
    {
        private const string XML_ROOT_ELEMENT = "settings";

        public XmlSettingProvider(PhysicalSettingProviderFactoryBase settingProviderFactory) 
            : base(settingProviderFactory)
        {
        }

        public override void Load(Stream stream)
        {
            //Newtonsoft.Json.JsonConvert.SerializeXmlNode("", Newtonsoft.Json.Formatting.None, true);
            try
            {
                var readerSetting = new XmlReaderSettings()
                {
                    CloseInput = true,
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true,
                    IgnoreWhitespace = true
                };
                using (var xmlReader = XmlReader.Create(stream, readerSetting))
                {
                    System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(xmlReader);
                    this.ParseXElements(doc.Root.Elements(), string.Empty);
                }
            }
            catch (XmlException ex)
            {
                throw new SettingFormatException(
                    string.Format(
                        "There's an error while parsing the XML file. Error on line: {0}, position: {1}",
                        ex.LineNumber,
                        ex.LinePosition
                    ),
                    ex
                );
            }
            
        }

        private void ReadAttribtue(IEnumerable<System.Xml.Linq.XAttribute> attributes, string path)
        {
            foreach (var attr in attributes)
            {
                this.AddToDictionary(attr.Value, SettingFlatterningKeyUtil.Join(path, $"@{attr.Name.LocalName}"));
            }
        }

        private void ParseXElements(IEnumerable<System.Xml.Linq.XElement> elements, string path)
        {
            foreach (var el in elements)
            {
                this.ParseXElement(el, SettingFlatterningKeyUtil.Join(path, el.Name.LocalName));
            }
        }

        private void ParseXElement(System.Xml.Linq.XElement xElement, string path)
        {
            if (xElement.HasAttributes)
            {
                this.ReadAttribtue(xElement.Attributes(), path);
            }

            if(!xElement.HasElements)
            {
                this.AddToDictionary(xElement.Value, path);
            }
            else
            {
                // try to find duplicated elements by name
                var elements = xElement.Elements();

                var singleElements = elements
                    .GroupBy(e => e.Name.LocalName.ToLower())
                    .Where(eGroup => eGroup.Count() == 1)
                    .Select(eGroup => eGroup.First());

                this.ParseXElements(singleElements, path);

                // Group elements which have same name into an array
                var duplicatedElementArray = elements
                    .GroupBy(e => e.Name.LocalName.ToLower())
                    .Where(eGroup => eGroup.Count() > 1)
                    .ToDictionary(gr => gr.Key, gr => gr.ToList(), StringComparer.OrdinalIgnoreCase);


                foreach (var key in duplicatedElementArray.Keys)
                {
                    this.ParseXElementArray(duplicatedElementArray[key], path);
                }
            }
        }

        private void ParseXElementArray(IEnumerable<System.Xml.Linq.XElement> elements, string path)
        {
            int index = 0;
            string currentName = null;
            foreach (var el in elements)
            {
                if(currentName == null || !string.Equals(currentName, el.Name.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    index = 0;
                    currentName = el.Name.LocalName;
                }

                this.ParseXElement(el, SettingFlatterningKeyUtil.Join(path, SettingFlatterningKeyUtil.Join(currentName, index.ToString())));
                

                ++index;
            }
        }

        private void AddToDictionary(string value, string path)
        {
            if (Data.ContainsKey(path))
            {
                throw new SettingFormatException(string.Format("Duplicated key found: {0}", path));
            }

            Data.Add(path, value ?? string.Empty);
        }

        protected override string SerializeData()
        {
            throw new NotImplementedException();
        }
    }
}

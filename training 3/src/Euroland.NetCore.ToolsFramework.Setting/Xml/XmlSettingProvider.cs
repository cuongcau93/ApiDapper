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
        private readonly Stack<String> _paths;
        private string _currentPath;

        public XmlSettingProvider(PhysicalSettingProviderFactoryBase settingProviderFactory) 
            : base(settingProviderFactory)
        {
            _paths = new Stack<string>();
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
                    foreach (var elm in doc.Root.Elements())
                    {
                        this.AddPath(elm.Name.LocalName);
                        this.ParseXElement(elm);
                        this.ReleasePath();
                    }
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

        private void ParseXElement(System.Xml.Linq.XElement xElement)
        {
            if (xElement.HasAttributes)
            {
                foreach (var attr in xElement.Attributes())
                {
                    this.AddPath(string.Format("@{0}", attr.Name.LocalName));
                    this.ReadTextNode(attr.Value);
                    this.ReleasePath();
                }
            }

            if(!xElement.HasElements)
            {
                this.ReadTextNode(xElement.Value);
            }
            else
            {
                // try to find duplicated elements by name
                var elements = xElement.Elements();

                // Group elements which have same name into an array
                var duplicatedElementArray = elements
                    .GroupBy(e => e.Name.LocalName.ToLower())
                    .Where(eGroup => eGroup.Count() > 1)
                    .ToDictionary(gr => gr.Key, gr => gr.ToList(), StringComparer.OrdinalIgnoreCase);

                var singleElements = elements
                    .GroupBy(e => e.Name.LocalName.ToLower())
                    .Where(eGroup => eGroup.Count() == 1)
                    .Select(eGroup => eGroup.First());

                foreach (var el in singleElements)
                {
                    this.AddPath(el.Name.LocalName);
                    this.ParseXElement(el);
                    this.ReleasePath();
                }

                foreach (var key in duplicatedElementArray.Keys)
                {
                    this.AddPath(key);
                    this.ParseXElementArray(duplicatedElementArray[key]);
                    this.ReleasePath();
                }
            }
        }

        private void ParseXElementArray(IEnumerable<System.Xml.Linq.XElement> elements)
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

                this.AddPath(index.ToString(System.Globalization.CultureInfo.InvariantCulture));
                this.ParseXElement(el);
                this.ReleasePath();

                ++index;
            }
        }

        private void ReadTextNode(string value)
        {
            if (Data.ContainsKey(_currentPath))
            {
                throw new SettingFormatException(string.Format("Duplicated key found: {0}", _currentPath));
            }

            Data.Add(_currentPath, value ?? string.Empty);
        }

        private void AddPath(string path)
        {
            _paths.Push(path);
            _currentPath = SettingFlatterningKeyUtil.Combine(_paths.Reverse());
        }

        private void ReleasePath()
        {
            _paths.Pop();
            _currentPath = SettingFlatterningKeyUtil.Combine(_paths.Reverse());
        }

        protected override string SerializeData()
        {
            throw new NotImplementedException();
        }
    }
}

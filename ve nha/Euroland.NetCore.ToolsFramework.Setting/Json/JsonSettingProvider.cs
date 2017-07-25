using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Euroland.NetCore.ToolsFramework.Setting.Json
{
    /// <summary>
    /// Class to flatten json structure to a simple <see cref="IDictionary{string, string}"/>
    /// Found an article flatterning json structure at http://www.bfcamara.com/post/75172803617/flatten-json-object-to-send-within-an-azure-hub
    /// </summary>
    public class JsonSettingProvider : PhysicalFileSettingProvider
    {
        private readonly Stack<string> _paths;
        private JsonTextReader _jsonReader;

        public JsonSettingProvider(PhysicalSettingProviderFactoryBase settingProviderFactory) 
            : base(settingProviderFactory)
        {
        }

        public override void Load(Stream stream)
        {
            try
            {
                using (var strReader = new StreamReader(stream))
                {
                    _jsonReader = new JsonTextReader(strReader);
                    _jsonReader.DateParseHandling = DateParseHandling.None;

                    var jObject = JObject.Load(_jsonReader);
                    this.ReadJObject(jObject, string.Empty);
                }
            }
            catch (JsonReaderException ex)
            {
                throw new SettingFormatException(
                    string.Format(
                        "There's an error while parsing the JSON file. Error on line: {0}, position: {1}, path: {2}", 
                        ex.LineNumber, 
                        ex.LinePosition, 
                        ex.Path),
                    ex
                );
            }
        }

        private void ReadJObject(JObject obj, string prefix)
        {
            foreach (var prop in obj.Properties())
            {
                this.ReadJToken(prop.Value, SettingFlatterningKeyUtil.Join(prefix, prop.Name));
            }
        }

        private void ReadJArray(JArray arr, string path)
        {
            int index = 0;
            foreach (var item in arr)
            {
                this.ReadJToken(item, SettingFlatterningKeyUtil.Join(path, index.ToString()));
                ++index;
            }
        }

        private void ReadJToken(JToken token, string path)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    this.ReadJObject(token.Value<JObject>(), path);
                    break;
                case JTokenType.Array:
                    this.ReadJArray(token.Value<JArray>(), path);
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                    this.AddToDictionary(token.Value<JValue>(), path);
                    break;
                default:
                    throw new SettingFormatException(
                        string.Format(
                            "Found unspported JSON token: {0}, Path {1}, line {0}, position {3}",
                            _jsonReader.TokenType,
                            _jsonReader.Path,
                            _jsonReader.LineNumber,
                            _jsonReader.LinePosition
                        )
                    );
            }
        }

        private void AddToDictionary(JValue value, string path)
        {
            if(Data.ContainsKey(path))
            {
                throw new SettingFormatException(string.Format("Duplicated key found: {0}", path));
            }

            Data.Add(path, value != null ? value.ToString(System.Globalization.CultureInfo.InvariantCulture) : string.Empty);
        }

        protected override string SerializeData()
        {
            var keys = this.Data.Keys.OrderBy(k => k.ToLower());
            System.Text.StringBuilder jsonString = new System.Text.StringBuilder();

            using (var strWriter = new StringWriter(jsonString, System.Globalization.CultureInfo.InvariantCulture))
            using (var jsonWriter = new JsonTextWriter(strWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.WriteStartObject();
                {
                    foreach (var path in keys)
                    {

                    }
                }
                jsonWriter.WriteEndObject();
            }

            return jsonString.ToString();
        }

        private bool IsPlainValue(string path)
        {
            return path.LastIndexOf(SettingFlatterningKeyUtil.NameDelimiter) == -1;
        }

        private bool IsArrayValue(string path)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(path, @".+[:]\d+$");
        }
    }
}

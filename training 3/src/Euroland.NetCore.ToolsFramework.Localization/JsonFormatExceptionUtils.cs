using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    public class JsonFormatExceptionUtils
    {
        public static FormatException GetException(Newtonsoft.Json.JsonReaderException innerException, Stream jsonContentStream)
        {
            var error = string.Empty;
            if (jsonContentStream.CanSeek)
            {
                jsonContentStream.Seek(0, SeekOrigin.Begin);
                IEnumerable<string> lines = null;
                using (var strReader = new StreamReader(jsonContentStream))
                {
                    lines = ReadLine(strReader);
                    error = RetrieveErrorLine(innerException, lines);
                }
            }

            return new FormatException(
                string.Format("Could not parse the JSON file. Error on line number '{0}': '{1}'",
                    innerException.LineNumber,
                    error), 
                innerException);
        }

        private static string RetrieveErrorLine(Newtonsoft.Json.JsonReaderException innerException, IEnumerable<string> jsonLines)
        {
            string errorLine = string.Empty;
            if(innerException.LineNumber > 2)
            {
                var error = jsonLines.Skip(innerException.LineNumber - 2).Take(2).ToList();
                if (error.Count() >= 2)
                {
                    errorLine = error[0].Trim() + Environment.NewLine + error[1].Trim();
                }
            }
            if (string.IsNullOrEmpty(errorLine))
            {
                var possibleLineContent = jsonLines.Skip(innerException.LineNumber - 1).FirstOrDefault();
                errorLine = possibleLineContent ?? string.Empty;
            }

            return errorLine;
        }

        private static IEnumerable<string> ReadLine(StreamReader strReader)
        {
            string line;
            do
            {
                line = strReader.ReadLine();
                yield return line;
            } while (line != null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    public class XmlLanguageToCultureProvider : ILanguageToCultureProvider
    {
        public IEnumerable<string> SupportedLanguages => throw new NotImplementedException();

        public IEnumerable<CultureInfo> AllSupportedCultures => throw new NotImplementedException();

        public IEnumerable<CultureInfo> GetCultures(string twoLetterLanguage)
        {
            throw new NotImplementedException();
        }

        public LanguageToCulture GetLanguage(string cultureCode)
        {
            throw new NotImplementedException();
        }

        public LanguageToCulture GetLanguage(CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}

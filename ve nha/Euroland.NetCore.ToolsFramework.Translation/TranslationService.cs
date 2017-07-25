using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    public interface ITranslationService
    {
        /// <summary>
        /// Gets translation for the language of user by the phrase id.
        /// </summary>
        /// <param name="id">The phrase id.</param>
        /// <remarks>The method returns <i>null</i>' never.</remarks>
        /// <returns>Phrase translation</returns>
        IEnumerable<Translation> GetTranslation(List<int> ids);

        /// <summary>
        /// Get a translation with specified phrase name and language name
        /// </summary>
        /// <param name="phraseName"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        Translation GetTranslation(string phraseName, string lang);

        /// <summary>
        /// Get a translation with specified phrase name
        /// </summary>
        /// <param name="phraseName"></param>
        /// <returns></returns>
        Translation GetTranslation(string phraseName);

    }

    /// <summary>
    /// Class to read translation from the database
    /// </summary>
    public class TranslationService : ITranslationService
    {
        private TranslationDataContext tranDbContext;
        private Func<string> languageSelected;
        public TranslationService(Func<string> getLanguageSelected, string connectionString)
        {
            tranDbContext = new TranslationDataContext(connectionString);
            languageSelected = getLanguageSelected;
        }

        public IEnumerable<Translation> GetTranslation(List<int> ids)
        {
            IEnumerable<Translation> trans = null;
            if (ids.Count > 0)
            {
                var translationIds = string.Join<int>(",", ids);
                trans = tranDbContext.LoadTranslations("spTranslationSelectByIDs", new { IDs = translationIds });
            }
            return trans;
        }

        public Translation GetTranslation(string phraseName, string lang)
        {
            Translation tran = null;
            IEnumerable<Translation> trans = tranDbContext.LoadTranslations("spPhrasesSelectByIdLangTranslation", new { Phrase = phraseName, Lang = lang });
            if (trans != null)
            {
                tran = trans.FirstOrDefault();
            }
            return tran;
        }

        public Translation GetTranslation(string phraseName)
        {
            return GetTranslation(phraseName, languageSelected());
        }
    }
}

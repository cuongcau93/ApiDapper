using System;
using System.Collections.Generic;
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
        IEnumerable<Translation> GetTranslation(IEnumerable<int> ids);

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
}

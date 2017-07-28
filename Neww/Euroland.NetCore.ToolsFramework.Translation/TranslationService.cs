using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    /// <summary>
    /// Class to read translation from the database
    /// </summary>
    public class TranslationService : ITranslationService
    {
        private readonly ITranslationDataContext _context;
        private const string TOOLS_CACHE_TRANSLATION_BY_ID_PREFIX = "Cache.Translation.ById";
        private const string TOOLS_CACHE_TRANSLATION_BY_PHRASE_LANG = "Cache.Translation.ByPhraseLang";
        private Dictionary<string, Translation> translationCache = new Dictionary<string, Translation>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="getLanguageSelected"></param>
        public TranslationService(ITranslationDataContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        /// <summary>
        /// Get translation by list of translation id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<Translation> GetTranslation(IEnumerable<int> ids)
        {
            List<Translation> trans = new List<Translation>();
            if (ids.Count() > 0)
            {
                List<int> translationIds = new List<int>();
                foreach (var id in ids)
                {
                    Translation tran = GetTranslationFromCache(id);
                    if (tran == null)
                    {
                        translationIds.Add(id);
                    }
                    else
                    {
                        trans.Add(tran);
                    }
                }
                if (translationIds.Count > 0)
                {
                    var transIds = string.Join<int>(",", translationIds);
                    IEnumerable<Translation> misTrans = _context.LoadTranslations("spTranslationSelectByIDs", new { IDs = transIds });
                    if (misTrans != null)
                    {
                        foreach (var tran in misTrans)
                        {
                            AddTranslationToCache(tran, tran.Id);
                            trans.Add(tran);
                        }
                    }
                }
            }

            return trans;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phraseName"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public Translation GetTranslation(string phraseName, string lang)
        {
            Translation tran = new Translation();
            string cacheKey = $"{TOOLS_CACHE_TRANSLATION_BY_PHRASE_LANG}.{phraseName}";
            if (translationCache.ContainsKey(cacheKey))
            {
                tran = translationCache[cacheKey];
            }
            else
            {
                IEnumerable<Translation> trans = _context.LoadTranslations("spPhrasesSelectByIdLangTranslation", new { Phrase = phraseName, Lang = lang });
                if (trans != null)
                {
                    tran = trans.FirstOrDefault();
                    translationCache.Add(cacheKey, tran);
                }
            }
            return tran;
        }

        /// <summary>
        /// Get Translation by phrase name
        /// </summary>
        /// <param name="phraseName"></param>
        /// <returns></returns>
        public Translation GetTranslation(string phraseName)
        {
            return GetTranslation(phraseName, null);
        }

        /// <summary>
        /// Select Phrases from database if that phrases is existed
        /// on the other hand Insert Phrases to databases if it is not exist in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phraseEnglish"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public Phrases InsertOrSelectById(int id, string phraseEnglish, string description)
        {
            return _context.ExecSingle<Phrases>("spPhrasesInsertNotExistSelectById", new { id = id, EnPhrase = phraseEnglish, Description = description });
        }

        /// <summary>
        /// Set Translation to cache by translation id
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="tranId"></param>
        private void AddTranslationToCache(Translation tran, int tranId)
        {
            if (tran == null)
                return;

            string cacheKey = GetCacheIdByTransId(tranId);
            translationCache.Add(cacheKey, tran);
        }

        /// <summary>
        /// Get Translation from cache by translation id
        /// </summary>
        /// <param name="tranId"></param>
        /// <returns></returns>
        private Translation GetTranslationFromCache(int tranId)
        {
            string cacheKey = GetCacheIdByTransId(tranId);
            if (translationCache != null && translationCache.ContainsKey(cacheKey))
            {
                return translationCache[cacheKey] as Translation;
            }

            return null;
        }

        /// <summary>
        /// Create a cache key by translation id
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        private string GetCacheIdByTransId(int translationId)
        {
            return $"{TOOLS_CACHE_TRANSLATION_BY_ID_PREFIX}.{translationId}";
        }
    }
}

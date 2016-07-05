using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Collections;


namespace CAF.WebSite.Application.Services.Localization
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY = "CrAnt.FrameWork.language.all-{0}";
        private const string LANGUAGES_COUNT = "CrAnt.FrameWork.language.count-{0}";
        private const string LANGUAGES_BY_CULTURE_KEY = "CrAnt.FrameWork.language.culture-{0}";
        private const string LANGUAGES_BY_SEOCODE_KEY = "CrAnt.FrameWork.language.seocode-{0}";
        private const string LANGUAGES_PATTERN_KEY = "CrAnt.FrameWork.language.";
        private const string LANGUAGES_BY_ID_KEY = "CrAnt.FrameWork.language.id-{0}";
        #endregion

        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISiteService _siteService;
        private readonly ISiteContext _siteContext;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageRepository">Language repository</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LanguageService(ICacheManager cacheManager,
            IRepository<Language> languageRepository,
          ISiteService storeService,
            ISiteContext storeContext,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._languageRepository = languageRepository;
            this._siteService = storeService;
            this._siteContext = storeContext;
            this._eventPublisher = eventPublisher;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void DeleteLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");




            _languageRepository.Delete(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(language);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="siteId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Language collection</returns>
        public virtual IList<Language> GetAllLanguages(bool showHidden = false, int siteId = 0)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            var languages = _cacheManager.Get(key, () =>
            {
                var query = _languageRepository.Table;
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder);
                return query.ToList();
            });

            //store mapping

            return languages;
        }

        /// <summary>
        /// Gets languages count
        /// </summary>
        /// <param name="showHidden">A value indicating whether to consider hidden records</param>
        /// <returns>The count of Languages</returns>
        public virtual int GetLanguagesCount(bool showHidden = false)
        {
            string key = string.Format(LANGUAGES_COUNT, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _languageRepository.Table;
                if (!showHidden)
                    query = query.Where(l => l.Published);
                return query.Select(x => x.Id).Count();
            });
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public virtual Language GetLanguageById(int languageId)
        {
            if (languageId == 0)
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return _cacheManager.Get(key, () =>
            {
                return _languageRepository.GetById(languageId);
            });
        }

        /// <summary>
        /// Gets a language by culture code (e.g.: en-US)
        /// </summary>
        /// <param name="culture">Culture code</param>
        /// <returns>Language</returns>
        /// <![CDATA[ codehint: sm-add ]]>
        public virtual Language GetLanguageByCulture(string culture)
        {
            if (!culture.HasValue())
                return null;

            string key = string.Format(LANGUAGES_BY_CULTURE_KEY, culture);
            return _cacheManager.Get(key, () =>
            {
                return _languageRepository.Table.Where(x => culture.Equals(x.LanguageCulture, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            });
        }

        public virtual Language GetLanguageBySeoCode(string seoCode)
        {
            if (!seoCode.HasValue())
                return null;

            string key = string.Format(LANGUAGES_BY_SEOCODE_KEY, seoCode);
            return _cacheManager.Get(key, () =>
            {
                return _languageRepository.Table.Where(x => seoCode.Equals(x.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            });
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Insert(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(language);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            //update language
            _languageRepository.Update(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(language);
        }


        public virtual bool IsPublishedLanguage(string seoCode, int storeId = 0)
        {
            if (storeId <= 0)
                storeId = _siteContext.CurrentSite.Id;

            var map = this.GetSiteLanguageMap();
            if (map.ContainsKey(storeId))
            {
                return map[storeId].Any(x => x.Item2 == seoCode);
            }

            return false;
        }

        public virtual bool IsPublishedLanguage(int languageId, int storeId = 0)
        {
            if (languageId <= 0)
                return false;

            if (storeId <= 0)
                storeId = _siteContext.CurrentSite.Id;

            var map = this.GetSiteLanguageMap();
            if (map.ContainsKey(storeId))
            {
                return map[storeId].Any(x => x.Item1 == languageId);
            }

            return false;
        }

        public virtual string GetDefaultLanguageSeoCode(int storeId = 0)
        {
            if (storeId <= 0)
                storeId = _siteContext.CurrentSite.Id;

            var map = this.GetSiteLanguageMap();
            if (map.ContainsKey(storeId))
            {
                return map[storeId].FirstOrDefault().Item2;
            }

            return null;
        }

        public virtual int GetDefaultLanguageId(int storeId = 0)
        {
            if (storeId <= 0)
                storeId = _siteContext.CurrentSite.Id;

            var map = this.GetSiteLanguageMap();
            if (map.ContainsKey(storeId))
            {
                return map[storeId].FirstOrDefault().Item1;
            }

            return 0;
        }

        /// <summary>
        /// Gets a map of active/published store languages
        /// </summary>
        /// <returns>A map of store languages where key is the store id and values are tuples of language ids and seo codes</returns>
        protected virtual Multimap<int, Tuple<int, string>> GetSiteLanguageMap()
        {
            var result = _cacheManager.Get(ServiceCacheConsumer.SITE_LANGUAGE_MAP_KEY, () =>
            {
                var map = new Multimap<int, Tuple<int, string>>();

                var allSites = _siteService.GetAllSites();
                foreach (var store in allSites)
                {
                    var languages = GetAllLanguages(false, store.Id);
                    if (!languages.Any())
                    {
                        // language-less stores aren't allowed but could exist accidentally. Correct this.
                        var firstSiteLang = GetAllLanguages(true, store.Id).FirstOrDefault();
                        if (firstSiteLang == null)
                        {
                            // absolute fallback
                            firstSiteLang = GetAllLanguages(true).FirstOrDefault();
                        }
                        map.Add(store.Id, new Tuple<int, string>(firstSiteLang.Id, firstSiteLang.UniqueSeoCode));
                    }
                    else
                    {
                        foreach (var lang in languages)
                        {
                            map.Add(store.Id, new Tuple<int, string>(lang.Id, lang.UniqueSeoCode));
                        }
                    }
                }

                return map;
            }, 1440 /* 24 hrs */);

            return result;
        }
        #endregion
    }
}

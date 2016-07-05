using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Article tag service
    /// </summary>
    public partial class ArticleTagService : IArticleTagService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string ARTICLETAG_COUNT_KEY = "caf.articletag.count-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ARTICLETAG_PATTERN_KEY = "caf.articletag.";

        #endregion

        #region Fields

        private readonly IRepository<ArticleTag> _articleTagRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="articleTagRepository">Article tag repository</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event published</param>
        public ArticleTagService(IRepository<ArticleTag> articleTagRepository,
            IDataProvider dataProvider,
            IDbContext dbContext,
            CommonSettings commonSettings,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            _articleTagRepository = articleTagRepository;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _commonSettings = commonSettings;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Nested classes

        private class ArticleTagWithCount
        {
            public int ArticleTagId { get; set; }
            public int ArticleCount { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get article count for each of existing article tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Dictionary of "article tag ID : article count"</returns>
        private Dictionary<int, int> GetArticleCount(int storeId)
        {
            string key = string.Format(ARTICLETAG_COUNT_KEY, storeId);
            return _cacheManager.Get(key, () =>
            {

                if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduresSupported)
                {
                    //stored procedures are enabled and supported by the database. 
                    //It's much faster than the LINQ implementation below 

                    #region Use stored procedure

                    //prepare parameters
                    var pSiteId = _dataProvider.GetParameter();
                    pSiteId.ParameterName = "SiteId";
                    pSiteId.Value = storeId;
                    pSiteId.DbType = DbType.Int32;


                    //invoke stored procedure
                    var result = _dbContext.SqlQuery<ArticleTagWithCount>(
                        "Exec ArticleTagCountLoadAll @SiteId",
                        pSiteId);

                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in result)
                        dictionary.Add(item.ArticleTagId, item.ArticleCount);
                    return dictionary;

                    #endregion
                }
                else
                {
                    //stored procedures aren't supported. Use LINQ
                    #region Search articles
                    var query = from pt in _articleTagRepository.Table
                                select new
                                {
                                    Id = pt.Id,
                                    ArticleCount = pt.Articles
                                        //published and not deleted article/variants
                                        .Count(p => !p.Deleted && p.StatusFormat == StatusFormat.Norma)
                                    //UNDOEN filter by store identifier if specified ( > 0 )
                                };

                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in query)
                        dictionary.Add(item.Id, item.ArticleCount);
                    return dictionary;

                    #endregion

                }
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        public virtual void DeleteArticleTag(ArticleTag articleTag)
        {
            if (articleTag == null)
                throw new ArgumentNullException("articleTag");

            _articleTagRepository.Delete(articleTag);

            //cache
            _cacheManager.RemoveByPattern(ARTICLETAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(articleTag);
        }

        /// <summary>
        /// Gets all article tags
        /// </summary>
        /// <returns>Article tags</returns>
        public virtual IList<ArticleTag> GetAllArticleTags()
        {
            var query = _articleTagRepository.Table;
            var articleTags = query.ToList();
            return articleTags;
        }

        /// <summary>
        /// Gets all article tag names
        /// </summary>
        /// <returns>Article tag names as list</returns>
        public virtual IList<string> GetAllArticleTagNames()
        {
            var query = from pt in _articleTagRepository.Table
                        orderby pt.Name ascending
                        select pt.Name;
            return query.ToList();
        }

        /// <summary>
        /// Gets article tag
        /// </summary>
        /// <param name="articleTagId">Article tag identifier</param>
        /// <returns>Article tag</returns>
        public virtual ArticleTag GetArticleTagById(int articleTagId)
        {
            if (articleTagId == 0)
                return null;

            var articleTag = _articleTagRepository.GetById(articleTagId);
            return articleTag;
        }

        /// <summary>
        /// Gets article tag by name
        /// </summary>
        /// <param name="name">Article tag name</param>
        /// <returns>Article tag</returns>
        public virtual ArticleTag GetArticleTagByName(string name)
        {
            var query = from pt in _articleTagRepository.Table
                        where pt.Name == name
                        select pt;

            var articleTag = query.FirstOrDefault();
            return articleTag;
        }

        /// <summary>
        /// Inserts a article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        public virtual void InsertArticleTag(ArticleTag articleTag)
        {
            if (articleTag == null)
                throw new ArgumentNullException("articleTag");

            _articleTagRepository.Insert(articleTag);

            //cache
            _cacheManager.RemoveByPattern(ARTICLETAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(articleTag);
        }

        /// <summary>
        /// Updates the article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        public virtual void UpdateArticleTag(ArticleTag articleTag)
        {
            if (articleTag == null)
                throw new ArgumentNullException("articleTag");

            _articleTagRepository.Update(articleTag);

            //cache
            _cacheManager.RemoveByPattern(ARTICLETAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(articleTag);
        }

        /// <summary>
        /// Get number of articles
        /// </summary>
        /// <param name="articleTagId">Article tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Number of articles</returns>
        public virtual int GetArticleCount(int articleTagId, int storeId)
        {
            var dictionary = GetArticleCount(storeId);
            if (dictionary.ContainsKey(articleTagId))
                return dictionary[articleTagId];
            else
                return 0;
        }

        #endregion
    }
}

using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using CAF.Infrastructure.Core.Domain.Cms.Media;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// ArticleCategory service
    /// </summary>
    public partial class ArticleService : IArticleService
    {
        #region Constants

        private const string ARTICLES_BY_ID_PN_KEY = "caf.article.id-{0}-{1}";
        private const string ARTICLES_BY_ID_KEY = "caf.article.id-{0}";
        private const string ARTICLES_PATTERN_KEY = "caf.article.";

        #endregion

        #region Fields
        private readonly IRepository<RelatedArticle> _relatedArticleRepository;
        private readonly IRepository<ArticleCategory> _categoryRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<Picture> _pictureRepository;
        private readonly IRepository<ArticleAlbum> _articleAlbumRepository;
        private readonly IRepository<ArticleAttribute> _articleAttributeRepository;
        private readonly IRepository<SiteMapping> _siteMappingRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly ILanguageService _languageService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="categoryRepository">ArticleCategory repository</param>
        /// <param name="ArticleArticleCategoryRepository">ArticleArticleCategory repository</param>
        /// <param name="ArticleRepository">Article repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="siteMappingRepository">Site mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="siteContext">Site context</param>
        /// <param name="eventPublisher">Event publisher</param>
        public ArticleService(ICacheManager cacheManager,
              IRepository<RelatedArticle> relatedArticleRepository,
            IRepository<ArticleCategory> categoryRepository,
            IRepository<Article> articleRepository,
            IRepository<Picture> pictureRepository,
            IRepository<ArticleAlbum> articleAlbumRepository,
            IRepository<ArticleAttribute> articleAttributeRepository,
            IRepository<SiteMapping> siteMappingRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<AclRecord> aclRepository,
            ILanguageService languageService,
            IWorkflowMessageService workflowMessageService,
            IDataProvider dataProvider, IDbContext dbContext,
            IWorkContext workContext,
            ISiteContext siteContext,
            LocalizationSettings localizationSettings, CommonSettings commonSettings,
            IEventPublisher eventPublisher)
        {
            this._relatedArticleRepository = relatedArticleRepository;
            this._cacheManager = cacheManager;
            this._categoryRepository = categoryRepository;
            this._articleRepository = articleRepository;
            this._pictureRepository = pictureRepository;
            this._articleAlbumRepository = articleAlbumRepository;
            this._articleAttributeRepository = articleAttributeRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._aclRepository = aclRepository;
            this._siteMappingRepository = siteMappingRepository;
            this._languageService = languageService;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._eventPublisher = eventPublisher;
            this._commonSettings = commonSettings;
            this.QuerySettings = DbQuerySettings.Default;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._localizationSettings = localizationSettings;
            this._workflowMessageService = workflowMessageService;
        }

        public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Utilities

        protected virtual int EnsureMutuallyRelatedArticles(List<int> articleIds)
        {
            int count = 0;

            foreach (int id1 in articleIds)
            {
                var mutualAssociations = (
                    from rp in _relatedArticleRepository.Table
                    join p in _articleRepository.Table on rp.ArticleId2 equals p.Id
                    where !p.Deleted && rp.ArticleId2 == id1
                    select rp).ToList();

                foreach (int id2 in articleIds)
                {
                    if (id1 == id2)
                        continue;

                    if (!mutualAssociations.Any(x => x.ArticleId1 == id2))
                    {
                        int maxDisplayOrder = _relatedArticleRepository.TableUntracked
                            .Where(x => x.ArticleId1 == id2)
                            .OrderByDescending(x => x.DisplayOrder)
                            .Select(x => x.DisplayOrder)
                            .FirstOrDefault();

                        var newRelatedArticle = new RelatedArticle
                        {
                            ArticleId1 = id2,
                            ArticleId2 = id1,
                            DisplayOrder = maxDisplayOrder + 1
                        };

                        InsertRelatedArticle(newRelatedArticle);
                        ++count;
                    }
                }
            }

            return count;
        }


        #endregion

        #region Methods

        #region Articles
        /// <summary>
        /// Delete a Article
        /// </summary>
        /// <param name="Article">Article</param>
        public virtual void DeleteArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("Article");

            //article.Deleted = true;
            //delete Article
            // UpdateArticle(article);
            DeleteArticleScope(article);

            //event notification
            _eventPublisher.EntityDeleted(article);
        }
        public virtual int DeleteArticleScope(Article article)
        {
            var ctx = _articleRepository.Context;

            using (var scope = new DbContextScope(ctx: ctx, autoDetectChanges: false, proxyCreation: true, validateOnSave: false, forceNoTracking: true))
            {

                var arAutoCommit = _articleRepository.AutoCommitEnabled;
                var abAutoCommit = _articleAlbumRepository.AutoCommitEnabled;
                var pcAutoCommit = _pictureRepository.AutoCommitEnabled;
                var aaAutoCommit = _articleAttributeRepository.AutoCommitEnabled;
                _articleRepository.AutoCommitEnabled = false;
                _articleAlbumRepository.AutoCommitEnabled = false;
                _pictureRepository.AutoCommitEnabled = false;
                _articleAttributeRepository.AutoCommitEnabled = false;

                int numberOfDeletedUsers = 0;
                try
                {


                    var query = this._articleAlbumRepository.Table;
                    query = query.Where(c => c.ArticleId == article.Id);

                    var articleAlbum = query.ToList();
                    foreach (var ab in articleAlbum)
                    {
                        this._articleAlbumRepository.Delete(ab);
                    }

                    var queryAtt = this._articleAttributeRepository.Table;
                    queryAtt = queryAtt.Where(c => c.EntityId == article.Id);

                    var articleAtt = queryAtt.ToList();
                    foreach (var att in articleAtt)
                    {
                        this._articleAttributeRepository.Delete(att);
                    }

                    this._articleRepository.Delete(article);

                    if (article.PictureId.HasValue)
                        this._pictureRepository.Delete(article.PictureId.Value);
                    try
                    {
                        scope.Commit();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }


                // save the rest
                scope.Commit();

                _articleRepository.AutoCommitEnabled = arAutoCommit;
                _articleRepository.AutoCommitEnabled = abAutoCommit;
                _pictureRepository.AutoCommitEnabled = pcAutoCommit;
                _articleAttributeRepository.AutoCommitEnabled = aaAutoCommit;
                return numberOfDeletedUsers;
            }
        }

        /// <summary>
        /// Gets all Articles displayed on the home page
        /// </summary>
        /// <returns>Article collection</returns>
        public virtual IList<Article> GetAllArticlesDisplayedOnHomePage()
        {
            var query = from p in _articleRepository.Table
                        orderby p.Title
                        where p.StatusFormat == StatusFormat.Norma &&
                        !p.Deleted
                        select p;
            var Articles = query.ToList();
            return Articles;
        }
        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>Topics</returns>
        public virtual IList<Article> GetAllArticles()
        {
            var query = _articleRepository.Table;
            query = query.OrderBy(t => t.CreatedOnUtc).ThenBy(t => t.DisplayOrder);

            return query.ToList();
        }
        /// <summary>
        /// Gets all blog posts
        /// </summary>
        /// <param name="siteId">The site identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
        /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Blog posts</returns>
        public virtual IPagedList<Article> GetAllArticles(int siteId,
            DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _articleRepository.Table;
            if (dateFrom.HasValue)
                query = query.Where(b => dateFrom.Value <= b.CreatedOnUtc);
            if (dateTo.HasValue)
                query = query.Where(b => dateTo.Value >= b.CreatedOnUtc);

            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(b => !b.StartDateUtc.HasValue || b.StartDateUtc <= utcNow);
                query = query.Where(b => !b.EndDateUtc.HasValue || b.EndDateUtc >= utcNow);
            }

            if (siteId > 0 && !QuerySettings.IgnoreMultiSite)
            {
                //Site mapping
                query = from bp in query
                        join sm in _siteMappingRepository.Table
                        on new { c1 = bp.Id, c2 = "Article" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                        from sm in bp_sm.DefaultIfEmpty()
                        where !bp.LimitedToSites || siteId == sm.SiteId
                        select bp;

                //only distinct blog posts (group by ID)
                query = from bp in query
                        group bp by bp.Id into bpGroup
                        orderby bpGroup.Key
                        select bpGroup.FirstOrDefault();
            }

            query = query.OrderByDescending(b => b.CreatedOnUtc);

            var articles = new PagedList<Article>(query, pageIndex, pageSize);
            return articles;
        }

        /// <summary>
        /// Gets Article
        /// </summary>
        /// <param name="ArticleId">Article identifier</param>
        /// <returns>Article</returns>
        public virtual Article GetArticleById(int articleId)
        {
            if (articleId == 0)
                return null;

            string key = string.Format(ARTICLES_BY_ID_KEY, articleId);
            return _cacheManager.Get(key, () =>
            {
                return _articleRepository.GetById(articleId);
            });
        }

        public virtual Article GetPreArticleById(int articleId, int categoryId)
        {
            if (articleId == 0)
                return null;

            string key = string.Format(ARTICLES_BY_ID_PN_KEY, articleId, "Prev");
            return _cacheManager.Get(key, () =>
            {
                var prev = _articleRepository.Table.Where(p => p.Id < articleId && p.CategoryId == categoryId).OrderBy(p => p.Id).Take(1).ToList();
                return (prev.Count > 0) ? prev.ElementAt(0) : null;
            });
        }
        public virtual Article GetNextArticleById(int articleId, int categoryId)
        {
            if (articleId == 0)
                return null;

            string key = string.Format(ARTICLES_BY_ID_PN_KEY, articleId, "Next");
            return _cacheManager.Get(key, () =>
            {
                var next = _articleRepository.Table.Where(p => p.Id > articleId && p.CategoryId == categoryId).OrderBy(p => p.Id).Take(1).ToList();
                return (next.Count > 0) ? next.ElementAt(0) : null;
            });
        }
        /// <summary>
        /// Get Articles by identifiers
        /// </summary>
        /// <param name="ArticleIds">Article identifiers</param>
        /// <returns>Articles</returns>
        public virtual IList<Article> GetArticlesByIds(int[] articleIds)
        {
            if (articleIds == null || articleIds.Length == 0)
                return new List<Article>();

            var query = from p in _articleRepository.Table
                        where articleIds.Contains(p.Id)
                        select p;
            var Articles = query.ToList();

            //sort by passed identifiers
            var sortedArticles = new List<Article>();

            foreach (int id in articleIds)
            {
                var Article = Articles.Find(x => x.Id == id);
                if (Article != null)
                    sortedArticles.Add(Article);
            }
            return sortedArticles;
        }

        public virtual int CountArticles(ArticleSearchContext ctx)
        {
            Guard.ArgumentNotNull(() => ctx);

            var query = this.PrepareArticleSearchQuery(ctx);
            return query.Distinct().Count();
        }

        public virtual IPagedList<Article> SearchArticles(ArticleSearchContext ctx)
        {

            _eventPublisher.Publish(new ArticlesSearchingEvent(ctx));

            //search by keyword
            bool searchLocalizedValue = false;
            if (ctx.LanguageId > 0)
            {
                if (ctx.ShowHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages(siteId: ctx.SiteId).Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (ctx.CategoryIds != null && ctx.CategoryIds.Contains(0))
                ctx.CategoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedUserRolesIds = _workContext.CurrentUser.UserRoles
                .Where(cr => cr.Active).Select(cr => cr.Id).ToList();

            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduresSupported)
            {
                //sited procedures are enabled and supported by the database. 
                //It's much faster than the LINQ implementation below 

                #region Use sited procedure

                //pass categry identifiers as comma-delimited string
                string commaSeparatedCategoryIds = "";
                if (ctx.CategoryIds != null && !ctx.WithoutCategories)
                {
                    for (int i = 0; i < ctx.CategoryIds.Count; i++)
                    {
                        commaSeparatedCategoryIds += ctx.CategoryIds[i].ToString();
                        if (i != ctx.CategoryIds.Count - 1)
                        {
                            commaSeparatedCategoryIds += ",";
                        }
                    }
                }

                //pass customer role identifiers as comma-delimited string
                string commaSeparatedAllowedUserRoleIds = "";
                for (int i = 0; i < allowedUserRolesIds.Count; i++)
                {
                    commaSeparatedAllowedUserRoleIds += allowedUserRolesIds[i].ToString();
                    if (i != allowedUserRolesIds.Count - 1)
                    {
                        commaSeparatedAllowedUserRoleIds += ",";
                    }
                }



                //some databases don't support int.MaxValue
                if (ctx.PageSize == int.MaxValue)
                    ctx.PageSize = int.MaxValue - 1;

                //prepare parameters
                var pCategoryIds = _dataProvider.GetParameter();
                pCategoryIds.ParameterName = "CategoryIds";
                pCategoryIds.Value = commaSeparatedCategoryIds != null ? (object)commaSeparatedCategoryIds : DBNull.Value;
                pCategoryIds.DbType = DbType.String;


                var pSiteId = _dataProvider.GetParameter();
                pSiteId.ParameterName = "SiteId";
                pSiteId.Value = QuerySettings.IgnoreMultiSite ? 0 : ctx.SiteId;
                pSiteId.DbType = DbType.Int32;

                var pArticleTagId = _dataProvider.GetParameter();
                pArticleTagId.ParameterName = "ArticleTagId";
                pArticleTagId.Value = ctx.ArticleTagId;
                pArticleTagId.DbType = DbType.Int32;

                var pFeaturedArticles = _dataProvider.GetParameter();
                pFeaturedArticles.ParameterName = "FeaturedArticles";
                pFeaturedArticles.Value = ctx.FeaturedArticles.HasValue ? (object)ctx.FeaturedArticles.Value : DBNull.Value;
                pFeaturedArticles.DbType = DbType.Boolean;


                var pKeywords = _dataProvider.GetParameter();
                pKeywords.ParameterName = "Keywords";
                pKeywords.Value = ctx.Keywords != null ? (object)ctx.Keywords : DBNull.Value;
                pKeywords.DbType = DbType.String;

                var pSearchDescriptions = _dataProvider.GetParameter();
                pSearchDescriptions.ParameterName = "SearchDescriptions";
                pSearchDescriptions.Value = ctx.SearchDescriptions;
                pSearchDescriptions.DbType = DbType.Boolean;

                var pSearchArticleTags = _dataProvider.GetParameter();
                pSearchArticleTags.ParameterName = "SearchArticleTags";
                pSearchArticleTags.Value = ctx.SearchDescriptions;
                pSearchArticleTags.DbType = DbType.Boolean;

                var pUseFullTextSearch = _dataProvider.GetParameter();
                pUseFullTextSearch.ParameterName = "UseFullTextSearch";
                pUseFullTextSearch.Value = _commonSettings.UseFullTextSearch;
                pUseFullTextSearch.DbType = DbType.Boolean;

                var pFullTextMode = _dataProvider.GetParameter();
                pFullTextMode.ParameterName = "FullTextMode";
                pFullTextMode.Value = (int)_commonSettings.FullTextMode;
                pFullTextMode.DbType = DbType.Int32;


                var pLanguageId = _dataProvider.GetParameter();
                pLanguageId.ParameterName = "LanguageId";
                pLanguageId.Value = searchLocalizedValue ? ctx.LanguageId : 0;
                pLanguageId.DbType = DbType.Int32;

                var pOrderBy = _dataProvider.GetParameter();
                pOrderBy.ParameterName = "OrderBy";
                pOrderBy.Value = (int)ctx.OrderBy;
                pOrderBy.DbType = DbType.Int32;

                var pAllowedUserRoleIds = _dataProvider.GetParameter();
                pAllowedUserRoleIds.ParameterName = "AllowedUserRoleIds";
                pAllowedUserRoleIds.Value = commaSeparatedAllowedUserRoleIds;
                pAllowedUserRoleIds.DbType = DbType.String;

                var pPageIndex = _dataProvider.GetParameter();
                pPageIndex.ParameterName = "PageIndex";
                pPageIndex.Value = ctx.PageIndex;
                pPageIndex.DbType = DbType.Int32;

                var pPageSize = _dataProvider.GetParameter();
                pPageSize.ParameterName = "PageSize";
                pPageSize.Value = ctx.PageSize;
                pPageSize.DbType = DbType.Int32;

                var pShowHidden = _dataProvider.GetParameter();
                pShowHidden.ParameterName = "ShowHidden";
                pShowHidden.Value = ctx.ShowHidden;
                pShowHidden.DbType = DbType.Boolean;

                var pWithoutCategories = _dataProvider.GetParameter();
                pWithoutCategories.ParameterName = "WithoutCategories";
                pWithoutCategories.Value = ctx.WithoutCategories;
                pWithoutCategories.DbType = DbType.Boolean;

                #region »ù´¡ÊôÐÔ¹ýÂË

                var IsHot = _dataProvider.GetParameter();
                IsHot.ParameterName = "IsHot";
                IsHot.Value = ctx.IsHot.HasValue ? (object)ctx.IsHot.Value : DBNull.Value;
                IsHot.DbType = DbType.Boolean;

                var IsRed = _dataProvider.GetParameter();
                IsRed.ParameterName = "IsRed";
                IsRed.Value = ctx.IsRed.HasValue ? (object)ctx.IsRed.Value : DBNull.Value;
                IsRed.DbType = DbType.Boolean;

                var IsTop = _dataProvider.GetParameter();
                IsTop.ParameterName = "IsTop";
                IsTop.Value = ctx.IsTop.HasValue ? (object)ctx.IsTop.Value : DBNull.Value;
                IsTop.DbType = DbType.Boolean;

                var IsSlide = _dataProvider.GetParameter();
                IsSlide.ParameterName = "IsSlide";
                IsSlide.Value = ctx.IsSlide.HasValue ? (object)ctx.IsSlide.Value : DBNull.Value;
                IsSlide.DbType = DbType.Boolean;

                #endregion

                var pTotalRecords = _dataProvider.GetParameter();
                pTotalRecords.ParameterName = "TotalRecords";
                pTotalRecords.Direction = ParameterDirection.Output;
                pTotalRecords.DbType = DbType.Int32;

                //invoke sited procedure
                var articles = _dbContext.ExecuteStoredProcedureList<Article>(
                    "ArticleLoadAllPaged",
                    pCategoryIds,
                    pSiteId,
                    pArticleTagId,
                    pFeaturedArticles,
                    pKeywords,
                    pSearchDescriptions,
                    pSearchArticleTags,
                    pUseFullTextSearch,
                    pFullTextMode,
                    pLanguageId,
                    pOrderBy,
                    pAllowedUserRoleIds,
                    pPageIndex,
                    pPageSize,
                    pShowHidden,
                    IsHot,
                    IsRed,
                    IsTop,
                    IsSlide,
                    pWithoutCategories,
                    pTotalRecords);


                // return articles
                int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
                return new PagedList<Article>(articles, ctx.PageIndex, ctx.PageSize, totalRecords);

                #endregion
            }
            else
            {
                //sited procedures aren't supported. Use LINQ

                #region Search articles

                var query = this.PrepareArticleSearchQuery(ctx, allowedUserRolesIds, searchLocalizedValue);

                // only distinct articles (group by ID)
                // if we use standard Distinct() method, then all fields will be compared (low performance)
                // it'll not work in SQL Server Compact when searching articles by a keyword)
                query = from p in query
                        group p by p.Id into pGroup
                        orderby pGroup.Key
                        select pGroup.FirstOrDefault();

                //sort articles
                if (ctx.OrderBy == ArticleSortingEnum.Position && ctx.CategoryIds != null && ctx.CategoryIds.Count > 0)
                {
                    //category position
                    var firstCategoryId = ctx.CategoryIds[0];
                    // query = query.OrderBy(p => p.ArticleCategories.Where(pc => pc.CategoryId == firstCategoryId).FirstOrDefault().DisplayOrder);
                }

                else if (ctx.OrderBy == ArticleSortingEnum.Position)
                {
                    //parent article specified (sort associated articles)
                    query = query.OrderBy(p => p.DisplayOrder);
                }
                else if (ctx.OrderBy == ArticleSortingEnum.Position)
                {
                    //otherwise sort by name
                    query = query.OrderBy(p => p.Title);
                }
                else if (ctx.OrderBy == ArticleSortingEnum.NameAsc)
                {
                    //Name: A to Z
                    query = query.OrderBy(p => p.Title);
                }
                else if (ctx.OrderBy == ArticleSortingEnum.NameDesc)
                {
                    //Name: Z to A
                    query = query.OrderByDescending(p => p.Title);
                }

                else if (ctx.OrderBy == ArticleSortingEnum.CreatedOn)
                {
                    //creation date
                    query = query.OrderByDescending(p => p.CreatedOnUtc);
                }
                else if (ctx.OrderBy == ArticleSortingEnum.CreatedOnAsc)
                {
                    // creation date: old to new
                    query = query.OrderBy(p => p.CreatedOnUtc);
                }
                else
                {
                    //actually this code is not reachable
                    query = query.OrderBy(p => p.Title);
                }

                var articles = new PagedList<Article>(query, ctx.PageIndex, ctx.PageSize);

                return articles;

                #endregion
            }
        }

        public virtual IQueryable<Article> PrepareArticleSearchQuery(
        ArticleSearchContext ctx,
        IEnumerable<int> allowedUserRolesIds = null,
        bool searchLocalizedValue = false)
        {
            return PrepareArticleSearchQuery<Article>(ctx, x => x, allowedUserRolesIds, searchLocalizedValue);
        }

        public virtual IQueryable<TResult> PrepareArticleSearchQuery<TResult>(
            ArticleSearchContext ctx,
            Expression<Func<Article, TResult>> selector,
            IEnumerable<int> allowedUserRolesIds = null,
            bool searchLocalizedValue = false)
        {
            Guard.ArgumentNotNull(() => ctx);
            Guard.ArgumentNotNull(() => selector);

            if (allowedUserRolesIds == null)
            {
                allowedUserRolesIds = _workContext.CurrentUser.UserRoles.Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            }

            // articles
            var query = ctx.Query ?? _articleRepository.Table;
            query = query.Where(p => !p.Deleted);

            if (!ctx.ShowHidden)
            {
                query = query.Where(p => p.StatusFormat == StatusFormat.Norma);
            }

            if (ctx.IsHot.HasValue)
            {
                query = query.Where(p => p.IsHot == ctx.IsHot.Value);
            }
            if (ctx.IsRed.HasValue)
            {
                query = query.Where(p => p.IsRed == ctx.IsRed.Value);
            }
            if (ctx.IsSlide.HasValue)
            {
                query = query.Where(p => p.IsSlide == ctx.IsSlide.Value);
            }
            if (ctx.IsTop.HasValue)
            {
                query = query.Where(p => p.IsTop == ctx.IsTop.Value);
            }
            if (ctx.ArticleIds != null && ctx.ArticleIds.Count > 0)
            {
                query = query.Where(x => ctx.ArticleIds.Contains(x.Id));
            }

            //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
            //That's why we pass the date value
            var nowUtc = DateTime.UtcNow;


            if (!ctx.ShowHidden)
            {
                //available dates
                query = query.Where(p =>
                    (!p.StartDateUtc.HasValue || p.StartDateUtc.Value < nowUtc) &&
                    (!p.EndDateUtc.HasValue || p.EndDateUtc.Value > nowUtc));
            }

            // searching by keyword
            if (!String.IsNullOrWhiteSpace(ctx.Keywords))
            {
                query = from p in query
                        join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                        from lp in p_lp.DefaultIfEmpty()
                        //  from pt in p.ArticleTags.DefaultIfEmpty()
                        where (p.Title.Contains(ctx.Keywords)) ||
                              (ctx.SearchDescriptions && p.ShortContent.Contains(ctx.Keywords)) ||
                              (ctx.SearchDescriptions && p.FullContent.Contains(ctx.Keywords)) ||
                            // (ctx.SearchArticleTags && pt.Name.Contains(ctx.Keywords)) ||
                            //localized values
                              (searchLocalizedValue && lp.LanguageId == ctx.LanguageId && lp.LocaleKeyGroup == "Article" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(ctx.Keywords)) ||
                              (ctx.SearchDescriptions && searchLocalizedValue && lp.LanguageId == ctx.LanguageId && lp.LocaleKeyGroup == "Article" && lp.LocaleKey == "ShortContent" && lp.LocaleValue.Contains(ctx.Keywords)) ||
                              (ctx.SearchDescriptions && searchLocalizedValue && lp.LanguageId == ctx.LanguageId && lp.LocaleKeyGroup == "Article" && lp.LocaleKey == "FullContent" && lp.LocaleValue.Contains(ctx.Keywords))
                        //UNDONE search localized values in associated article tags
                        select p;
            }

            //if (!ctx.ShowHidden && !QuerySettings.IgnoreAcl)
            //{
            //    query =
            //        from p in query
            //        join acl in _aclRepository.Table on new { pid = p.Id, pname = "Article" } equals new { pid = acl.EntityId, pname = acl.EntityName } into pacl
            //        from acl in pacl.DefaultIfEmpty()
            //        where !p.SubjectToAcl || allowedUserRolesIds.Contains(acl.UserRoleId)
            //        select p;
            //}

            if (ctx.SiteId > 0 && !QuerySettings.IgnoreMultiSite)
            {
                query =
                    from p in query
                    join sm in _siteMappingRepository.Table on new { pid = p.Id, pname = "Article" } equals new { pid = sm.EntityId, pname = sm.EntityName } into psm
                    from sm in psm.DefaultIfEmpty()
                    where !p.LimitedToSites || ctx.SiteId == sm.SiteId
                    select p;
            }



            // category filtering
            //if (ctx.WithoutCategories)
            //{
            //    query = query.Where(x => x.ArticleCategories.Count == 0);
            //}
            //else if (ctx.CategoryIds != null && ctx.CategoryIds.Count > 0)
            //{
            //    //search in subcategories
            //    if (ctx.MatchAllcategories)
            //    {
            //        query = from p in query
            //                where ctx.CategoryIds.All(i => p.ArticleCategories.Any(p2 => p2.CategoryId == i))
            //                from pc in p.ArticleCategories
            //                where (!ctx.FeaturedArticles.HasValue || ctx.FeaturedArticles.Value == pc.IsFeaturedArticle)
            //                select p;
            //    }
            //    else
            //    {
            //        query = from p in query
            //                from pc in p.ArticleCategories.Where(pc => ctx.CategoryIds.Contains(pc.CategoryId))
            //                where (!ctx.FeaturedArticles.HasValue || ctx.FeaturedArticles.Value == pc.IsFeaturedArticle)
            //                select p;
            //    }
            //}



            // related articles filtering
            //if (relatedToArticleId > 0)
            //{
            //    query = from p in query
            //            join rp in _relatedArticleRepository.Table on p.Id equals rp.ArticleId2
            //            where (relatedToArticleId == rp.ArticleId1)
            //            select p;
            //}

            // tag filtering
            //if (ctx.ArticleTagId > 0)
            //{
            //    query = from p in query
            //            from pt in p.ArticleTags.Where(pt => pt.Id == ctx.ArticleTagId)
            //            select p;
            //}

            return query.Select(selector);
        }

        /// <summary>
        /// Inserts a Article
        /// </summary>
        /// <param name="Article">Article</param>
        public virtual void InsertArticle(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("Article");

            //insert
            _articleRepository.Insert(article);

            //clear cache
            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(article);
        }

        /// <summary>
        /// Updates the Article
        /// </summary>
        /// <param name="Article">Article</param>
        public virtual void UpdateArticle(Article article, bool publishEvent = true)
        {
            if (article == null)
                throw new ArgumentNullException("Article");

            bool modified = false;
            if (publishEvent)
            {
                modified = _articleRepository.IsModified(article);
            }

            //update
            _articleRepository.Update(article);

            //cache
            _cacheManager.RemoveByPattern(ARTICLES_PATTERN_KEY);

            //event notification
            if (publishEvent && modified)
            {
                _eventPublisher.EntityUpdated(article);
            }
        }


        /// <summary>
        /// Update blog post comment totals
        /// </summary>
        /// <param name="article">Blog post</param>
        public virtual void UpdateCommentTotals(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            int approvedCommentCount = 0;
            int notApprovedCommentCount = 0;
            var articleComments = article.ArticleComments;
            foreach (var ac in articleComments)
            {
                if (ac.IsApproved)
                    approvedCommentCount++;
                else
                    notApprovedCommentCount++;
            }

            article.ApprovedCommentCount = approvedCommentCount;
            article.NotApprovedCommentCount = notApprovedCommentCount;
            UpdateArticle(article);
        }


        /// <summary>
        /// Update article review totals
        /// </summary>
        /// <param name="article">Article</param>
        public virtual void UpdateArticleReviewTotals(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            int approvedRatingSum = 0;
            int notApprovedRatingSum = 0;
            int approvedTotalReviews = 0;
            int notApprovedTotalReviews = 0;
            var reviews = article.ArticleReviews;
            foreach (var pr in reviews)
            {
                if (pr.IsApproved)
                {
                    approvedRatingSum += pr.Rating;
                    approvedTotalReviews++;
                }
                else
                {
                    notApprovedRatingSum += pr.Rating;
                    notApprovedTotalReviews++;
                }
            }

            article.ApprovedRatingSum = approvedRatingSum;
            article.NotApprovedRatingSum = notApprovedRatingSum;
            article.ApprovedTotalReviews = approvedTotalReviews;
            article.NotApprovedTotalReviews = notApprovedTotalReviews;
            UpdateArticle(article);
        }
        #endregion

        #region Article pictures

        /// <summary>
        /// Deletes a Article picture
        /// </summary>
        /// <param name="ArticleAlbum">Article picture</param>
        public virtual void DeleteArticleAlbum(ArticleAlbum ArticleAlbum)
        {
            if (ArticleAlbum == null)
                throw new ArgumentNullException("ArticleAlbum");

            _articleAlbumRepository.Delete(ArticleAlbum);

            //event notification
            _eventPublisher.EntityDeleted(ArticleAlbum);
        }


        /// <summary>
        /// Gets a Article pictures by Article identifier
        /// </summary>
        /// <param name="ArticleId">The Article identifier</param>
        /// <returns>Article pictures</returns>
        public virtual IList<ArticleAlbum> GetArticleAlbumsByArticleId(int ArticleId)
        {
            var query = from pp in _articleAlbumRepository.Table
                        where pp.ArticleId == ArticleId
                        orderby pp.DisplayOrder
                        select pp;
            var ArticleAlbums = query.ToList();
            return ArticleAlbums;
        }

        /// <summary>
        /// Gets a Article picture
        /// </summary>
        /// <param name="ArticleAlbumId">Article picture identifier</param>
        /// <returns>Article picture</returns>
        public virtual ArticleAlbum GetArticleAlbumById(int ArticleAlbumId)
        {
            if (ArticleAlbumId == 0)
                return null;

            var pp = _articleAlbumRepository.GetById(ArticleAlbumId);
            return pp;
        }

        /// <summary>
        /// Inserts a Article picture
        /// </summary>
        /// <param name="ArticleAlbum">Article picture</param>
        public virtual void InsertArticleAlbum(ArticleAlbum ArticleAlbum)
        {
            if (ArticleAlbum == null)
                throw new ArgumentNullException("ArticleAlbum");

            _articleAlbumRepository.Insert(ArticleAlbum);

            //event notification
            _eventPublisher.EntityInserted(ArticleAlbum);
        }

        /// <summary>
        /// Updates a Article picture
        /// </summary>
        /// <param name="ArticleAlbum">Article picture</param>
        public virtual void UpdateArticleAlbum(ArticleAlbum ArticleAlbum)
        {
            if (ArticleAlbum == null)
                throw new ArgumentNullException("ArticleAlbum");

            _articleAlbumRepository.Update(ArticleAlbum);

            //event notification
            _eventPublisher.EntityUpdated(ArticleAlbum);
        }

        #endregion


        #region Related articles

        /// <summary>
        /// Deletes a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        public virtual void DeleteRelatedArticle(RelatedArticle relatedArticle)
        {
            if (relatedArticle == null)
                throw new ArgumentNullException("relatedArticle");

            _relatedArticleRepository.Delete(relatedArticle);

            //event notification
            _eventPublisher.EntityDeleted(relatedArticle);
        }

        /// <summary>
        /// Gets a related article collection by article identifier
        /// </summary>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related article collection</returns>
        public virtual IList<RelatedArticle> GetRelatedArticlesByArticleId1(int articleId1, bool showHidden = false)
        {
            var query = from rp in _relatedArticleRepository.Table
                        join p in _articleRepository.Table on rp.ArticleId2 equals p.Id
                        where rp.ArticleId1 == articleId1 && !p.Deleted && (showHidden || p.StatusId == (int)StatusFormat.Norma)
                        orderby rp.DisplayOrder
                        select rp;

            var relatedArticles = query.ToList();
            return relatedArticles;
        }

        /// <summary>
        /// Gets a related article
        /// </summary>
        /// <param name="relatedArticleId">Related article identifier</param>
        /// <returns>Related article</returns>
        public virtual RelatedArticle GetRelatedArticleById(int relatedArticleId)
        {
            if (relatedArticleId == 0)
                return null;

            var relatedArticle = _relatedArticleRepository.GetById(relatedArticleId);
            return relatedArticle;
        }

        /// <summary>
        /// Inserts a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        public virtual void InsertRelatedArticle(RelatedArticle relatedArticle)
        {
            if (relatedArticle == null)
                throw new ArgumentNullException("relatedArticle");

            _relatedArticleRepository.Insert(relatedArticle);

            //event notification
            _eventPublisher.EntityInserted(relatedArticle);
        }

        /// <summary>
        /// Updates a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        public virtual void UpdateRelatedArticle(RelatedArticle relatedArticle)
        {
            if (relatedArticle == null)
                throw new ArgumentNullException("relatedArticle");

            _relatedArticleRepository.Update(relatedArticle);

            //event notification
            _eventPublisher.EntityUpdated(relatedArticle);
        }

        /// <summary>
        /// Ensure existence of all mutually related articles
        /// </summary>
        /// <param name="articleId1">First article identifier</param>
        /// <returns>Number of inserted related articles</returns>
        public virtual int EnsureMutuallyRelatedArticles(int articleId1)
        {
            var relatedArticles = GetRelatedArticlesByArticleId1(articleId1, true);
            var articleIds = relatedArticles.Select(x => x.ArticleId2).ToList();

            if (articleIds.Count > 0 && !articleIds.Any(x => x == articleId1))
                articleIds.Add(articleId1);

            int count = EnsureMutuallyRelatedArticles(articleIds);
            return count;
        }

        #endregion
        #endregion
    }
}

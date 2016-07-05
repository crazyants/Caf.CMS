using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// ArticleCategory service
    /// </summary>
    public partial class ArticleCategoryService : IArticleCategoryService
    {
        #region Constants

        private const string CATEGORIES_BY_PARENT_CATEGORY_ID_KEY = "cms.category.byparent-{0}-{1}-{2}-{3}";
        private const string ARTICLECATEGORIES_ALLBYCATEGORYID_KEY = "cms.articlecategory.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}";
        private const string ARTICLECATEGORIES_ALLBYARTICLEID_KEY = "cms.articlecategory.allbyarticleid-{0}-{1}-{2}-{3}";
        private const string CATEGORIES_PATTERN_KEY = "cms.category.";
        private const string ARTICLECATEGORIES_PATTERN_KEY = "cms.articlecategory.";
        private const string CATEGORIES_BY_ID_KEY = "cms.category.id-{0}";

        #endregion

        #region Fields

        private readonly IRepository<ArticleCategory> _categoryRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<SiteMapping> _siteMappingRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IAclService _aclService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="categoryRepository">ArticleCategory repository</param>
        /// <param name="articleArticleCategoryRepository">ProductArticleCategory repository</param>
        /// <param name="articleRepository">Product repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="siteMappingRepository">Site mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="siteContext">Site context</param>
        /// <param name="eventPublisher">Event publisher</param>
        public ArticleCategoryService(ICacheManager cacheManager,
            IRepository<ArticleCategory> categoryRepository,
            IRepository<Article> articleRepository,
            IRepository<SiteMapping> siteMappingRepository,
                 IRepository<AclRecord> aclRepository,
            IWorkContext workContext,
            ISiteContext siteContext,
            IEventPublisher eventPublisher,
            IAclService aclService,
            ISiteMappingService siteMappingService)
        {
            this._cacheManager = cacheManager;
            this._categoryRepository = categoryRepository;
            this._articleRepository = articleRepository;
            this._siteMappingRepository = siteMappingRepository;
            this._aclRepository = aclRepository;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._eventPublisher = eventPublisher;
            this._siteMappingService = siteMappingService;
            this._aclService = aclService;
            this.QuerySettings = DbQuerySettings.Default;
        }

        public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Utilities

        private void DeleteAllCategories(IList<ArticleCategory> categories, bool delete)
        {
            foreach (var category in categories)
            {
                //if (delete)
                //    category.Deleted = true;

                //UpdateArticleCategory(category);

                var childCategories = GetAllArticleCategoriesByParentCategoryId(category.Id, true);
                DeleteAllCategories(childCategories, delete);
                _categoryRepository.Delete(category);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">ArticleCategory</param>
        /// <param name="deleteChilds">Whether to delete child categories or to set them to no parent.</param>
        public virtual void DeleteArticleCategory(ArticleCategory category, bool deleteChilds = false)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            //category.Deleted = true;
            //UpdateArticleCategory(category);

            var childCategories = GetAllArticleCategoriesByParentCategoryId(category.Id, true);
            DeleteAllCategories(childCategories, deleteChilds);

            _categoryRepository.Delete(category);
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">ArticleCategory name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="alias">Alias to be filtered</param>
        /// <param name="applyNavigationFilters">Whether to apply <see cref="IArticleCategoryNavigationFilter"/> instances to the actual categories query. Never applied when <paramref name="showHidden"/> is <c>true</c></param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<ArticleCategory> GetAllCategories(string categoryName = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string alias = null, int? channelId = null,
            bool applyNavigationFilters = true, bool ignoreCategoriesWithoutExistingParent = true)
        {
            var query = _categoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!String.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));
            if (!String.IsNullOrWhiteSpace(alias))
                query = query.Where(c => c.Alias.Contains(alias));
            if (channelId.HasValue)
                query = query.Where(c => c.ChannelId == channelId.Value);

            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.Id).ThenBy(c => c.DisplayOrder);


            var unsortedCategories = query.ToList();

            // sort categories
            var sortedCategories = unsortedCategories.SortCategoriesForTree(ignoreCategoriesWithoutExistingParent: ignoreCategoriesWithoutExistingParent);

            // paging
            return new PagedList<ArticleCategory>(sortedCategories, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentArticleCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>ArticleCategory collection</returns>
        public IList<ArticleCategory> GetAllArticleCategoriesByParentCategoryId(int parentArticleCategoryId, bool showHidden = false)
        {
            string key = string.Format(CATEGORIES_BY_PARENT_CATEGORY_ID_KEY, parentArticleCategoryId, showHidden, _workContext.CurrentUser.Id, _siteContext.CurrentSite.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = _categoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.Where(c => c.ParentCategoryId == parentArticleCategoryId);
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder);

                if (!showHidden)
                {
                    query = ApplyHiddenCategoriesFilter(query, false);
                    query = query.OrderBy(c => c.DisplayOrder);
                }

                var categories = query.ToList();
                return categories;
            });

        }


        protected virtual IQueryable<ArticleCategory> ApplyHiddenCategoriesFilter(IQueryable<ArticleCategory> query, bool applyNavigationFilters)
        {
            // ACL (access control list)
            var allowedUserRolesIds = _workContext.CurrentUser.UserRoles.Where(x => x.Active).Select(x => x.Id).ToList();

            if (!QuerySettings.IgnoreAcl)
            {
                query = from c in query
                        join acl in _aclRepository.Table
                        on new { c1 = c.Id, c2 = "Category" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                        from acl in c_acl.DefaultIfEmpty()
                        where !c.SubjectToAcl || allowedUserRolesIds.Contains(acl.UserRoleId)
                        select c;
            }

            if (!QuerySettings.IgnoreMultiSite)
            {
                //Site mapping
                var currentSiteId = _siteContext.CurrentSite.Id;
                query = from c in query
                        join sm in _siteMappingRepository.Table
                        on new { c1 = c.Id, c2 = "Category" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                        from sm in c_sm.DefaultIfEmpty()
                        where !c.LimitedToSites || currentSiteId == sm.SiteId
                        select c;
            }

            //only distinct categories (group by ID)
            query = from c in query
                    group c by c.Id into cGroup
                    orderby cGroup.Key
                    select cGroup.FirstOrDefault();

            //if (applyNavigationFilters)
            //{
            //    var filters = _navigationFilters.Value;
            //    if (filters.Any())
            //    {
            //        filters.Each(x =>
            //        {
            //            query = x.Apply(query);
            //        });
            //    }
            //}

            return query;
        }



        public IQueryable<ArticleCategory> GetAllCategoriesQ()
        {
            var query = _categoryRepository.Table;
            return query;
        }

        public IList<ArticleCategory> GetCategorysByIds(int[] CategoryIds)
        {
            if (CategoryIds == null || CategoryIds.Length == 0)
                return new List<ArticleCategory>();

            var query = from c in _categoryRepository.Table
                        where CategoryIds.Contains(c.Id)
                        select c;
            var Categorys = query.ToList();
            //sort by passed identifiers
            var sortedCategory = new List<ArticleCategory>();
            foreach (int id in CategoryIds)
            {
                var Category = Categorys.Find(x => x.Id == id);
                if (Category != null)
                    sortedCategory.Add(Category);
            }
            return sortedCategory;
        }

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Categories</returns>
        public virtual IList<ArticleCategory> GetAllCategoriesDisplayedOnHomePage()
        {
            var query = from c in _categoryRepository.Table
                        orderby c.DisplayOrder
                        where c.Published &&
                        !c.Deleted &&
                        c.ShowOnHomePage
                        select c;

            var categories = query.ToList();
            return categories;
        }

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">ArticleCategory identifier</param>
        /// <returns>ArticleCategory</returns>
        public virtual ArticleCategory GetArticleCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            string key = string.Format(CATEGORIES_BY_ID_KEY, categoryId);
            return _cacheManager.Get(key, () =>
            {
                return _categoryRepository.GetById(categoryId);
            });
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">ArticleCategory</param>
        public virtual void InsertArticleCategory(ArticleCategory category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            _categoryRepository.Insert(category);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLECATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(category);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">ArticleCategory</param>
        public virtual void UpdateArticleCategory(ArticleCategory category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            //validate category hierarchy
            var parentArticleCategory = GetArticleCategoryById(category.ParentCategoryId);
            while (parentArticleCategory != null)
            {
                if (category.Id == parentArticleCategory.Id)
                {
                    category.ParentCategoryId = 0;
                    break;
                }
                parentArticleCategory = GetArticleCategoryById(parentArticleCategory.ParentCategoryId);
            }

            _categoryRepository.Update(category);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ARTICLECATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(category);
        }

        /// <summary>
        /// Gets a article category mapping collection
        /// </summary>
        /// <param name="articleId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product category mapping collection</returns>
        public virtual ArticleCategory GetArticleCategoriesByArticleId(int articleId, bool showHidden = false)
        {
            if (articleId == 0)
                return new ArticleCategory();

            string key = string.Format(ARTICLECATEGORIES_ALLBYARTICLEID_KEY, showHidden, articleId, _workContext.CurrentUser.Id, _siteContext.CurrentSite.Id);
            return _cacheManager.Get(key, () =>
            {

                var query = from c in _categoryRepository.Table
                            join a in _articleRepository.Table on c.Id equals a.CategoryId
                            where a.Id == articleId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby a.DisplayOrder
                            select c;

                var result = query.FirstOrDefault();
                if (!showHidden)
                {
                    // ACL (access control list) and site mapping
                    var category = result;
                    if (!_aclService.Authorize(category) || !_siteMappingService.Authorize(category))
                        result = null;
                }

                return result;
            });
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAF.Infrastructure.Core;
//using CAF.Infrastructure.Core.Domain.Shop.Catalog;
using CAF.Infrastructure.Core.Domain.Cms.Articles;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Recently viewed articles service
    /// </summary>
    public partial class RecentlyViewedArticlesService : IRecentlyViewedArticlesService
    {
        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IArticleService _articleService;
        private readonly ArticleCatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="articleService">Article service</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public RecentlyViewedArticlesService(HttpContextBase httpContext, IArticleService articleService,
            ArticleCatalogSettings catalogSettings)
        {
            this._httpContext = httpContext;
            this._articleService = articleService;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a "recently viewed articles" identifier list
        /// </summary>
        /// <returns>"recently viewed articles" list</returns>
        protected IList<int> GetRecentlyViewedArticlesIds()
        {
            return GetRecentlyViewedArticlesIds(int.MaxValue);
        }

        /// <summary>
        /// Gets a "recently viewed articles" identifier list
        /// </summary>
        /// <param name="number">Number of articles to load</param>
        /// <returns>"recently viewed articles" list</returns>
        protected IList<int> GetRecentlyViewedArticlesIds(int number)
        {
            var articleIds = new List<int>();
            var recentlyViewedCookie = _httpContext.Request.Cookies.Get("WebSite.RecentlyViewedArticles");
            if ((recentlyViewedCookie == null) || (recentlyViewedCookie.Values == null))
                return articleIds;

            string[] values = recentlyViewedCookie.Values.GetValues("RecentlyViewedArticleIds");
            if (values == null)
                return articleIds;

            articleIds.AddRange(values.Select(x => int.Parse(x)).Distinct().Take(number));

            return articleIds;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Gets a "recently viewed articles" list
        /// </summary>
        /// <param name="number">Number of articles to load</param>
        /// <returns>"recently viewed articles" list</returns>
        public virtual IList<Article> GetRecentlyViewedArticles(int number)
        {
            var articles = new List<Article>();
            var articleIds = GetRecentlyViewedArticlesIds(number);
            var recentlyViewedArticles = _articleService.GetArticlesByIds(articleIds.ToArray()).Where(x => x.StatusFormat == StatusFormat.Norma && !x.Deleted);

            articles.AddRange(recentlyViewedArticles);
            return articles;
        }

        /// <summary>
        /// Adds a article to a recently viewed articles list
        /// </summary>
        /// <param name="articleId">Article identifier</param>
        public virtual void AddArticleToRecentlyViewedList(int articleId)
        {
            if (!_catalogSettings.RecentlyViewedArticlesEnabled)
                return;

            var oldArticleIds = GetRecentlyViewedArticlesIds();
            var newArticleIds = new List<int>(oldArticleIds);

            if (!newArticleIds.Contains(articleId))
            {
                newArticleIds.Add(articleId);
            }

            var recentlyViewedCookie = _httpContext.Request.Cookies.Get("WebSite.RecentlyViewedArticles");
            if (recentlyViewedCookie == null)
            {
                recentlyViewedCookie = new HttpCookie("WebSite.RecentlyViewedArticles");
                recentlyViewedCookie.HttpOnly = true;
            }
            recentlyViewedCookie.Values.Clear();

            int maxArticles = _catalogSettings.RecentlyViewedArticlesNumber;
            if (maxArticles <= 0)
                maxArticles = 10;

            int skip = Math.Max(0, newArticleIds.Count - maxArticles);
            newArticleIds.Skip(skip).Take(maxArticles).Each(x =>
            {
                recentlyViewedCookie.Values.Add("RecentlyViewedArticleIds", x.ToString());
            });

            recentlyViewedCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.Cookies.Set(recentlyViewedCookie);
        }

        #endregion
    }
}

using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// ArticleCategory service interface
    /// </summary>
    public partial interface IArticleService
    {
        #region Articles

        /// <summary>
        /// Delete a Article
        /// </summary>
        /// <param name="Article">Article</param>
        void DeleteArticle(Article article);

        /// <summary>
        /// Gets all Articles displayed on the home page
        /// </summary>
        /// <returns>Article collection</returns>
        IList<Article> GetAllArticlesDisplayedOnHomePage();
        IList<Article> GetAllArticles();

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
        IPagedList<Article> GetAllArticles(int siteId,
            DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize, bool showHidden = false);


        /// <summary>
        /// Gets Article
        /// </summary>
        /// <param name="ArticleId">Article identifier</param>
        /// <returns>Article</returns>
        Article GetArticleById(int articleId);

        Article GetPreArticleById(int articleId, int categoryId);
        Article GetNextArticleById(int articleId, int categoryId);

        /// <summary>
        /// Gets Articles by identifier
        /// </summary>
        /// <param name="ArticleIds">Article identifiers</param>
        /// <returns>Articles</returns>
        IList<Article> GetArticlesByIds(int[] ArticleIds);

        int CountArticles(ArticleSearchContext ctx);

        IPagedList<Article> SearchArticles(ArticleSearchContext ctx);
        IQueryable<Article> PrepareArticleSearchQuery(
        ArticleSearchContext ctx,
        IEnumerable<int> allowedUserRolesIds = null,
        bool searchLocalizedValue = false);

        IQueryable<TResult> PrepareArticleSearchQuery<TResult>(
            ArticleSearchContext ctx,
            Expression<Func<Article, TResult>> selector,
            IEnumerable<int> allowedUserRolesIds = null,
            bool searchLocalizedValue = false);
        /// <summary>
        /// Inserts a Article
        /// </summary>
        /// <param name="Article">Article</param>
        void InsertArticle(Article article);

        /// <summary>
        /// Updates the Article
        /// </summary>
        /// <param name="Article">Article</param>
        void UpdateArticle(Article article, bool publishEvent = true);

        /// <summary>
        /// Update blog post comment totals
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        void UpdateCommentTotals(Article article);

        void UpdateArticleReviewTotals(Article article);
        #endregion

        #region Article pictures

        /// <summary>
        /// Deletes a Article picture
        /// </summary>
        /// <param name="ArticleAlbum">Article picture</param>
        void DeleteArticleAlbum(ArticleAlbum ArticleAlbum);

        /// <summary>
        /// Gets a Article pictures by Article identifier
        /// </summary>
        /// <param name="ArticleId">The Article identifier</param>
        /// <returns>Article pictures</returns>
        IList<ArticleAlbum> GetArticleAlbumsByArticleId(int ArticleId);

        /// <summary>
        /// Gets a Article picture
        /// </summary>
        /// <param name="ArticleAlbumId">Article picture identifier</param>
        /// <returns>Article picture</returns>
        ArticleAlbum GetArticleAlbumById(int ArticleAlbumId);

        /// <summary>
        /// Inserts a Article picture
        /// </summary>
        /// <param name="ArticleAlbum">Article picture</param>
        void InsertArticleAlbum(ArticleAlbum ArticleAlbum);

        /// <summary>
        /// Updates a Article picture
        /// </summary>
        /// <param name="ArticleAlbum">Article picture</param>
        void UpdateArticleAlbum(ArticleAlbum ArticleAlbum);

        #endregion

        #region Related articles

        /// <summary>
        /// Deletes a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        void DeleteRelatedArticle(RelatedArticle relatedArticle);

        /// <summary>
        /// Gets a related article collection by article identifier
        /// </summary>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related article collection</returns>
        IList<RelatedArticle> GetRelatedArticlesByArticleId1(int articleId1, bool showHidden = false);

        /// <summary>
        /// Gets a related article
        /// </summary>
        /// <param name="relatedArticleId">Related article identifier</param>
        /// <returns>Related article</returns>
        RelatedArticle GetRelatedArticleById(int relatedArticleId);

        /// <summary>
        /// Inserts a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        void InsertRelatedArticle(RelatedArticle relatedArticle);

        /// <summary>
        /// Updates a related article
        /// </summary>
        /// <param name="relatedArticle">Related article</param>
        void UpdateRelatedArticle(RelatedArticle relatedArticle);

        /// <summary>
        /// Ensure existence of all mutually related articles
        /// </summary>
        /// <param name="articleId1">First article identifier</param>
        /// <returns>Number of inserted related articles</returns>
        int EnsureMutuallyRelatedArticles(int articleId1);

        #endregion
    }

}

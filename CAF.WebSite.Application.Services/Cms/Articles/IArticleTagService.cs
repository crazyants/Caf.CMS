using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Article tag service interface
    /// </summary>
    public partial interface IArticleTagService
    {
        /// <summary>
        /// Delete a article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        void DeleteArticleTag(ArticleTag articleTag);

        /// <summary>
        /// Gets all article tags
        /// </summary>
        /// <returns>Article tags</returns>
        IList<ArticleTag> GetAllArticleTags();

        /// <summary>
        /// Gets all article tag names
        /// </summary>
        /// <returns>Article tag names as list</returns>
        IList<string> GetAllArticleTagNames();

        /// <summary>
        /// Gets article tag
        /// </summary>
        /// <param name="articleTagId">Article tag identifier</param>
        /// <returns>Article tag</returns>
        ArticleTag GetArticleTagById(int articleTagId);
        
        /// <summary>
        /// Gets article tag by name
        /// </summary>
        /// <param name="name">Article tag name</param>
        /// <returns>Article tag</returns>
        ArticleTag GetArticleTagByName(string name);

        /// <summary>
        /// Inserts a article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        void InsertArticleTag(ArticleTag articleTag);

        /// <summary>
        /// Updates the article tag
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        void UpdateArticleTag(ArticleTag articleTag);

		/// <summary>
		/// Get number of articles
		/// </summary>
		/// <param name="articleTagId">Article tag identifier</param>
		/// <param name="storeId">Store identifier</param>
		/// <returns>Number of articles</returns>
		int GetArticleCount(int articleTagId, int storeId);
    }
}

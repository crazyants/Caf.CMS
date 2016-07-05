
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Recently viewed products service
    /// </summary>
    public partial interface IRecentlyViewedArticlesService
    {
        /// <summary>
        /// Gets a "recently viewed products" list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
		IList<Article> GetRecentlyViewedArticles(int number);

        /// <summary>
        /// Adds a product to a recently viewed products list
        /// </summary>
        /// <param name="productId">Article identifier</param>
		void AddArticleToRecentlyViewedList(int articleId);
    }
}

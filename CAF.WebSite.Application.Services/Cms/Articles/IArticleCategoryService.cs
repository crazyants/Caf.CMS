using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// ArticleCategory service interface
    /// </summary>
    public partial interface IArticleCategoryService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">ArticleCategory</param>
        /// <param name="deleteChilds">Whether to delete child categories or to set them to no parent.</param>
        void DeleteArticleCategory(ArticleCategory category, bool deleteChilds = false);

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
        IPagedList<ArticleCategory> GetAllCategories(string categoryName = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string alias = null, int? channelId = null,
            bool applyNavigationFilters = true, bool ignoreCategoriesWithoutExistingParent = true);
        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category collection</returns>
        IList<ArticleCategory> GetAllArticleCategoriesByParentCategoryId(int parentCategoryId, bool showHidden = false);


        IQueryable<ArticleCategory> GetAllCategoriesQ();

        IList<ArticleCategory> GetCategorysByIds(int[] CategoryIds);
        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Categories</returns>
        IList<ArticleCategory> GetAllCategoriesDisplayedOnHomePage();

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">ArticleCategory identifier</param>
        /// <returns>ArticleCategory</returns>
        ArticleCategory GetArticleCategoryById(int categoryId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">ArticleCategory</param>
        void InsertArticleCategory(ArticleCategory category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">ArticleCategory</param>
        void UpdateArticleCategory(ArticleCategory category);

        /// <summary>
        /// 根据文章ID获取类别
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        ArticleCategory GetArticleCategoriesByArticleId(int articleId, bool showHidden = false);
    }

}

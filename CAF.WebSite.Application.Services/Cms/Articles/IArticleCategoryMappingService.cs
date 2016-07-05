
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
	/// <summary>
	/// ArticleCategory mapping service interface
	/// </summary>
    public partial interface IArticleCategoryMappingService
	{
        /// <summary>
        /// Deletes a category mapping record
        /// </summary>
        /// <param name="categoryMapping">ArticleCategory mapping record</param>
        void DeleteArticleCategoryMapping(ArticleCategoryMapping categoryMapping);

        /// <summary>
        /// Gets a category mapping record
        /// </summary>
        /// <param name="categoryMappingId">ArticleCategory mapping record identifier</param>
        /// <returns>ArticleCategory mapping record</returns>
        ArticleCategoryMapping GetArticleCategoryMappingById(int categoryMappingId);

        /// <summary>
        /// Gets category mapping records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ArticleCategory mapping records</returns>
        IList<ArticleCategoryMapping> GetArticleCategoryMappings<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// Gets category mapping records
        /// </summary>
        /// <param name="entityName">Could be null</param>
        /// <param name="entityId">Could be 0</param>
        /// <returns>ArticleCategory mapping record query</returns>
        IQueryable<ArticleCategoryMapping> GetArticleCategoryMappingsFor(string entityName, int entityId);

        /// <summary>
        /// Save the category napping for an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">The entity</param>
        /// <param name="selectedArticleCategoryIds">Array of selected category ids</param>
        void SaveArticleCategoryMappings<T>(T entity, int[] selectedArticleCategoryIds) where T : BaseEntity;

        /// <summary>
        /// Inserts a category mapping record
        /// </summary>
        /// <param name="categoryMapping">ArticleCategory mapping</param>
        void InsertArticleCategoryMapping(ArticleCategoryMapping categoryMapping);

        /// <summary>
        /// Inserts a category mapping record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="categoryId">ArticleCategory id</param>
        /// <param name="entity">Entity</param>
        void InsertArticleCategoryMapping<T>(T entity, int categoryId) where T : BaseEntity;

        /// <summary>
        /// Updates the category mapping record
        /// </summary>
        /// <param name="categoryMapping">ArticleCategory mapping</param>
        void UpdateArticleCategoryMapping(ArticleCategoryMapping categoryMapping);

        /// <summary>
        /// Find category identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>ArticleCategory identifiers</returns>
        int[] GetArticleCategorysIdsWithAccess<T>(T entity) where T : BaseEntity;

      
	}
}
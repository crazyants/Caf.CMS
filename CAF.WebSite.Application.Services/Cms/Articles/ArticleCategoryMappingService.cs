
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
	/// <summary>
	/// Site mapping service
	/// </summary>
	public partial class ArticleCategoryMappingService : IArticleCategoryMappingService
	{
		#region Constants

        private const string STOREMAPPING_BY_ENTITYID_NAME_KEY = "WebSite.articlecategorymapping.entityid-name-{0}-{1}";
        private const string STOREMAPPING_PATTERN_KEY = "WebSite.articlecategorymapping.";

		#endregion

		#region Fields

		private readonly IRepository<ArticleCategoryMapping> _articleCategoryMappingRepository;
		private readonly IArticleCategoryService _articleCategoryService;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Ctor

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="cacheManager">Cache manager</param>
		/// <param name="articleCategoryMappingRepository">ArticleCategory mapping repository</param>
        public ArticleCategoryMappingService(ICacheManager cacheManager,
			IArticleCategoryService articleCategoryService,
			IRepository<ArticleCategoryMapping> articleCategoryMappingRepository)
		{
			this._cacheManager = cacheManager;
		 
			this._articleCategoryService = articleCategoryService;
			this._articleCategoryMappingRepository = articleCategoryMappingRepository;

			this.QuerySettings = DbQuerySettings.Default;
		}

		public DbQuerySettings QuerySettings { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Deletes a articleCategory mapping record
		/// </summary>
		/// <param name="articleCategoryMapping">ArticleCategory mapping record</param>
		public virtual void DeleteArticleCategoryMapping(ArticleCategoryMapping articleCategoryMapping)
		{
			if (articleCategoryMapping == null)
				throw new ArgumentNullException("articleCategoryMapping");

			_articleCategoryMappingRepository.Delete(articleCategoryMapping);

			//cache
			_cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Gets a articleCategory mapping record
		/// </summary>
		/// <param name="articleCategoryMappingId">ArticleCategory mapping record identifier</param>
		/// <returns>ArticleCategory mapping record</returns>
		public virtual ArticleCategoryMapping GetArticleCategoryMappingById(int articleCategoryMappingId)
		{
			if (articleCategoryMappingId == 0)
				return null;

			var articleCategoryMapping = _articleCategoryMappingRepository.GetById(articleCategoryMappingId);
			return articleCategoryMapping;
		}

		/// <summary>
		/// Gets articleCategory mapping records
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Entity</param>
		/// <returns>ArticleCategory mapping records</returns>
		public virtual IList<ArticleCategoryMapping> GetArticleCategoryMappings<T>(T entity) where T : BaseEntity
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			var query = from sm in _articleCategoryMappingRepository.Table
						where sm.EntityId == entityId &&
						sm.EntityName == entityName
						select sm;
			var articleCategoryMappings = query.ToList();
			return articleCategoryMappings;
		}

		/// <summary>
		/// Gets articleCategory mapping records
		/// </summary>
		/// <param name="entityName">Could be null</param>
		/// <param name="entityId">Could be 0</param>
		/// <returns>ArticleCategory mapping record query</returns>
		public virtual IQueryable<ArticleCategoryMapping> GetArticleCategoryMappingsFor(string entityName, int entityId)
		{
			var query = _articleCategoryMappingRepository.Table;

			if (entityName.HasValue())
				query = query.Where(x => x.EntityName == entityName);

			if (entityId != 0)
				query = query.Where(x => x.EntityId == entityId);

			return query;
		}

		/// <summary>
		/// Save the articleCategory napping for an entity
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <param name="entity">The entity</param>
		/// <param name="selectedArticleCategoryIds">Array of selected articleCategory ids</param>
		public virtual void SaveArticleCategoryMappings<T>(T entity, int[] selectedArticleCategoryIds) where T : BaseEntity
		{
			var existingArticleCategoryMappings = GetArticleCategoryMappings(entity);
			var allArticleCategorys = _articleCategoryService.GetAllCategories();

			foreach (var articleCategory in allArticleCategorys)
			{
				if (selectedArticleCategoryIds != null && selectedArticleCategoryIds.Contains(articleCategory.Id))
				{
					if (existingArticleCategoryMappings.Where(sm => sm.ArticleCategoryId == articleCategory.Id).Count() == 0)
						InsertArticleCategoryMapping(entity, articleCategory.Id);
				}
				else
				{
					var articleCategoryMappingToDelete = existingArticleCategoryMappings.Where(sm => sm.ArticleCategoryId == articleCategory.Id).FirstOrDefault();
					if (articleCategoryMappingToDelete != null)
						DeleteArticleCategoryMapping(articleCategoryMappingToDelete);
				}
			}
		}

		/// <summary>
		/// Inserts a articleCategory mapping record
		/// </summary>
		/// <param name="articleCategoryMapping">ArticleCategory mapping</param>
		public virtual void InsertArticleCategoryMapping(ArticleCategoryMapping articleCategoryMapping)
		{
			if (articleCategoryMapping == null)
				throw new ArgumentNullException("articleCategoryMapping");

			_articleCategoryMappingRepository.Insert(articleCategoryMapping);

			//cache
			_cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Inserts a articleCategory mapping record
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="categoryId">ArticleCategory id</param>
		/// <param name="entity">Entity</param>
		public virtual void InsertArticleCategoryMapping<T>(T entity, int categoryId) where T : BaseEntity
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			if (categoryId == 0)
				throw new ArgumentOutOfRangeException("categoryId");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			var articleCategoryMapping = new ArticleCategoryMapping()
			{
				EntityId = entityId,
				EntityName = entityName,
				ArticleCategoryId = categoryId
			};

			InsertArticleCategoryMapping(articleCategoryMapping);
		}

		/// <summary>
		/// Updates the articleCategory mapping record
		/// </summary>
		/// <param name="articleCategoryMapping">ArticleCategory mapping</param>
		public virtual void UpdateArticleCategoryMapping(ArticleCategoryMapping articleCategoryMapping)
		{
			if (articleCategoryMapping == null)
				throw new ArgumentNullException("articleCategoryMapping");

			_articleCategoryMappingRepository.Update(articleCategoryMapping);

			//cache
			_cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Find articleCategory identifiers with granted access (mapped to the entity)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Wntity</param>
		/// <returns>ArticleCategory identifiers</returns>
		public virtual int[] GetArticleCategorysIdsWithAccess<T>(T entity) where T : BaseEntity
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			string key = string.Format(STOREMAPPING_BY_ENTITYID_NAME_KEY, entityId, entityName);
			return _cacheManager.Get(key, () =>
			{
				var query = from sm in _articleCategoryMappingRepository.Table
							where sm.EntityId == entityId &&
							sm.EntityName == entityName
							select sm.ArticleCategoryId;
				var result = query.ToArray();
				//little hack here. nulls aren't cacheable so set it to ""
				if (result == null)
					result = new int[0];
				return result;
			});
		}

	 

		#endregion
	}
}
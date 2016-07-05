
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Sites
{
	/// <summary>
	/// Site mapping service
	/// </summary>
	public partial class SiteMappingService : ISiteMappingService
	{
		#region Constants

        private const string STOREMAPPING_BY_ENTITYID_NAME_KEY = "WebSite.sitemapping.entityid-name-{0}-{1}";
        private const string STOREMAPPING_PATTERN_KEY = "WebSite.sitemapping.";

		#endregion

		#region Fields

		private readonly IRepository<SiteMapping> _siteMappingRepository;
		private readonly ISiteContext _siteContext;
		private readonly ISiteService _siteService;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Ctor

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="cacheManager">Cache manager</param>
		/// <param name="siteContext">Site context</param>
		/// <param name="siteMappingRepository">Site mapping repository</param>
		public SiteMappingService(ICacheManager cacheManager,
			ISiteContext siteContext,
			ISiteService siteService,
			IRepository<SiteMapping> siteMappingRepository)
		{
			this._cacheManager = cacheManager;
			this._siteContext = siteContext;
			this._siteService = siteService;
			this._siteMappingRepository = siteMappingRepository;

			this.QuerySettings = DbQuerySettings.Default;
		}

		public DbQuerySettings QuerySettings { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Deletes a site mapping record
		/// </summary>
		/// <param name="siteMapping">Site mapping record</param>
		public virtual void DeleteSiteMapping(SiteMapping siteMapping)
		{
			if (siteMapping == null)
				throw new ArgumentNullException("siteMapping");

			_siteMappingRepository.Delete(siteMapping);

			//cache
			_cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Gets a site mapping record
		/// </summary>
		/// <param name="siteMappingId">Site mapping record identifier</param>
		/// <returns>Site mapping record</returns>
		public virtual SiteMapping GetSiteMappingById(int siteMappingId)
		{
			if (siteMappingId == 0)
				return null;

			var siteMapping = _siteMappingRepository.GetById(siteMappingId);
			return siteMapping;
		}

		/// <summary>
		/// Gets site mapping records
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Entity</param>
		/// <returns>Site mapping records</returns>
		public virtual IList<SiteMapping> GetSiteMappings<T>(T entity) where T : BaseEntity, ISiteMappingSupported
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			var query = from sm in _siteMappingRepository.Table
						where sm.EntityId == entityId &&
						sm.EntityName == entityName
						select sm;
			var siteMappings = query.ToList();
			return siteMappings;
		}

		/// <summary>
		/// Gets site mapping records
		/// </summary>
		/// <param name="entityName">Could be null</param>
		/// <param name="entityId">Could be 0</param>
		/// <returns>Site mapping record query</returns>
		public virtual IQueryable<SiteMapping> GetSiteMappingsFor(string entityName, int entityId)
		{
			var query = _siteMappingRepository.Table;

			if (entityName.HasValue())
				query = query.Where(x => x.EntityName == entityName);

			if (entityId != 0)
				query = query.Where(x => x.EntityId == entityId);

			return query;
		}

		/// <summary>
		/// Save the site napping for an entity
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <param name="entity">The entity</param>
		/// <param name="selectedSiteIds">Array of selected site ids</param>
		public virtual void SaveSiteMappings<T>(T entity, int[] selectedSiteIds) where T : BaseEntity, ISiteMappingSupported
		{
			var existingSiteMappings = GetSiteMappings(entity);
			var allSites = _siteService.GetAllSites();

			foreach (var site in allSites)
			{
				if (selectedSiteIds != null && selectedSiteIds.Contains(site.Id))
				{
					if (existingSiteMappings.Where(sm => sm.SiteId == site.Id).Count() == 0)
						InsertSiteMapping(entity, site.Id);
				}
				else
				{
					var siteMappingToDelete = existingSiteMappings.Where(sm => sm.SiteId == site.Id).FirstOrDefault();
					if (siteMappingToDelete != null)
						DeleteSiteMapping(siteMappingToDelete);
				}
			}
		}

		/// <summary>
		/// Inserts a site mapping record
		/// </summary>
		/// <param name="siteMapping">Site mapping</param>
		public virtual void InsertSiteMapping(SiteMapping siteMapping)
		{
			if (siteMapping == null)
				throw new ArgumentNullException("siteMapping");

			_siteMappingRepository.Insert(siteMapping);

			//cache
			_cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Inserts a site mapping record
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="siteId">Site id</param>
		/// <param name="entity">Entity</param>
		public virtual void InsertSiteMapping<T>(T entity, int siteId) where T : BaseEntity, ISiteMappingSupported
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			if (siteId == 0)
				throw new ArgumentOutOfRangeException("siteId");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			var siteMapping = new SiteMapping()
			{
				EntityId = entityId,
				EntityName = entityName,
				SiteId = siteId
			};

			InsertSiteMapping(siteMapping);
		}

		/// <summary>
		/// Updates the site mapping record
		/// </summary>
		/// <param name="siteMapping">Site mapping</param>
		public virtual void UpdateSiteMapping(SiteMapping siteMapping)
		{
			if (siteMapping == null)
				throw new ArgumentNullException("siteMapping");

			_siteMappingRepository.Update(siteMapping);

			//cache
			_cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);
		}

		/// <summary>
		/// Find site identifiers with granted access (mapped to the entity)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Wntity</param>
		/// <returns>Site identifiers</returns>
		public virtual int[] GetSitesIdsWithAccess<T>(T entity) where T : BaseEntity, ISiteMappingSupported
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			int entityId = entity.Id;
			string entityName = typeof(T).Name;

			string key = string.Format(STOREMAPPING_BY_ENTITYID_NAME_KEY, entityId, entityName);
			return _cacheManager.Get(key, () =>
			{
				var query = from sm in _siteMappingRepository.Table
							where sm.EntityId == entityId &&
							sm.EntityName == entityName
							select sm.SiteId;
				var result = query.ToArray();
				//little hack here. nulls aren't cacheable so set it to ""
				if (result == null)
					result = new int[0];
				return result;
			});
		}

		/// <summary>
		/// Authorize whether entity could be accessed in the current site (mapped to this site)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Wntity</param>
		/// <returns>true - authorized; otherwise, false</returns>
		public virtual bool Authorize<T>(T entity) where T : BaseEntity, ISiteMappingSupported
		{
			return Authorize(entity, _siteContext.CurrentSite.Id);
		}

		/// <summary>
		/// Authorize whether entity could be accessed in a site (mapped to this site)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Entity</param>
		/// <param name="site">Site</param>
		/// <returns>true - authorized; otherwise, false</returns>
		public virtual bool Authorize<T>(T entity, int siteId) where T : BaseEntity, ISiteMappingSupported
		{
			if (entity == null)
				return false;

			if (siteId == 0)
				//return true if no site specified/found
				return true;

			if (QuerySettings.IgnoreMultiSite)
				return true;

			if (!entity.LimitedToSites)
				return true;

			foreach (var siteIdWithAccess in GetSitesIdsWithAccess(entity))
				if (siteId == siteIdWithAccess)
					//yes, we have such permission
					return true;

			//no permission found
			return false;
		}

		#endregion
	}
}
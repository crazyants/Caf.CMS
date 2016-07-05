
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Sites
{
	/// <summary>
	/// Site mapping service interface
	/// </summary>
    public partial interface ISiteMappingService
	{
        /// <summary>
        /// Deletes a store mapping record
        /// </summary>
        /// <param name="storeMapping">Site mapping record</param>
        void DeleteSiteMapping(SiteMapping storeMapping);

        /// <summary>
        /// Gets a store mapping record
        /// </summary>
        /// <param name="storeMappingId">Site mapping record identifier</param>
        /// <returns>Site mapping record</returns>
        SiteMapping GetSiteMappingById(int storeMappingId);

        /// <summary>
        /// Gets store mapping records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Site mapping records</returns>
        IList<SiteMapping> GetSiteMappings<T>(T entity) where T : BaseEntity, ISiteMappingSupported;

        /// <summary>
        /// Gets store mapping records
        /// </summary>
        /// <param name="entityName">Could be null</param>
        /// <param name="entityId">Could be 0</param>
        /// <returns>Site mapping record query</returns>
        IQueryable<SiteMapping> GetSiteMappingsFor(string entityName, int entityId);

        /// <summary>
        /// Save the store napping for an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">The entity</param>
        /// <param name="selectedSiteIds">Array of selected store ids</param>
        void SaveSiteMappings<T>(T entity, int[] selectedSiteIds) where T : BaseEntity, ISiteMappingSupported;

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <param name="storeMapping">Site mapping</param>
        void InsertSiteMapping(SiteMapping storeMapping);

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Site id</param>
        /// <param name="entity">Entity</param>
        void InsertSiteMapping<T>(T entity, int storeId) where T : BaseEntity, ISiteMappingSupported;

        /// <summary>
        /// Updates the store mapping record
        /// </summary>
        /// <param name="storeMapping">Site mapping</param>
        void UpdateSiteMapping(SiteMapping storeMapping);

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>Site identifiers</returns>
        int[] GetSitesIdsWithAccess<T>(T entity) where T : BaseEntity, ISiteMappingSupported;

        /// <summary>
        /// Authorize whether entity could be accessed in the current store (mapped to this store)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize<T>(T entity) where T : BaseEntity, ISiteMappingSupported;

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Site identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize<T>(T entity, int storeId) where T : BaseEntity, ISiteMappingSupported;
	}
}
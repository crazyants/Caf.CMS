using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.RegionalContents;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.RegionalContents
{
    /// <summary>
    /// RegionalContentService service
    /// </summary>
    public partial class RegionalContentService : IRegionalContentService
    {
        #region Fields

        private readonly IRepository<RegionalContent> _regionalContentRepository;
		private readonly IRepository<SiteMapping> _siteMappingRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public RegionalContentService(IRepository<RegionalContent> regionalContentRepository,
			IRepository<SiteMapping> siteMappingRepository,
			IEventPublisher eventPublisher)
        {
            _regionalContentRepository = regionalContentRepository;
			_siteMappingRepository = siteMappingRepository;
            _eventPublisher = eventPublisher;

			this.QuerySettings = DbQuerySettings.Default;
		}

		public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a regionalContent
        /// </summary>
        /// <param name="regionalContent">RegionalContent</param>
        public virtual void DeleteRegionalContent(RegionalContent regionalContent)
        {
            if (regionalContent == null)
                throw new ArgumentNullException("regionalContent");

            _regionalContentRepository.Delete(regionalContent);

            //event notification
            _eventPublisher.EntityDeleted(regionalContent);
        }

        /// <summary>
        /// Gets a regionalContent
        /// </summary>
        /// <param name="regionalContentId">The regionalContent identifier</param>
        /// <returns>RegionalContent</returns>
        public virtual RegionalContent GetRegionalContentById(int regionalContentId)
        {
            if (regionalContentId == 0)
                return null;

            return _regionalContentRepository.GetById(regionalContentId);
        }

        /// <summary>
        /// Gets a regionalContent
        /// </summary>
        /// <param name="systemName">The regionalContent system name</param>
		/// <param name="siteId">Site identifier</param>
        /// <returns>RegionalContent</returns>
		public virtual RegionalContent GetRegionalContentBySystemName(string systemName, int siteId,int languageId)
        {
            if (String.IsNullOrEmpty(systemName))
                return null;

			var query = _regionalContentRepository.Table;
           
			if (languageId > 0)
			{
				query = query.Where(n => languageId == n.LanguageId);
			}

			query = query.Where(t => t.SystemName == systemName);
			query = query.OrderBy(t => t.Id);

			//Site mapping
            if (siteId > 0 && !QuerySettings.IgnoreMultiSite)
			{
				query = from t in query
						join sm in _siteMappingRepository.Table
						on new { c1 = t.Id, c2 = "RegionalContent" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into t_sm
						from sm in t_sm.DefaultIfEmpty()
						where !t.LimitedToSites || siteId == sm.SiteId
						select t;

				//only distinct items (group by ID)
				query = from t in query
						group t by t.Id into tGroup
						orderby tGroup.Key
						select tGroup.FirstOrDefault();
				query = query.OrderBy(t => t.Id);
			}

			return query.FirstOrDefault();
        }

        /// <summary>
        /// Gets all regionalContents
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>RegionalContents</returns>
		public virtual IList<RegionalContent> GetAllRegionalContents(int siteId)
        {
			var query = _regionalContentRepository.Table;
			query = query.OrderBy(t => t.CreatedOnUtc).ThenBy(t => t.SystemName);

			//Site mapping
            if (siteId > 0 && !QuerySettings.IgnoreMultiSite)
			{
				query = from t in query
						join sm in _siteMappingRepository.Table
						on new { c1 = t.Id, c2 = "RegionalContent" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into t_sm
						from sm in t_sm.DefaultIfEmpty()
                        where !t.LimitedToSites || siteId == sm.SiteId
						select t;

				//only distinct items (group by ID)
				query = from t in query
						group t by t.Id	into tGroup
						orderby tGroup.Key
						select tGroup.FirstOrDefault();
				query = query.OrderBy(t => t.SystemName);
			}

			return query.ToList();
        }

        /// <summary>
        /// Gets all links
        /// </summary>
        /// <param name="isHome">IsHome identifier; pass 0 to load all records</param>
        /// <returns>Links</returns>
        public virtual IPagedList<RegionalContent> GetAllRegionalContents(int languageId, int siteId, int pageIndex, int pageSize)
        {
            var query = _regionalContentRepository.Table;
            if (languageId > 0)
            {
                query = query.Where(n => languageId == n.LanguageId);
            }

            query = query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.CreatedOnUtc);
            //Site mapping
            if (siteId > 0 && !QuerySettings.IgnoreMultiSite)
            {
                query = from n in query
                        join sm in _siteMappingRepository.Table
                        on new { c1 = n.Id, c2 = "RegionalContent" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into n_sm
                        from sm in n_sm.DefaultIfEmpty()
                        where !n.LimitedToSites || siteId == sm.SiteId
                        select n;

                //only distinct items (group by ID)
                query = from n in query
                        group n by n.Id into nGroup
                        orderby nGroup.Key
                        select nGroup.FirstOrDefault();

                query = query.OrderByDescending(n => n.CreatedOnUtc);
            }

            var links = new PagedList<RegionalContent>(query, pageIndex, pageSize);
            return links;
        }


        /// <summary>
        /// Inserts a regionalContent
        /// </summary>
        /// <param name="regionalContent">RegionalContent</param>
        public virtual void InsertRegionalContent(RegionalContent regionalContent)
        {
            if (regionalContent == null)
                throw new ArgumentNullException("regionalContent");

            _regionalContentRepository.Insert(regionalContent);

            //event notification
            _eventPublisher.EntityInserted(regionalContent);
        }

        /// <summary>
        /// Updates the regionalContent
        /// </summary>
        /// <param name="regionalContent">RegionalContent</param>
        public virtual void UpdateRegionalContent(RegionalContent regionalContent)
        {
            if (regionalContent == null)
                throw new ArgumentNullException("regionalContent");

            _regionalContentRepository.Update(regionalContent);

            //event notification
            _eventPublisher.EntityUpdated(regionalContent);
        }

        #endregion
    }
}

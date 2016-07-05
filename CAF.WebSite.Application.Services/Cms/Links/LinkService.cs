using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Links
{
    /// <summary>
    /// Link service
    /// </summary>
    public partial class LinkService : ILinkService
    {
        #region Fields

        private readonly IRepository<Link> _linkRepository;
        private readonly IRepository<SiteMapping> _siteMappingRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public LinkService(IRepository<Link> linkRepository,
            IRepository<SiteMapping> siteMappingRepository,
            IEventPublisher eventPublisher)
        {
            _linkRepository = linkRepository;
            _siteMappingRepository = siteMappingRepository;
            _eventPublisher = eventPublisher;

            this.QuerySettings = DbQuerySettings.Default;
        }

        public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a link
        /// </summary>
        /// <param name="link">Link</param>
        public virtual void DeleteLink(Link link)
        {
            if (link == null)
                throw new ArgumentNullException("link");

            _linkRepository.Delete(link);

            //event notification
            _eventPublisher.EntityDeleted(link);
        }

        /// <summary>
        /// Gets a link
        /// </summary>
        /// <param name="linkId">The link identifier</param>
        /// <returns>Link</returns>
        public virtual Link GetLinkById(int linkId)
        {
            if (linkId == 0)
                return null;

            return _linkRepository.GetById(linkId);
        }

        /// <summary>
        /// Gets all links
        /// </summary>
        /// <param name="isHome">IsHome identifier; pass 0 to load all records</param>
        /// <returns>Links</returns>
        public virtual IPagedList<Link> GetAllLinks(int languageId, int siteId, int pageIndex, int pageSize, bool isHome = false)
        {
            var query = _linkRepository.Table;
            if (languageId > 0)
            {
                query = query.Where(n => languageId == n.LanguageId);
            }
            if (isHome)
                query = query.Where(p => p.IsHome == isHome);

            query = query.OrderBy(t => t.SortId).ThenBy(t => t.CreatedOnUtc);
            //Site mapping
            if (siteId > 0 && !QuerySettings.IgnoreMultiSite)
            {
                query = from n in query
                        join sm in _siteMappingRepository.Table
                        on new { c1 = n.Id, c2 = "Link" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into n_sm
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

            var links = new PagedList<Link>(query, pageIndex, pageSize);
            return links;
        }

        /// <summary>
        /// Inserts a link
        /// </summary>
        /// <param name="link">Link</param>
        public virtual void InsertLink(Link link)
        {
            if (link == null)
                throw new ArgumentNullException("link");

            _linkRepository.Insert(link);

            //event notification
            _eventPublisher.EntityInserted(link);
        }

        /// <summary>
        /// Updates the link
        /// </summary>
        /// <param name="link">Link</param>
        public virtual void UpdateLink(Link link)
        {
            if (link == null)
                throw new ArgumentNullException("link");

            _linkRepository.Update(link);

            //event notification
            _eventPublisher.EntityUpdated(link);
        }

        #endregion
    }
}

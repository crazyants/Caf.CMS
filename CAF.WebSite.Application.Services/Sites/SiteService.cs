

using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Sites
{

    public partial class SiteService : ISiteService
    {

        #region Constants


        #endregion

        #region Fields
        private readonly IRepository<Site> _siteRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private bool? _isSingleSiteMode = null;
        #endregion

        #region Ctor


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="siteRepository"></param>
        /// <param name="eventPublisher"></param>
        public SiteService(ICacheManager cacheManager,
            IRepository<Site> siteRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._siteRepository = siteRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Utilities

        #endregion

        #region Site

        public IPagedList<Site> GetAllSites(int pageIndex, int pageSize)
        {
            var query = _siteRepository.Table;
            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var Site = new PagedList<Site>(query, pageIndex, pageSize);
            return Site;
        }

        public void DeleteSite(Site Site)
        {
            if (Site == null)
                throw new ArgumentNullException("Site");

            //Site.Deleted = true;
            //UpdateSite(Site);

            _siteRepository.Delete(Site);

            //event notification
            _eventPublisher.EntityDeleted(Site);
        }

        public Site GetSiteById(int SiteId)
        {
            if (SiteId == 0)
                return null;

            var Site = _siteRepository.GetById(SiteId);
            return Site;
        }

        public Site GetSiteByName(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return null;
            var query = from c in _siteRepository.Table
                        where c.Name == Name
                        select c;
            var Site = query.FirstOrDefault();
            return Site;
        }
        public Site GetSiteByEmail(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
                return null;
            var query = from c in _siteRepository.Table
                        where c.Email == Email
                        select c;
            var Site = query.FirstOrDefault();
            return Site;
        }

        public IList<Site> GetSiteByIds(int[] SiteIds)
        {
            if (SiteIds == null || SiteIds.Length == 0)
                return new List<Site>();

            var query = from c in _siteRepository.Table
                        where SiteIds.Contains(c.Id)
                        select c;
            var Site = query.ToList();
            //sort by passed identifiers
            var sortedSite = new List<Site>();
            foreach (int id in SiteIds)
            {
                var site = Site.Find(x => x.Id == id);
                if (site != null)
                    sortedSite.Add(site);
            }
            return sortedSite;
        }

        /// <summary>
        /// Gets all Site
        /// </summary>
        /// <returns>Site</returns>
        public IList<Site> GetAllSites()
        {
            var query = from s in _siteRepository.Table
                        orderby s.CreatedOnUtc
                        select s;
            var site = query.ToList();
            return site;
        }

        public IQueryable<Site> GetAllSiteQ()
        {
            var query = _siteRepository.Table;
            return query;
        }

        public void InsertSite(Site Site)
        {
            if (Site == null)
                throw new ArgumentNullException("Site");

            _siteRepository.Insert(Site);

            //event notification
            _eventPublisher.EntityInserted(Site);
        }
        //public void InsertSite(Site Site)
        //{
        //    var ctx = _siteRepository.Context;
        //    if (Site == null)
        //        throw new ArgumentNullException("Site");
        //    using (var scope = new DbContextScope(ctx: ctx, autoDetectChanges: false, proxyCreation: true, validateOnSave: false))
        //    {
        //        var cmAutoCommit = _siteRepository.AutoCommitEnabled;
               
        //        _siteRepository.AutoCommitEnabled = false;
              
        //        _siteRepository.Insert(Site);
             
        //        // save the rest
        //        scope.Commit();

        //        _siteRepository.AutoCommitEnabled = cmAutoCommit;
              
        //    }
        //    //event notification
        //    _eventPublisher.EntityInserted(Site);
        //}


        /// <summary>
        /// Updates the Site
        /// </summary>
        /// <param name="Site">Site</param>
        public virtual void UpdateSite(Site Site)
        {
            if (Site == null)
                throw new ArgumentNullException("Site");

            _siteRepository.Update(Site);

            //event notification
            _eventPublisher.EntityUpdated(Site);
        }
        /// <summary>
        /// True if there's only one site. Otherwise False.
        /// </summary>
        public virtual bool IsSingleSiteMode()
        {
            if (!_isSingleSiteMode.HasValue)
            {
                var query = from s in _siteRepository.Table
                            select s;
                _isSingleSiteMode = query.Count() <= 1;
            }

            return _isSingleSiteMode.Value;
        }

        /// <summary>
        /// True if the site data is valid. Otherwise False.
        /// </summary>
        /// <param name="site">Store entity</param>
        public virtual bool IsSiteDataValid(Site site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            if (site.Url.IsEmpty())
                return false;

            try
            {
                var uri = new Uri(site.Url);
                var domain = uri.DnsSafeHost.EmptyNull().ToLower();

                switch (domain)
                {
                    case "www.yoursite.com":
                    case "yoursite.com":
                    case "www.mein-shop.de":
                    case "mein-shop.de":
                        return false;
                    default:
                        return site.Url.IsWebUrl();
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        #endregion

        #region  ExtensionMethod

        #endregion

    }
}

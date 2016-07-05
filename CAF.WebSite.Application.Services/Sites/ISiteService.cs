

using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Sites
{

    public partial interface ISiteService
    {

        #region Site

        /// <summary>
        /// Gets all Site 
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Site</returns>
        IPagedList<Site> GetAllSites(int pageIndex, int pageSize);

        /// <summary>
        /// Delete a Site
        /// </summary>
        /// <param name="Site">Site</param>
        void DeleteSite(Site Site);

        /// <summary>
        /// Gets a Site
        /// </summary>
        /// <param name="SiteId">Site identifier</param>
        /// <returns>A Site</returns>
        Site GetSiteById(int SiteId);

        /// <summary>
        /// Get Site by identifiers
        /// </summary>
        /// <param name="SiteIds">Site identifiers</param>
        /// <returns>Site</returns>
        IList<Site> GetSiteByIds(int[] SiteIds);

        /// <summary>
        /// Get Site by identifiers
        /// </summary>
        /// <returns>Site</returns>
        IList<Site> GetAllSites();

        /// <summary>
        /// Get Site by identifiers
        /// </summary>
        /// <returns>Site</returns>
        IQueryable<Site> GetAllSiteQ();

        /// <summary>
        /// Gets a Site
        /// </summary>
        /// <param name="Name">Name</param>
        /// <returns>Site</returns>
        Site GetSiteByName(string Name);
        /// <summary>
        /// Gets a Site
        /// </summary>
        /// <param name="Email">Email</param>
        /// <returns>Site</returns>
        Site GetSiteByEmail(string Email);

        /// <summary>
        /// Insert a Site
        /// </summary>
        /// <param name="Site">Site</param>
        void InsertSite(Site Site);


        /// <summary>
        /// Updates the Site
        /// </summary>
        /// <param name="Site">Site</param>
        void UpdateSite(Site Site);


        /// <summary>
        /// True if there's only one site. Otherwise False.
        /// </summary>
        /// <remarks>codehint: sm-add</remarks>
        bool IsSingleSiteMode();
        /// <summary>
        /// True if the site data is valid. Otherwise False.
        /// </summary>
        /// <param name="site">Store entity</param>
        bool IsSiteDataValid(Site site);
        #endregion

        #region  ExtensionMethod

        #endregion

    }
}

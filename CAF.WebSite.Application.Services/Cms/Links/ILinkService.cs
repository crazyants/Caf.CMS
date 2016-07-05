using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Links
{
    /// <summary>
    /// Link service interface
    /// </summary>
    public partial interface ILinkService
    {
        /// <summary>
        /// Deletes a link
        /// </summary>
        /// <param name="link">Link</param>
        void DeleteLink(Link link);

        /// <summary>
        /// Gets a link
        /// </summary>
        /// <param name="linkId">The link identifier</param>
        /// <returns>Link</returns>
        Link GetLinkById(int linkId);

        /// <summary>
        /// Gets all links
        /// </summary>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>Links</returns>
        IPagedList<Link> GetAllLinks(int languageId, int siteId, int pageIndex, int pageSize, bool isHome = true);

        /// <summary>
        /// Inserts a link
        /// </summary>
        /// <param name="link">Link</param>
        void InsertLink(Link link);

        /// <summary>
        /// Updates the link
        /// </summary>
        /// <param name="link">Link</param>
        void UpdateLink(Link link);
    }
}

using System.Collections.Generic;
using CAF.Infrastructure.Core.Domain.Cms.RegionalContents;
using CAF.Infrastructure.Core.Pages;

namespace CAF.WebSite.Application.Services.RegionalContents
{
    /// <summary>
    /// RegionalContentService service interface
    /// </summary>
    public partial interface IRegionalContentService
    {
        /// <summary>
        /// Deletes a regionalContent
        /// </summary>
        /// <param name="regionalContent">RegionalContent</param>
        void DeleteRegionalContent(RegionalContent regionalContent);

        /// <summary>
        /// Gets a regionalContent
        /// </summary>
        /// <param name="regionalContentId">The regionalContent identifier</param>
        /// <returns>RegionalContent</returns>
        RegionalContent GetRegionalContentById(int regionalContentId);

        /// <summary>
        /// Gets a regionalContent
        /// </summary>
        /// <param name="systemName">The regionalContent system name</param>
        /// <param name="siteId">Site identifier</param>
        /// <returns>RegionalContent</returns>
        RegionalContent GetRegionalContentBySystemName(string systemName, int siteId, int languageId);

        /// <summary>
        /// Gets all regionalContents
        /// </summary>
        /// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <returns>RegionalContents</returns>
        IList<RegionalContent> GetAllRegionalContents(int siteId);

        IPagedList<RegionalContent> GetAllRegionalContents(int languageId, int siteId, int pageIndex, int pageSize);

        /// <summary>
        /// Inserts a regionalContent
        /// </summary>
        /// <param name="regionalContent">RegionalContent</param>
        void InsertRegionalContent(RegionalContent regionalContent);

        /// <summary>
        /// Updates the regionalContent
        /// </summary>
        /// <param name="regionalContent">RegionalContent</param>
        void UpdateRegionalContent(RegionalContent regionalContent);
    }
}


using CAF.Infrastructure.Core.Pages;
using CAF.WebSite.Domain.Cms.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Channels
{

    public partial interface IChannelCategoryService
    {

        #region ChannelCategorys

        /// <summary>
        /// Gets all ChannelCategorys 
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>ChannelCategorys</returns>
        IPagedList<ChannelCategory> GetAllChannelCategorys(int pageIndex, int pageSize);

        /// <summary>
        /// Delete a ChannelCategory
        /// </summary>
        /// <param name="ChannelCategory">ChannelCategory</param>
        void DeleteChannelCategory(ChannelCategory ChannelCategory);

        /// <summary>
        /// Gets a ChannelCategory
        /// </summary>
        /// <param name="ChannelCategoryId">ChannelCategory identifier</param>
        /// <returns>A ChannelCategory</returns>
        ChannelCategory GetChannelCategoryById(int ChannelCategoryId);

        /// <summary>
        /// Get ChannelCategorys by identifiers
        /// </summary>
        /// <param name="ChannelCategoryIds">ChannelCategory identifiers</param>
        /// <returns>ChannelCategorys</returns>
        IList<ChannelCategory> GetChannelCategorysByIds(int[] ChannelCategoryIds);

        /// <summary>
        /// Get ChannelCategorys by identifiers
        /// </summary>
        /// <returns>ChannelCategorys</returns>
        IList<ChannelCategory> GetAllChannelCategorys();

        /// <summary>
        /// Get ChannelCategorys by identifiers
        /// </summary>
        /// <returns>ChannelCategorys</returns>
        IQueryable<ChannelCategory> GetAllChannelCategoryQ();


        /// <summary>
        /// Insert a ChannelCategory
        /// </summary>
        /// <param name="ChannelCategory">ChannelCategory</param>
        void InsertChannelCategory(ChannelCategory ChannelCategory);

        /// <summary>
        /// Updates the ChannelCategory
        /// </summary>
        /// <param name="ChannelCategory">ChannelCategory</param>
        void UpdateChannelCategory(ChannelCategory ChannelCategory);

        #endregion

        #region  ExtensionMethod

        #endregion

    }
}

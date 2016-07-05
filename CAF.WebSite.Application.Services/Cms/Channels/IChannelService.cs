

using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Channels
{

    public partial interface IChannelService
    {

        #region Channels

        /// <summary>
        /// Gets all Channels 
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Channels</returns>
        IPagedList<Channel> GetAllChannels(int pageIndex, int pageSize);

        /// <summary>
        /// Delete a Channel
        /// </summary>
        /// <param name="Channel">Channel</param>
        void DeleteChannel(Channel Channel);

        /// <summary>
        /// Gets a Channel
        /// </summary>
        /// <param name="ChannelId">Channel identifier</param>
        /// <returns>A Channel</returns>
        Channel GetChannelById(int ChannelId);

        /// <summary>
        /// Get Channels by identifiers
        /// </summary>
        /// <param name="ChannelIds">Channel identifiers</param>
        /// <returns>Channels</returns>
        IList<Channel> GetChannelsByIds(int[] ChannelIds);

        /// <summary>
        /// Get Channels by identifiers
        /// </summary>
        /// <returns>Channels</returns>
        IList<Channel> GetAllChannels();

        /// <summary>
        /// Get Channels by identifiers
        /// </summary>
        /// <returns>Channels</returns>
        IQueryable<Channel> GetAllChannelQ();


        /// <summary>
        /// Insert a Channel
        /// </summary>
        /// <param name="Channel">Channel</param>
        void InsertChannel(Channel Channel);

        /// <summary>
        /// Updates the Channel
        /// </summary>
        /// <param name="Channel">Channel</param>
        void UpdateChannel(Channel Channel);

        #endregion

        #region  ExtensionMethod

        #endregion

    }
}

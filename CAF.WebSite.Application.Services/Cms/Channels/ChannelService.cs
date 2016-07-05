
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Channels
{

    public partial class ChannelService : IChannelService
    {

        #region Constants


        #endregion

        #region Fields
        private readonly IRepository<Channel> _channelRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="channelRepository"></param>
        /// <param name="eventPublisher"></param>
        public ChannelService(ICacheManager cacheManager,
            IRepository<Channel> channelRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._channelRepository = channelRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Utilities

        #endregion

        #region Channels

        public IPagedList<Channel> GetAllChannels(int pageIndex, int pageSize)
        {
            var query = _channelRepository.Table;
            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var Channels = new PagedList<Channel>(query, pageIndex, pageSize);
            return Channels;
        }

        public void DeleteChannel(Channel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            // channel.IsDeleted = true;
            // UpdateChannel(channel);

            _channelRepository.Delete(channel);

            //event notification
            _eventPublisher.EntityDeleted(channel);
        }

        public Channel GetChannelById(int channelId)
        {
            if (channelId == 0)
                return null;

            var channel = _channelRepository.GetById(channelId);
            return channel;
        }


        public IList<Channel> GetChannelsByIds(int[] channelIds)
        {
            if (channelIds == null || channelIds.Length == 0)
                return new List<Channel>();

            var query = from c in _channelRepository.Table
                        where channelIds.Contains(c.Id)
                        select c;
            var channels = query.ToList();
            //sort by passed identifiers
            var sortedChannel = new List<Channel>();
            foreach (int id in channelIds)
            {
                var channel = channels.Find(x => x.Id == id);
                if (channel != null)
                    sortedChannel.Add(channel);
            }
            return sortedChannel;
        }

        /// <summary>
        /// Gets all channels
        /// </summary>
        /// <returns>Channels</returns>
        public IList<Channel> GetAllChannels()
        {
            var query = from s in _channelRepository.Table
                        orderby s.CreatedOnUtc
                        select s;
            var channels = query.ToList();
            return channels;
        }

        public IQueryable<Channel> GetAllChannelQ()
        {
            var query = _channelRepository.Table;
            return query;
        }

        public void InsertChannel(Channel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            _channelRepository.Insert(channel);

            //event notification
            _eventPublisher.EntityInserted(channel);
        }


        /// <summary>
        /// Updates the channel
        /// </summary>
        /// <param name="channel">Channel</param>
        public virtual void UpdateChannel(Channel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            _channelRepository.Update(channel);

            //event notification
            _eventPublisher.EntityUpdated(channel);
        }

        #endregion
        #endregion

        #region  ExtensionMethod

        #endregion

    }
}

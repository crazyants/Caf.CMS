
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Pages;
using CAF.WebSite.Domain.Cms.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Channels
{
     
    public partial class ChannelCategoryService : IChannelCategoryService
    {

        #region Constants


        #endregion

        #region Fields
        private readonly IRepository<ChannelCategory> _channelcategoryRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="channelcategoryRepository"></param>
        /// <param name="eventPublisher"></param>
        public ChannelCategoryService(ICacheManager cacheManager,
            IRepository<ChannelCategory> channelcategoryRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._channelcategoryRepository = channelcategoryRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion
       
        #region Methods

        #region Utilities

        #endregion

        #region ChannelCategorys
         
        public IPagedList<ChannelCategory> GetAllChannelCategorys(int pageIndex, int pageSize)
        {
            var query = _channelcategoryRepository.Table;
            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var ChannelCategorys = new PagedList<ChannelCategory>(query, pageIndex, pageSize);
            return ChannelCategorys;
        }

        public void DeleteChannelCategory(ChannelCategory channelcategory)
        {
            if (channelcategory == null)
                throw new ArgumentNullException("channelcategory");
        
            //channelcategory.IsDeleted = true;
            //UpdateChannelCategory(channelcategory);

            _channelcategoryRepository.Delete(channelcategory);

            //event notification
            _eventPublisher.EntityDeleted(channelcategory);
        }

        public ChannelCategory GetChannelCategoryById(int channelcategoryId)
        {
            if (channelcategoryId == 0)
                return null;

            var channelcategory = _channelcategoryRepository.GetById(channelcategoryId);
            return channelcategory;
        }


        public IList<ChannelCategory> GetChannelCategorysByIds(int[] channelcategoryIds)
        {
            if (channelcategoryIds == null || channelcategoryIds.Length == 0)
                return new List<ChannelCategory>();

            var query = from c in _channelcategoryRepository.Table
                        where channelcategoryIds.Contains(c.Id)
                        select c;
            var channelcategorys = query.ToList();
            //sort by passed identifiers
            var sortedChannelCategory = new List<ChannelCategory>();
            foreach (int id in channelcategoryIds)
            {
                var channelcategory = channelcategorys.Find(x => x.Id == id);
                if (channelcategory != null)
                    sortedChannelCategory.Add(channelcategory);
            }
            return sortedChannelCategory;
        }

        /// <summary>
        /// Gets all channelcategorys
        /// </summary>
        /// <returns>ChannelCategorys</returns>
        public IList<ChannelCategory> GetAllChannelCategorys()
        {
            var query = from s in _channelcategoryRepository.Table
                        orderby s.CreatedOnUtc
                        select s;
            var channelcategorys = query.ToList();
            return channelcategorys;
        }

        public IQueryable<ChannelCategory> GetAllChannelCategoryQ()
        {
            var query = _channelcategoryRepository.Table;
            return query;
        }

        public void InsertChannelCategory(ChannelCategory channelcategory)
        {
            if (channelcategory == null)
                throw new ArgumentNullException("channelcategory");

            _channelcategoryRepository.Insert(channelcategory);

            //event notification
            _eventPublisher.EntityInserted(channelcategory);
        }


        /// <summary>
        /// Updates the channelcategory
        /// </summary>
        /// <param name="channelcategory">ChannelCategory</param>
        public virtual void UpdateChannelCategory(ChannelCategory channelcategory)
        {
            if (channelcategory == null)
                throw new ArgumentNullException("channelcategory");

            _channelcategoryRepository.Update(channelcategory);

            //event notification
            _eventPublisher.EntityUpdated(channelcategory);
        }

        #endregion
        #endregion

        #region  ExtensionMethod

        #endregion

    }
}

using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Directory
{
    /// <summary>
    /// DeliveryTime service
    /// </summary>
    public partial class DeliveryTimeService : IDeliveryTimeService
    {
        #region Constants
        private const string DELIVERYTIMES_ALL_KEY = "SMN.deliverytime.all";
        private const string DELIVERYTIMES_PATTERN_KEY = "SMN.deliverytime.";
        #endregion

        #region Fields

        private readonly IRepository<DeliveryTime> _deliveryTimeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IUserService _userService;
        //private readonly CurrencySettings _currencySettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="currencyRepository">DeliveryTime repository</param>
        /// <param name="userService">User service</param>
        /// <param name="currencySettings">Currency settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="eventPublisher">Event published</param>
        public DeliveryTimeService(ICacheManager cacheManager,
            IRepository<DeliveryTime> deliveryTimeRepository,
            IUserService userService,
            IPluginFinder pluginFinder,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._deliveryTimeRepository = deliveryTimeRepository;
            this._userService = userService;
            //this._currencySettings = currencySettings;
            this._pluginFinder = pluginFinder;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes DeliveryTime
        /// </summary>
        /// <param name="currency">DeliveryTime</param>
        public virtual void DeleteDeliveryTime(DeliveryTime deliveryTime)
        {
            if (deliveryTime == null)
                throw new ArgumentNullException("deliveryTime");

            //if (this.IsAssociated(deliveryTime.Id))
            //    throw new WorkException("The delivery time cannot be deleted. It has associated product variants");

            _deliveryTimeRepository.Delete(deliveryTime);

            _cacheManager.RemoveByPattern(DELIVERYTIMES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(deliveryTime);
        }



        /// <summary>
        /// Gets a DeliveryTime
        /// </summary>
        /// <param name="currencyId">DeliveryTime identifier</param>
        /// <returns>DeliveryTime</returns>
        public virtual DeliveryTime GetDeliveryTimeById(int deliveryTimeId)
        {
            if (deliveryTimeId == 0)
                return null;

            return _deliveryTimeRepository.GetById(deliveryTimeId);
        }

        /// <summary>
        /// Gets all delivery times
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>DeliveryTime collection</returns>
        public virtual IList<DeliveryTime> GetAllDeliveryTimes()
        {
            string key = string.Format(DELIVERYTIMES_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = _deliveryTimeRepository.Table;
                query = query.OrderBy(c => c.DisplayOrder);
                var deliveryTimes = query.ToList();
                return deliveryTimes;
            });
        }

        /// <summary>
        /// Inserts a DeliveryTime
        /// </summary>
        /// <param name="deliveryTime">DeliveryTime</param>
        public virtual void InsertDeliveryTime(DeliveryTime deliveryTime)
        {
            if (deliveryTime == null)
                throw new ArgumentNullException("deliveryTime");

            _deliveryTimeRepository.Insert(deliveryTime);

            _cacheManager.RemoveByPattern(DELIVERYTIMES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(deliveryTime);
        }

        /// <summary>
        /// Updates the DeliveryTime
        /// </summary>
        /// <param name="deliveryTime">DeliveryTime</param>
        public virtual void UpdateDeliveryTime(DeliveryTime deliveryTime)
        {
            if (deliveryTime == null)
                throw new ArgumentNullException("deliveryTime");

            _deliveryTimeRepository.Update(deliveryTime);

            _cacheManager.RemoveByPattern(DELIVERYTIMES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(deliveryTime);
        }

        #endregion

    }
}
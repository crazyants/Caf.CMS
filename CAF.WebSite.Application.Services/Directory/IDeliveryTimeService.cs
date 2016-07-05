using CAF.Infrastructure.Core.Domain.Directory;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Directory
{
    /// <summary>
    /// DeliveryTime service
    /// </summary>
    public partial interface IDeliveryTimeService
    {
        
        /// <summary>
        /// Deletes delivery time
        /// </summary>
        /// <param name="deliveryTime">DeliveryTime</param>
        void DeleteDeliveryTime(DeliveryTime deliveryTime);

        /// <summary>
        /// Gets a delivery time
        /// </summary>
        /// <param name="deliveryTimeId">delivery time identifier</param>
        /// <returns>DeliveryTime</returns>
        DeliveryTime GetDeliveryTimeById(int deliveryTimeId);

        /// <summary>
        /// Gets all delivery times
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>delivery time collection</returns>
        IList<DeliveryTime> GetAllDeliveryTimes();

        /// <summary>
        /// Inserts a delivery time
        /// </summary>
        /// <param name="currency">DeliveryTime</param>
        void InsertDeliveryTime(DeliveryTime deliveryTime);

		void UpdateDeliveryTime(DeliveryTime deliveryTime);
    }
}
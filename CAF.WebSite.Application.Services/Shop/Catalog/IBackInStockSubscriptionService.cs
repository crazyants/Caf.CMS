

using CAF.Infrastructure.Core.Pages;
using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
namespace CAF.WebSite.Application.Services.Catalog
{
    /// <summary>
    /// Back in stock subscription service interface
    /// </summary>
    public partial interface IBackInStockSubscriptionService
    {
        /// <summary>
        /// Delete a back in stock subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void DeleteSubscription(BackInStockSubscription subscription);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="userId">User identifier</param>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Subscriptions</returns>
        IPagedList<BackInStockSubscription> GetAllSubscriptionsByUserId(int userId,
			int siteId, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
		/// <param name="productId">Product identifier</param>
		/// <param name="siteId">Site identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Subscriptions</returns>
        IPagedList<BackInStockSubscription> GetAllSubscriptionsByProductId(int productId,
			int siteId, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="userId">User id</param>
		/// <param name="productId">Product identifier</param>
		/// <param name="siteId">Site identifier</param>
        /// <returns>Subscriptions</returns>
		BackInStockSubscription FindSubscription(int userId, int productId, int siteId);

        /// <summary>
        /// Gets a subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription identifier</param>
        /// <returns>Subscription</returns>
        BackInStockSubscription GetSubscriptionById(int subscriptionId);

        /// <summary>
        /// Inserts subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void InsertSubscription(BackInStockSubscription subscription);

        /// <summary>
        /// Updates subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        void UpdateSubscription(BackInStockSubscription subscription);

        /// <summary>
        /// Send notification to subscribers
        /// </summary>
		/// <param name="product">The Product</param>
        /// <returns>Number of sent email</returns>
        int SendNotificationsToSubscribers(Product product);
    }
}

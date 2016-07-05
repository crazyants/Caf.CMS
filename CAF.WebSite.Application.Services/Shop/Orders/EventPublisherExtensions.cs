
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Users;
namespace CAF.WebSite.Application.Services.Orders
{
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// Publishes the order paid event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="order">The order.</param>
        public static void PublishOrderPaid(this IEventPublisher eventPublisher, Order order)
        {
            eventPublisher.Publish(new OrderPaidEvent(order));
        }

        /// <summary>
        /// Publishes the order placed event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="order">The order.</param>
        public static void PublishOrderPlaced(this IEventPublisher eventPublisher, Order order)
        {
            eventPublisher.Publish(new OrderPlacedEvent(order));
        }

		/// <summary>
		/// Publishes the order updated event.
		/// </summary>
		/// <param name="eventPublisher">The event publisher.</param>
		/// <param name="order">The order.</param>
		public static void PublishOrderUpdated(this IEventPublisher eventPublisher, Order order)
		{
			if (order != null)
				eventPublisher.Publish(new OrderUpdatedEvent(order));
		}

		/// <summary>
		/// Publishes the migrate shopping cart event.
		/// </summary>
		/// <param name="eventPublisher">The event publisher.</param>
		/// <param name="fromUser">The source user entity.</param>
		/// <param name="toUser">The destination user entity.</param>
		/// <param name="siteId">Store identifier.</param>
		public static void PublishMigrateShoppingCart(this IEventPublisher eventPublisher, User fromUser, User toUser, int siteId)
		{
			if (fromUser != null && toUser != null)
				eventPublisher.Publish(new MigrateShoppingCartEvent(fromUser, toUser, siteId));
		}
    }
}
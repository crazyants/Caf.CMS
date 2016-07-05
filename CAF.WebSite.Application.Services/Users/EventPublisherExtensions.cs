using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Users;
namespace CAF.WebSite.Application.Services.Users
{
    public static class EventPublisherExtensions
    {


        /// <summary>
        /// Publishes the user placed event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="user">The user.</param>
        public static void PublishUserPlaced(this IEventPublisher eventPublisher, User user)
        {
            eventPublisher.Publish(new UserRegisterEvent(user));
        }

    }
}
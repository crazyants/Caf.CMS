
namespace CAF.Infrastructure.Core.Events
{
    /// <summary>
    /// 事件发布者
    /// </summary>
    public interface IEventPublisher
    {
        void Publish<T>(T eventMessage);
    }
    /// <summary>
    /// 事件发布者扩展
    /// </summary>
	public static class IEventPublisherExtensions
	{
		public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
		{
			eventPublisher.Publish(new EntityInserted<T>(entity));
		}

		public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
		{
			eventPublisher.Publish(new EntityUpdated<T>(entity));
		}

		public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
		{
			eventPublisher.Publish(new EntityDeleted<T>(entity));
		}
	}
}


namespace CAF.Infrastructure.Core.Events
{
    /// <summary>
    /// 空事件发布者
    /// </summary>
	public class NullEventPublisher : IEventPublisher
	{
		private readonly static IEventPublisher s_instance = new NullEventPublisher();

		public static IEventPublisher Instance
		{
			get { return s_instance; }
		}

		public void Publish<T>(T eventMessage)
		{
			// Noop
		}
	}
}

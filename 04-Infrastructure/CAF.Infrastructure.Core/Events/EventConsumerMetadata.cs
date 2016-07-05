using CAF.Infrastructure.Core.Plugins;
using System;

namespace CAF.Infrastructure.Core.Events
{
    /// <summary>
    /// 事件消费者的元数据
    /// </summary>
	public class EventConsumerMetadata
	{
		public bool ExecuteAsync { get; set; }
		public bool IsActive { get; set; }
        public PluginDescriptor PluginDescriptor { get; set; }
	}
}

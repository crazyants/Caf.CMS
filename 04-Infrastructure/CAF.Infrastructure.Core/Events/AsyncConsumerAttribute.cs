using System;

namespace CAF.Infrastructure.Core.Events
{
    /// <summary>
    /// 异步消费者属性
    /// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited=true, AllowMultiple=false)]
	public class AsyncConsumerAttribute : Attribute
	{
	}
}

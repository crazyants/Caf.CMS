using System;

namespace CAF.Infrastructure.Core.Plugins
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
	public sealed class DisplayOrderAttribute : Attribute
	{
		public DisplayOrderAttribute(int displayOrder)
		{
			DisplayOrder = displayOrder;
		}

		public int DisplayOrder { get; set; }
	}
}

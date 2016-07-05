using System;
using System.Web;
using System.Web.Mvc;

namespace CAF.Infrastructure.Core.Events
{
	/// <summary>
	/// to register global filters in Application_Start
    /// 注册 过滤器
	/// </summary>
	/// <remarks>codehint: sm-add</remarks>
	public class AppRegisterGlobalFiltersEvent
	{
		public GlobalFilterCollection Filters { get; set; }
	}
}

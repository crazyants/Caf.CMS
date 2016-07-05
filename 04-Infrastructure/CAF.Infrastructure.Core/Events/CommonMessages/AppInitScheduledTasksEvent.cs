using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
 

namespace CAF.Infrastructure.Core.Events
{
	/// <summary>
	/// to initialize scheduled tasks in Application_Start
    /// 初始化在Application_Start预定任务
	/// </summary>
	/// <remarks>codehint: sm-add</remarks>
	public class AppInitScheduledTasksEvent<T>
	{
        public IList<T> ScheduledTasks { get; set; }
	}
  
}

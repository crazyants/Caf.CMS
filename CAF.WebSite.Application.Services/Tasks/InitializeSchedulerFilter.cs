using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Utilities;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Tasks;

namespace CAF.WebSite.Application.Services.Tasks
{
    public class InitializeSchedulerFilter : IAuthorizationFilter
    {
        private readonly static object s_lock = new object();
        private static bool s_initializing = false;

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            lock (s_lock)
            {
                if (!s_initializing)
                {
                    s_initializing = true;

                    var logger = EngineContext.Current.Resolve<ILogger>();

                    try
                    {
                        var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();
                        var siteService = EngineContext.Current.Resolve<ISiteService>();
                        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
                        var taskScheduler = EngineContext.Current.Resolve<ITaskScheduler>();

                        var tasks = taskService.GetAllTasks(true);
                        taskService.CalculateFutureSchedules(tasks, true /* isAppStart */);

                        var baseUrl = CommonHelper.GetAppSetting<string>("caf:TaskSchedulerBaseUrl");
                        if (baseUrl.IsWebUrl())
                        {
                            taskScheduler.BaseUrl = baseUrl;
                        }
                        else
                        {
                            // autoresolve base url
                            taskScheduler.SetBaseUrl(siteService, filterContext.HttpContext);
                        }

                        taskScheduler.SweepIntervalMinutes = CommonHelper.GetAppSetting<int>("caf:TaskSchedulerSweepInterval", 1);
                        taskScheduler.Start();

                        logger.Information("Initialized TaskScheduler with base url '{0}'".FormatInvariant(taskScheduler.BaseUrl));

                        eventPublisher.Publish(new AppInitScheduledTasksEvent<ScheduleTask> { ScheduledTasks = tasks });
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error while initializing Task Scheduler", ex);
                    }
                    finally
                    {
                        GlobalFilters.Filters.Remove(this);
                    }
                }
            }
        }
    }
}

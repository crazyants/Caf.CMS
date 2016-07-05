using System;
using System.Web;
using System.Linq;
using System.Web.Hosting;
using System.Collections.Generic;
using System.Threading;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Data;
using CAF.Infrastructure.Core;

namespace CAF.WebSite.Application.Services.Tasks
{
    /// <summary>
    /// Task scheduler interface
    /// </summary>
	public interface ITaskScheduler
    {
        /// <summary>
        /// The interval in which the scheduler triggers the sweep url 
		/// (which determines pending tasks and executes them in the scope of a regular HTTP request).
        /// </summary>
		int SweepIntervalMinutes { get; set; }

		/// <summary>
		/// The fully qualified base url
		/// </summary>
        string BaseUrl { get; set; }

		/// <summary>
		///  Gets a value indicating whether the scheduler is active and periodically sweeps all tasks.
		/// </summary>
        bool IsActive { get; }

		/// <summary>
		/// Gets a <see cref="CancellationTokenSource"/> instance used 
		/// to signal a task cancellation request.
		/// </summary>
		/// <param name="scheduleTaskId">A <see cref="ScheduleTask"/> identifier</param>
		/// <returns>A <see cref="CancellationTokenSource"/> instance if the task is running, <c>null</c> otherwise</returns>
		CancellationTokenSource GetCancelTokenSourceFor(int scheduleTaskId);

		/// <summary>
		/// Starts/initializes the scheduler
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the scheduler
		/// </summary>
        void Stop();

		/// <summary>
		/// Executes a single task immediately
		/// </summary>
		/// <param name="scheduleTaskId"></param>
        void RunSingleTask(int scheduleTaskId);

		/// <summary>
		/// Verifies the authentication token which is generated right before the HTTP endpoint gets called.
		/// </summary>
		/// <param name="authToken">The authentication token to verify</param>
		/// <returns><c>true</c> if the validation succeeds, <c>false</c> otherwise</returns>
		/// <remarks>
		/// The task scheduler sends the token as a HTTP request header item.
		/// The called endpoint (e.g. a controller action) is reponsible for invoking
		/// this method and quitting the tasks's execution - preferrably with HTTP 403 -
		/// if the verification fails.
		/// </remarks>
        bool VerifyAuthToken(string authToken);
    }

    public static class ITaskSchedulerExtensions
    {

		internal static void SetBaseUrl(this ITaskScheduler scheduler, ISiteService siteService, HttpContextBase httpContext)
        {
            string url = "";

            if (!httpContext.Request.IsLocal)
            {
                var defaultSite = siteService.GetAllSites().FirstOrDefault(x => siteService.IsSiteDataValid(x));
                if (defaultSite != null)
                {
                    url = defaultSite.Url.EnsureEndsWith("/") + "TaskScheduler";
                }
            }

            if (url.IsEmpty())
            {
				url = WebHelper.GetAbsoluteUrl(VirtualPathUtility.ToAbsolute("~/TaskScheduler"), httpContext.Request);
            }

            scheduler.BaseUrl = url;
        }

    }
}

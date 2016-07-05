using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Utilities;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace CAF.WebSite.Application.Services.Tasks
{
    public class ChangeTaskSchedulerBaseUrlConsumer :
        IConsumer<EntityInserted<Site>>,
        IConsumer<EntityUpdated<Site>>,
        IConsumer<EntityDeleted<Site>>
    {
        private readonly ITaskScheduler _taskScheduler;
        private readonly ISiteService _siteService;
        private readonly HttpContextBase _httpContext;
		private readonly bool _shouldChange;

		public ChangeTaskSchedulerBaseUrlConsumer(ITaskScheduler taskScheduler, ISiteService siteService, HttpContextBase httpContext)
        {
			this._taskScheduler = taskScheduler;
            this._siteService = siteService;
            this._httpContext = httpContext;
			this._shouldChange = CommonHelper.GetAppSetting<string>("caf:TaskSchedulerBaseUrl").IsWebUrl() == false;
        }

        public void HandleEvent(EntityInserted<Site> eventMessage)
        {
			HandleEventCore();
        }

        public void HandleEvent(EntityUpdated<Site> eventMessage)
        {
			HandleEventCore();
        }

        public void HandleEvent(EntityDeleted<Site> eventMessage)
        {
			HandleEventCore();
        }

		private void HandleEventCore() 
		{
			if (_shouldChange)
			{
				_taskScheduler.SetBaseUrl(_siteService, _httpContext);
			}
		}
    }
}

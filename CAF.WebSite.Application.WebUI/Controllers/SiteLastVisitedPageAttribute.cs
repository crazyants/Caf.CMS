using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using System;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Domain.Users;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class SiteLastVisitedPageAttribute : ActionFilterAttribute
    {

		public Lazy<IWebHelper> WebHelper { get; set; }
		public Lazy<IWorkContext> WorkContext { get; set; }
		public Lazy<UserSettings> CustomerSettings { get; set; }
		public Lazy<IGenericAttributeService> GenericAttributeService { get; set; }
		
		public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
			if (!DataSettings.DatabaseIsInstalled())
                return;

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            var customerSettings = CustomerSettings.Value;
            if (!customerSettings.SiteLastVisitedPage)
                return;

            var webHelper = this.WebHelper.Value;
            var pageUrl = webHelper.GetThisPageUrl(true);
            if (!String.IsNullOrEmpty(pageUrl))
            {
                var workContext = WorkContext.Value;
                var genericAttributeService = GenericAttributeService.Value;

                var previousPageUrl = workContext.CurrentUser.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);
                if (!pageUrl.Equals(previousPageUrl))
                {
                    genericAttributeService.SaveAttribute(workContext.CurrentUser, SystemUserAttributeNames.LastVisitedPage, pageUrl);
                }
            }
        }
    }
}

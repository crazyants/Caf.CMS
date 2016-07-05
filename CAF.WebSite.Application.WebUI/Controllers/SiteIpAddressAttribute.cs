using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using System;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Users;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class SiteIpAddressAttribute : ActionFilterAttribute
    {

		public Lazy<IWebHelper> WebHelper { get; set; }
		public Lazy<IWorkContext> WorkContext { get; set; }

		public Lazy<IUserService> UserService { get; set; }
		
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

            var webHelper = this.WebHelper.Value;

            //update IP address
            string currentIpAddress = webHelper.GetCurrentIpAddress();
            if (!String.IsNullOrEmpty(currentIpAddress))
            {
                var workContext = WorkContext.Value;
                var user = workContext.CurrentUser;
                if (!currentIpAddress.Equals(user.LastIpAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userService = UserService.Value;
                    user.LastIpAddress = currentIpAddress;
                    userService.UpdateUser(user);
                }
            }
        }
    }
}

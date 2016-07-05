using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core.Data;
using System;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Users;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class UserLastActivityAttribute : ActionFilterAttribute
    {
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

            var workContext = WorkContext.Value;
            var user = workContext.CurrentUser;
            
            //update last activity date
            if (user.LastActivityDateUtc.AddMinutes(1.0) < DateTime.UtcNow)
            {
                var userService = UserService.Value;
                user.LastActivityDateUtc = DateTime.UtcNow;
                userService.UpdateUser(user);
            }
        }
    }
}

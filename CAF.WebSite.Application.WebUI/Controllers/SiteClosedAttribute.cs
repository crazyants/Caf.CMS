using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using System;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Users;
using System.Collections.Generic;
using System.Web;
using CAF.Infrastructure.Core.Domain;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class SiteClosedAttribute : ActionFilterAttribute
    {
		private static readonly List<Tuple<string, string>> s_permittedRoutes = new List<Tuple<string, string>> 
		{
 			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "Login"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "Logout"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.HomeController", "SiteClosed"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CommonController", "SetLanguage")
		};


		public Lazy<IWorkContext> WorkContext { get; set; }
		public Lazy<SiteInformationSettings> SiteInformationSettings { get; set; }
		
		public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

			//don't apply filter to child methods
			if (filterContext.IsChildAction)
				return;

            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            string controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

			if (!DataSettings.DatabaseIsInstalled())
                return;

            var storeInformationSettings = SiteInformationSettings.Value;
            if (!storeInformationSettings.SiteClosed)
                return;

            if (!IsPermittedRoute(controllerName, actionName)) 
			{
                if (storeInformationSettings.SiteClosedAllowForAdmins && WorkContext.Value.CurrentUser.IsAdmin())
                {
                    //do nothing - allow admin access
                }
                else
                {
                    var storeClosedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl("SiteClosed");
                    filterContext.Result = new RedirectResult(storeClosedUrl);
                }
            }
        }

		private static bool IsPermittedRoute(string controllerName, string actionName)
		{
			foreach (var route in s_permittedRoutes)
			{
				if (controllerName.IsCaseInsensitiveEqual(route.Item1) && actionName.IsCaseInsensitiveEqual(route.Item2))
				{
					return true;
				}
			}

			return false;
		}
    }
}

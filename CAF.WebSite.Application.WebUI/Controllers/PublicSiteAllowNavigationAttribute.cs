using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Security;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class PublicSiteAllowNavigationAttribute : ActionFilterAttribute
    {
		private static readonly List<Tuple<string, string>> s_permittedRoutes = new List<Tuple<string, string>> 
		{
 			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "Login"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "Logout"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "Register"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "PasswordRecovery"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "PasswordRecoveryConfirm"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "AccountActivation"),
			new Tuple<string, string>("CAF.WebSite.Mvc.Controllers.CustomerController", "CheckUsernameAvailability")
		};

		public Lazy<IPermissionService> PermissionService { get; set; }
		
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

			var permissionService = PermissionService.Value;
            var publicSiteAllowNavigation = permissionService.Authorize(StandardPermissionProvider.PublicSiteAllowNavigation);
            if (!publicSiteAllowNavigation && !IsPermittedRoute(controllerName, actionName))
            {
                filterContext.Result = new HttpUnauthorizedResult();
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

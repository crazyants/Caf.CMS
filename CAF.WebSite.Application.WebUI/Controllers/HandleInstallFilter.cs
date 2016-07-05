using CAF.Infrastructure.Core.Data;
using System.Web.Mvc;
using System.Web.Routing;
 

namespace CAF.WebSite.Application.WebUI.Controllers
{ 
    public class HandleInstallFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;

            if (controllerName == "Install" && actionName != "Index" && filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // probably "progress" or "finalize" call
                return;
            }

            if (!DataSettings.DatabaseIsInstalled() && controllerName != "Install")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "Controller", "Install" }, 
                        { "Action", "Index" } 
                    });
            }
        }
        
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // nada
        }

    }

}

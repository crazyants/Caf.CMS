using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.Application.WebUI.Controllers
{

    public abstract partial class PluginControllerBase : BaseController
    {
        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="requestContext">Request context</param>
        protected override void Initialize(RequestContext requestContext)
        {
            //set work context to admin mode
            EngineContext.Current.Resolve<IWorkContext>().IsAdmin = true;

            base.Initialize(requestContext);
        }

        /// <summary>
        /// Renders default access denied view as a partial
        /// </summary>
        /// <remarks>codehint: caf-add</remarks>
        protected ActionResult AccessDeniedPartialView()
        {
            return PartialView("~/Administration/Views/Security/AccessDenied.cshtml");
        }

    }
}

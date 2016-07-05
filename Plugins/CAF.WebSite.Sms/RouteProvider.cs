using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.Sms
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("CAF.WebSite.Sms",
				 "Plugins/CAF.WebSite.Sms/{action}",
                 new { controller = "Sms", action = "Configure" },
                 new[] { "CAF.WebSite.Sms.Controllers" }
            )
			.DataTokens["area"] = "CAF.WebSite.Sms";
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}

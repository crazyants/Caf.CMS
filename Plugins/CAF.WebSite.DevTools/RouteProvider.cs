using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.DevTools
{
    
	public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
			routes.MapRoute("CAF.WebSite.DevTools",
				 "Plugin/CAF.WebSite.DevTools/{action}/{id}",
				 new { controller = "DevTools", action = "Configure", id = UrlParameter.Optional },
				 new[] { "CAF.WebSite.DevTools.Controllers" }
			)
			.DataTokens["area"] = "CAF.WebSite.DevTools";

			//routes.MapRoute("CAF.WebSite.DevTools.MyCheckout",
			//	 "MyCheckout/{action}",
			//	 new { controller = "MyCheckout", action = "MyBillingAddress" },
			//	 new[] { "CAF.WebSite.DevTools.Controllers" }
			//)
			//.DataTokens["area"] = "CAF.WebSite.DevTools";
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

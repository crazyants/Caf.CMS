
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System;
using System.Web.Mvc;
using System.Web.Routing;
namespace CAF.WebSite.CustomBanner
{
	public class RouteProvider : IRouteProvider
	{
		public int Priority
		{
			get
			{
				return 0;
			}
		}
		public void RegisterRoutes(RouteCollection routes)
		{
            RouteCollectionExtensions.MapRoute(routes, "CAF.WebSite.CustomBanner", "Plugins/CustomBanner/{action}", new
			{
				controller = "CustomBanner",
				action = "Configure"
			}, new string[]
			{
				"CAF.WebSite.CustomBanner.Controllers"
			}).DataTokens["area"] = "CAF.WebSite.CustomBanner";
		}
	}
}

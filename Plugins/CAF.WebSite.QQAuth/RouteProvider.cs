using CAF.WebSite.Application.WebUI.Mvc.Routes;
using CAF.WebSite.QQAuth.Core;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.QQAuth
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("CAF.WebSite.QQAuth",
                 "Plugins/CAF.WebSite.QQAuth/{action}",
                 new { controller = "ExternalAuthQQ" },
                 new[] { "CAF.WebSite.QQAuth.Controllers" }
            )
            .DataTokens["area"] = Provider.SystemName;
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

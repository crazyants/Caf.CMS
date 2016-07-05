
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;


namespace CAF.WebSite.Mvc.Admin.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var idConstraint = new MinRouteConstraint(1);

            var route = routes.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "Admin", id = "" },
                new[] { "CAF.WebSite.Mvc.Admin.Controllers" }
            );
            route.DataTokens["area"] = "Admin";

            var routeCategory = routes.MapLocalizedRoute("CustomerProfile",
              "Admin/Category/{id}",
              new { controller = "Category", action = "Index", area = "Admin" },
              new { id = idConstraint },
              new[] { "CAF.WebSite.Mvc.Admin.Controllers" }
              );
            routeCategory.DataTokens["area"] = "Admin";
        }

        public int Priority
        {
            get { return 1000; }
        }
    }
}
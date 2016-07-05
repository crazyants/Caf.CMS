using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Mvc.Controllers;
using CAF.WebSite.Application.WebUI.Seo;


namespace CAF.WebSite.Mvc.Infrastructure
{
    public partial class GeneralRoutes : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {

            //   routes.Add(
            //  "@DomainRoute", new DomainRoute(
            //  "myhost.com",
            //  "{controller}/{action}/{id}",
            //  new { controller = "Home", action = "Index", id = UrlParameter.Optional }

            //));
            //   routes.Add(
            //    "wwwDomainRoute", new DomainRoute(
            //    "www.myhost.com",
            //    "{controller}/{action}/{id}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //  ));


            routes.MapLocalizedRoute(
                "Default_Localized",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { controller = new IsKnownController() },
                new[] { "CAF.WebSite.Mvc.Controllers" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { controller = new IsKnownController() },
                new[] { "CAF.WebSite.Mvc.Controllers" }
            );
            //routes.Add(
            //              "DomainRoute", new DomainRoute(
            //              "{CityNameUrl}.myhost.com",
            //              "{controller}/{action}/{id}",
            //            new { CityNameUrl = "", controller = "Domain", action = "Index", id = "" }
            //            ));
        }

        public int Priority
        {
            get { return -999; }
        }
    }

    internal class IsKnownController : IRouteConstraint
    {
        private readonly static HashSet<string> s_knownControllers = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        static IsKnownController()
        {
            var assembly = typeof(HomeController).Assembly;
            var controllerTypes = from t in assembly.GetExportedTypes()
                                  where typeof(IController).IsAssignableFrom(t) && t.Namespace == "CAF.WebSite.Mvc.Controllers"
                                  select t;

            foreach (var type in controllerTypes)
            {
                var name = type.Name.Substring(0, type.Name.Length - 10);
                s_knownControllers.Add(name);
            }
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(parameterName, out value))
            {
                var requestedController = Convert.ToString(value);
                if (s_knownControllers.Contains(requestedController))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Linq;
using CAF.WebSite.Application.WebUI.Localization;
using System.Collections.Generic;
using CAF.WebSite.Application.WebUI.Mvc.Routes;
using System.Web.Mvc.Routing.Constraints;

namespace CAF.WebSite.Mvc.Infrastructure
{
    public partial class MvcCaptchaRoutes : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var idConstraint = new MinRouteConstraint(1);

            //routes.MapLocalizedRoute("DynamicStart",
            //    "dynamiccaptcha/start",
            //    new { controller = "DynamicCaptcha", action = "CaptchaStart", numberOfImages = UrlParameter.Optional },
            //    new[] { "CAF.WebSite.Application.WebUI.MvcCaptcha" });


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

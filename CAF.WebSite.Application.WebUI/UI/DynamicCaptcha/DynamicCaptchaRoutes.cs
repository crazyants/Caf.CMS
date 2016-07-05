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
    public partial class DynamicCaptchaRoutes : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var idConstraint = new MinRouteConstraint(1);

            routes.MapLocalizedRoute("DynamicStart",
                "dynamiccaptcha/start",
                new { controller = "DynamicCaptcha", action = "CaptchaStart", numberOfImages = UrlParameter.Optional },
                new[] { "CAF.WebSite.Application.WebUI.DynamicCaptcha" });

            routes.MapLocalizedRoute("Image",
            "dynamiccaptcha/image",
             new { controller = "DynamicCaptcha", action = "CaptchaImage", imageIndex = UrlParameter.Optional },
             new[] { "CAF.WebSite.Application.WebUI.DynamicCaptcha" });

            routes.MapLocalizedRoute("Audio",
             "dynamiccaptcha/audio",
             new { controller = "DynamicCaptcha", action = "CaptchaAudio", index = UrlParameter.Optional },
             new[] { "CAF.WebSite.Application.WebUI.DynamicCaptcha" });

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

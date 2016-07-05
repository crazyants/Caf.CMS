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
using CAF.WebSite.Application.WebUI.Seo;

namespace CAF.WebSite.Mvc.Infrastructure.Routes
{
    public partial class SeoRoutes : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                "{*generic_se_name}",
                new { controller = "Common", action = "GenericUrl" },
                new[] { "CAF.WebSite.Mvc.Controllers" });

            // Routes solely needed for URL creation, NOT for route matching.
            routes.MapLocalizedRoute("Article",
                "{SeName}",
                new { controller = "Article", action = "Article" },
              new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ArticleCategory",
                "{SeName}",
                new { controller = "ArticleCatalog", action = "Category" },
                new[] { "CAF.WebSite.Mvc.Controllers" });
            //routes.MapLocalizedRoute("Topic",
            //    "{SeName}",
            //    new { controller = "Topic", action = "Topic" },
            //    new[] { "CAF.WebSite.Mvc.Controllers" });
            // TODO: actually this one's never reached, because the "GenericUrl" route
            // at the top handles this.
            routes.MapLocalizedRoute("PageNotFound",
                "{*path}",
                new { controller = "Error", action = "NotFound" },
                 new[] { "CAF.WebSite.Mvc.Controllers" });

 
        }

        public int Priority
        {
            get
            {
                return int.MinValue;
            }
        }
    }
}
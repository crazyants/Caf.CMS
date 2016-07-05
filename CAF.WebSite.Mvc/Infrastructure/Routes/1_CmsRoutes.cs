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
    public partial class CmsRoutes : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var idConstraint = new MinRouteConstraint(1);

            /* Common
            ----------------------------------------*/

            routes.MapLocalizedRoute("HomePage",
                "",
                new { controller = "Home", action = "Index" },
                new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("Register",
            "register/",
            new { controller = "Member", action = "Register" },
              new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("Login",
                "login/",
                new { controller = "Member", action = "Login" },
                new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ContactUs",
                "contactus",
                new { controller = "Home", action = "ContactUs" },
                new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("Feedback",
             "feedback",
             new { controller = "Home", action = "Feedback" },
             new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("Logout",
                "logout/",
                new { controller = "Member", action = "Logout" },
                 new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("Topic",
                "t/{SystemName}",
                new { controller = "Topic", action = "TopicDetails" },
                new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("TopicPopup",
             "t-popup/{SystemName}",
              new { controller = "Topic", action = "TopicDetailsPopup" },
              new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ChangeDevice",
              "changedevice/{dontusemobileversion}",
              new { controller = "Common", action = "ChangeDevice" },
              new[] { "CAF.WebSite.Mvc.Controllers" });


            routes.MapRoute("ChangeLanguage",
                "changelanguage/{langid}",
                new { controller = "Common", action = "SetLanguage" },
                new { langid = idConstraint },
                new[] { "CAF.WebSite.Mvc.Controllers" });

            /* Newsletter
        ----------------------------------------*/
            routes.MapLocalizedRoute("NewsletterActivation",
                "newsletter/subscriptionactivation/{token}/{active}",
                new { controller = "Newsletter", action = "SubscriptionActivation" },
                new { token = new GuidConstraint(false) },
                          new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("SubscribeNewsletter", // COMPAT: subscribenewsletter >> newsletter/subscribe
                "Newsletter/Subscribe",
                new { controller = "Newsletter", action = "Subscribe" },
                           new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ArticleSearch",
            "search/",
            new { controller = "ArticleCatalog", action = "Search" },
             new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ArticleSearchAutoComplete",
                "ArticleCatalog/searchtermautocomplete",
                new { controller = "ArticleCatalog", action = "SearchTermAutoComplete" },
                   new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ArticlesByTag",
            "articlestag/{articleTagId}/{SeName}",
            new { controller = "ArticleCatalog", action = "ArticlesByTag", SeName = UrlParameter.Optional },
            new { articleTagId = idConstraint },
            new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ArticleTagsAll",
                "articletag/all/",
                new { controller = "ArticleCatalog", action = "ArticlesTagsAll" },
                 new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("ClientList",
            "client/all/",
            new { controller = "ArticleCatalog", action = "ClientAll" },
             new[] { "CAF.WebSite.Mvc.Controllers" });



            /* Misc
        ----------------------------------------*/

            routes.MapLocalizedRoute("RegisterResult",
                "registerresult/{resultId}",
                new { controller = "Member", action = "RegisterResult" },
                new { resultId = idConstraint },
               new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("Sitemap",
              "sitemap",
              new { controller = "Home", action = "Sitemap" },
              new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("SitemapSEO",
                "sitemap.xml",
                new { controller = "Home", action = "SitemapSeo" },
                new[] { "CAF.WebSite.Mvc.Controllers" });
            routes.MapLocalizedRoute("SiteClosed",
            "siteclosed",
            new { controller = "Home", action = "SiteClosed" },
             new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapRoute("robots.txt",
                "robots.txt",
                new { controller = "Common", action = "RobotsTextFile" },
              new[] { "CAF.WebSite.Mvc.Controllers" });

            routes.MapLocalizedRoute("Settings",
                "settings",
                new { controller = "Common", action = "Settings" },
               new[] { "CAF.WebSite.Mvc.Controllers" });


            routes.MapLocalizedRoute("Captcha",
                "Captcha/",
                new { controller = "DefaultCaptcha", action = "Generate" },
                 new[] { "CaptchaMvc.Controllers" });

            routes.MapLocalizedRoute("RefreshCaptcha",
             "RefreshCaptcha/",
             new { controller = "DefaultCaptcha", action = "Refresh" },
              new[] { "CaptchaMvc.Controllers" });



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

using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.Mvc.Admin.Models.Themes
{
    public class ConfigureThemeModel : ModelBase
    {
        public string ThemeName { get; set; }

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }

		[LangResourceDisplayName("Admin.Common.Site")]
		public int SiteId { get; set; }
		public IList<SelectListItem> AvailableSites { get; set; }
    }
}
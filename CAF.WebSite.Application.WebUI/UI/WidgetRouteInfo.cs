using CAF.WebSite.Application.WebUI.Mvc;
using System.Web.Routing;

namespace CAF.WebSite.Application.WebUI.UI
{
    public partial class WidgetRouteInfo : ModelBase
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
		public int Order { get; set; }
    }
}
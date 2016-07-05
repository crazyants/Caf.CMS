using CAF.WebSite.Application.WebUI.Mvc;
using System.Web.Routing;
 

namespace CAF.WebSite.Mvc.Models.Users
{
    public partial class ExternalAuthenticationMethodModel : ModelBase
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}
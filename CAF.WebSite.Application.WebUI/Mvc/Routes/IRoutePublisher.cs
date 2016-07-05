using System.Web.Routing;

namespace CAF.WebSite.Application.WebUI.Mvc.Routes
{
    public interface IRoutePublisher
    {
        void RegisterRoutes(RouteCollection routeCollection);
    }
}

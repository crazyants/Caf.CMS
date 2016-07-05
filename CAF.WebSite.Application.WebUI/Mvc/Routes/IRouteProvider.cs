using System.Web.Routing;

namespace CAF.WebSite.Application.WebUI.Mvc.Routes
{
    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }
    }
}

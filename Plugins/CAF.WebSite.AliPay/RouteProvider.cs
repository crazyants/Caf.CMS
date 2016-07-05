using System.Web.Mvc;
using System.Web.Routing;
 
using CAF.WebSite.Application.WebUI.Mvc.Routes;
 

namespace CAF.WebSite.AliPay
{
	public class RouteProvider : IRouteProvider
	{
		public void RegisterRoutes(RouteCollection routes)
		{
            routes.MapRoute("CAF.WebSite.AliPay.Notify",
                "Plugins/PaymentAliPay/Notify",
                new { controller = "PaymentAliPay", action = "Notify" },
              new[] { "CAF.WebSite.AliPay.Controllers" }
               ).DataTokens["area"] = "CAF.WebSite.AliPay"; 

            //Notify
            routes.MapRoute("CAF.WebSite.AliPay.Return",
                 "Plugins/PaymentAliPay/Return",
                 new { controller = "PaymentAliPay", action = "Return" },
                new[] { "CAF.WebSite.AliPay.Controllers" }
            ).DataTokens["area"] = "CAF.WebSite.AliPay"; 
		}

		public int Priority { get { return 0; } }
	}
}
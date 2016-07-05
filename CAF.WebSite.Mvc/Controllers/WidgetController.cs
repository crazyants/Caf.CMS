using CAF.WebSite.Application.WebUI.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Controllers
{
    /// <summary>
    /// 站内广告位
    /// </summary>
    public partial class WidgetController : PublicControllerBase
    {
        [ChildActionOnly]
        public ActionResult WidgetsByZone(IEnumerable<WidgetRouteInfo> widgets)
        {
            return PartialView(widgets);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CAF.WebSite.Application.WebUI.UI
{
    public interface IWidgetSelector
    {
		IEnumerable<WidgetRouteInfo> GetWidgets(string widgetZone, object model);
    }
}

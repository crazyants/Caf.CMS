using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Collections.Generic;
 

namespace CAF.WebSite.Application.WebUI.UI
{
	public class WidgetZoneModel : ModelBase
	{
		public IEnumerable<WidgetRouteInfo> Widgets { get; set; }
		public string WidgetZone { get; set; }
		public object Model { get; set; }
	}
}
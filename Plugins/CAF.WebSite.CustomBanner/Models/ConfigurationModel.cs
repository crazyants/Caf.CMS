
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
namespace CAF.WebSite.CustomBanner.Models
{
	public class ConfigurationModel : ModelBase
	{
		[LangResourceDisplayName("Plugins.Widget.CustomBanner.MaxBannerHeight", "MaxBannerHeight")]
		public int MaxBannerHeight
		{
			get;
			set;
		}
		[LangResourceDisplayName("Plugins.Widget.CustomBanner.StretchPicture", "StretchPicture")]
		public bool StretchPicture
		{
			get;
			set;
		}
		[LangResourceDisplayName("Plugins.Widget.CustomBanner.ShowBorderTop", "ShowBorderTop")]
		public bool ShowBorderTop
		{
			get;
			set;
		}
		[LangResourceDisplayName("Plugins.Widget.CustomBanner.ShowBorderBottom", "ShowBorderBottom")]
		public bool ShowBorderBottom
		{
			get;
			set;
		}
		[LangResourceDisplayName("Plugins.Widget.CustomBanner.BorderTopColor", "BorderTopColor")]
		public string BorderTopColor
		{
			get;
			set;
		}
		[LangResourceDisplayName("Plugins.Widget.CustomBanner.BorderBottomColor", "BorderBottomColor")]
		public string BorderBottomColor
		{
			get;
			set;
		}
	}
}

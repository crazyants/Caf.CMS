using CAF.Infrastructure.Core.Configuration;
using System;
namespace CAF.WebSite.CustomBanner.Settings
{
	public class CustomBannerSettings : ISettings
	{
		public int MaxBannerHeight
		{
			get;
			set;
		}
		public bool StretchPicture
		{
			get;
			set;
		}
		public bool ShowBorderTop
		{
			get;
			set;
		}
		public bool ShowBorderBottom
		{
			get;
			set;
		}
		public string BorderTopColor
		{
			get;
			set;
		}
		public string BorderBottomColor
		{
			get;
			set;
		}
	}
}

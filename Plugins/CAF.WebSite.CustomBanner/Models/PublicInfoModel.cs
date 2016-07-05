
using CAF.WebSite.Application.WebUI.Mvc;
using System;
namespace CAF.WebSite.CustomBanner.Models
{
	public class PublicInfoModel : ModelBase
	{
		public string PicturePath
		{
			get;
			set;
		}
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

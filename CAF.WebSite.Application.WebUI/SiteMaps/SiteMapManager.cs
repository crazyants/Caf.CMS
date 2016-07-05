using System;
namespace CAF.WebSite.Application.WebUI
{
	public static class SiteMapManager
	{
		private static readonly SiteMapDictionary siteMaps = new SiteMapDictionary();
		public static SiteMapDictionary SiteMaps
		{
			get
			{
				return SiteMapManager.siteMaps;
			}
		}
		internal static void Clear()
		{
			SiteMapManager.SiteMaps.Clear();
		}
	}
}

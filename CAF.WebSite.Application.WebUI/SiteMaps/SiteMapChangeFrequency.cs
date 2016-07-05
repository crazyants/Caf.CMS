using System;
namespace CAF.WebSite.Application.WebUI
{
	[Serializable]
	public enum SiteMapChangeFrequency
	{
		Automatic,
		Daily,
		Always,
		Hourly,
		Weekly,
		Monthly,
		Yearly,
		Never
	}
}

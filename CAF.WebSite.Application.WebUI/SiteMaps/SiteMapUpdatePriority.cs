using System;
namespace CAF.WebSite.Application.WebUI
{
	[Serializable]
	public enum SiteMapUpdatePriority
	{
		Automatic,
		Low = 30,
		Normal = 50,
		High = 80,
		Critical = 100
	}
}

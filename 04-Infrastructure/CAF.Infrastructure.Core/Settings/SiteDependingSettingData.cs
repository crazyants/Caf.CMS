using System.Collections.Generic;

namespace CAF.Infrastructure.Core.Settings
{
	/// <remarks>codehint: sm-add</remarks>
	public class SiteDependingSettingData
	{
		public SiteDependingSettingData()
		{
			OverrideSettingKeys = new List<string>();
		}

		public int ActiveSiteScopeConfiguration { get; set; }
		public List<string> OverrideSettingKeys { get; set; }
		public string RootSettingClass { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAF.Infrastructure.Core.Configuration;

namespace CAF.WebSite.DevTools
{
	public class ProfilerSettings : ISettings
	{
		public bool EnableMiniProfilerInPublicStore { get; set; }
	}
}
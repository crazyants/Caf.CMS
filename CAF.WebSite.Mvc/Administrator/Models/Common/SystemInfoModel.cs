using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Admin.Models.Common
{
    public class SystemInfoModel : ModelBase
    {
        public SystemInfoModel()
        {
            this.LoadedAssemblies = new List<LoadedAssembly>();
        }

        [LangResourceDisplayName("Admin.System.SystemInfo.ASPNETInfo")]
        public string AspNetInfo { get; set; }

        [LangResourceDisplayName("Admin.System.SystemInfo.IsFullTrust")]
        public string IsFullTrust { get; set; }

        [LangResourceDisplayName("Admin.System.SystemInfo.AppVersion")]
        public string AppVersion { get; set; }

		[LangResourceDisplayName("Admin.System.SystemInfo.AppDate")]
		public DateTime AppDate { get; set; }

        [LangResourceDisplayName("Admin.System.SystemInfo.OperatingSystem")]
        public string OperatingSystem { get; set; }

        [LangResourceDisplayName("Admin.System.SystemInfo.ServerLocalTime")]
        public DateTime ServerLocalTime { get; set; }

        [LangResourceDisplayName("Admin.System.SystemInfo.ServerTimeZone")]
        public string ServerTimeZone { get; set; }

        [LangResourceDisplayName("Admin.System.SystemInfo.UTCTime")]
        public DateTime UtcTime { get; set; }

		[LangResourceDisplayName("Admin.System.SystemInfo.HTTPHOST")]
		public string HttpHost { get; set; }

        [LangResourceDisplayName("Admin.System.SystemInfo.LoadedAssemblies")]
        public IList<LoadedAssembly> LoadedAssemblies { get; set; }

		[LangResourceDisplayName("Admin.System.SystemInfo.DatabaseSize")]
		public double DatabaseSize { get; set; }
		public string DatabaseSizeString
		{
			get
			{
				return (DatabaseSize == 0.0 ? "" : "{0:0.00} MB".FormatWith(DatabaseSize));
			}
		}

		[LangResourceDisplayName("Admin.System.SystemInfo.DataProviderFriendlyName")]
		public string DataProviderFriendlyName { get; set; }

        public class LoadedAssembly : ModelBase
        {
            public string FullName { get; set; }
            public string Location { get; set; }
        }

    }
}
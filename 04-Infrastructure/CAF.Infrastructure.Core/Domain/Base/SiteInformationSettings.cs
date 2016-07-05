
using CAF.Infrastructure.Core.Configuration;

namespace CAF.Infrastructure.Core.Domain
{
    public class SiteInformationSettings : ISettings
    {
		public SiteInformationSettings()
		{
			SiteClosedAllowForAdmins = true;
		}
		
		/// <summary>
        /// Gets or sets a value indicating whether store is closed
        /// </summary>
        public bool SiteClosed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether administrators can visit a closed store
        /// </summary>
        public bool  SiteClosedAllowForAdmins { get; set; }
        /// <summary>
        /// 开启站点内容共享，开启：内容编辑时可选择共享的站点；关闭：即默认当前站点
        /// </summary>
        public bool SiteContentShare { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed in public store (used for debugging)
        /// </summary>
        public bool DisplayMiniProfilerInPublicSite { get; set; }
    }
}

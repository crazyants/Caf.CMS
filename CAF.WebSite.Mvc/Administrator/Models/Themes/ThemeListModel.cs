using CAF.WebSite.Application.WebUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Themes
{
    public class ThemeListModel
    {
        public ThemeListModel()
        {
            this.AvailableBundleOptimizationValues = new List<SelectListItem>();
            this.DesktopThemes = new List<ThemeManifestModel>();
            this.MobileThemes = new List<ThemeManifestModel>();
			this.AvailableSites = new List<SelectListItem>();
        }

        public IList<SelectListItem> AvailableBundleOptimizationValues { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Themes.Option.BundleOptimizationEnabled")]
        public int BundleOptimizationEnabled { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Themes.Option.DefaultDesktopTheme")]
        public string DefaultDesktopTheme { get; set; }
        public IList<ThemeManifestModel> DesktopThemes { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Themes.Option.DefaultMobileTheme")]
        public string DefaultMobileTheme { get; set; }
        public IList<ThemeManifestModel> MobileThemes { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Themes.Option.AllowCustomerToSelectTheme")]
        public bool AllowCustomerToSelectTheme { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Themes.Option.SaveThemeChoiceInCookie")]
		public bool SaveThemeChoiceInCookie { get; set; }

        [LangResourceDisplayName("Admin.Configuration.Themes.Option.MobileDevicesSupported")]
        public bool MobileDevicesSupported { get; set; }

		[LangResourceDisplayName("Admin.Common.Site")]
		public int SiteId { get; set; }
		public IList<SelectListItem> AvailableSites { get; set; }
    }
}
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Cms;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.CustomBanner.Data.Migrations;
using CAF.WebSite.CustomBanner.Settings;
using CAF.Infrastructure.Core.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Web.Routing;
namespace CAF.WebSite.CustomBanner
{
	
	public class Plugin : BasePlugin, IWidget, IProvider, IUserEditable, IConfigurable
	{
        private readonly ILogger _logger;
		private readonly ILocalizationService _localizationService;
		private readonly ISettingService _settingService;
        public Plugin(ILogger logger, ISettingService settingService, ILocalizationService localizationService)
		{
            this._logger = logger;
			this._settingService = settingService;
			this._localizationService = localizationService;
		}
		public override void Install()
		{
			this._localizationService.ImportPluginResourcesFromXml(this.PluginDescriptor, null, true, null);
			CustomBannerSettings customBannerSettings = new CustomBannerSettings();
			customBannerSettings.MaxBannerHeight = 150;
			customBannerSettings.StretchPicture = true;
			customBannerSettings.ShowBorderBottom = true;
			customBannerSettings.ShowBorderTop = true;
			customBannerSettings.BorderTopColor = "#ccc";
			customBannerSettings.BorderBottomColor = "#ccc";
			this._settingService.SaveSetting<CustomBannerSettings>(customBannerSettings, 0);
			base.Install();
            _logger.Information(string.Format("Plugin installed: SystemName: {0}, Version: {1}, Description: '{2}'", PluginDescriptor.SystemName, PluginDescriptor.Version, PluginDescriptor.FriendlyName));
		}
		public override void Uninstall()
		{
			this._localizationService.DeleteLocaleStringResources(this.PluginDescriptor.ResourceRootKey, true);
			this._localizationService.DeleteLocaleStringResources("Plugins.FriendlyName.Widgets.CustomBanner", true);
			this._settingService.DeleteSetting<CustomBannerSettings>();	
            var migrator = new DbMigrator(new Configuration());
            migrator.Update(DbMigrator.InitialDatabase);

			base.Uninstall();
		}
		public IList<string> GetWidgetZones()
		{
			return new List<string>
			{
				"content_before"
			};
		}
		public void GetDisplayWidgetRoute(string widgetZone, object model, int storeId, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "PublicInfo";
			controllerName = "CustomBanner";
			routeValues = new RouteValueDictionary
			{

				{
					"Namespaces",
					"CAF.WebSite.CustomBanner.Controllers"
				},

				{
					"area",
					"CAF.WebSite.CustomBanner"
				},

				{
					"widgetZone",
					widgetZone
				},

				{
					"model",
					model
				}
			};
		}
		public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "Configure";
			controllerName = "CustomBanner";
			routeValues = new RouteValueDictionary
			{

				{
					"area",
					"CAF.WebSite.CustomBanner"
				}
			};
		}
	}
}

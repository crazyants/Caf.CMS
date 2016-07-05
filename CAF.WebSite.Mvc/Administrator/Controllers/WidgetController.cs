using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Cms;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Plugins;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.WebSite.Mvc.Admin.Models.Widget;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
 

namespace CAF.WebSite.Mvc.Admin.Controllers
{
 
    public partial class WidgetController : AdminControllerBase
	{
		#region Fields

        private readonly IWidgetService _widgetService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly WidgetSettings _widgetSettings;
	    private readonly IPluginFinder _pluginFinder;
		private readonly PluginMediator _pluginMediator;

	    #endregion

		#region Constructors

        public WidgetController(
			IWidgetService widgetService,
            IPermissionService permissionService, 
			ISettingService settingService,
            WidgetSettings widgetSettings, 
			IPluginFinder pluginFinder,
			PluginMediator pluginMediator)
		{
            this._widgetService = widgetService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._widgetSettings = widgetSettings;
            this._pluginFinder = pluginFinder;
			this._pluginMediator = pluginMediator;
		}

		#endregion 
        
        #region Methods
        
        public ActionResult Index()
        {
            return RedirectToAction("Providers");
        }

		public ActionResult Providers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widgetsModel = new List<WidgetModel>();
            var widgets = _widgetService.LoadAllWidgets();
            foreach (var widget in widgets)
            {
                var model = _pluginMediator.ToProviderModel<IWidget, WidgetModel>(widget);
                model.IsActive = widget.IsWidgetActive(_widgetSettings);
                widgetsModel.Add(model);
            }

			return View(widgetsModel);
        }

		public ActionResult ActivateProvider(string systemName, bool activate)
		{
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
				return AccessDeniedView();
			
			var widget = _widgetService.LoadWidgetBySystemName(systemName);
			if (widget.IsWidgetActive(_widgetSettings))
			{
				if (!activate)
				{
					// mark as disabled
					_widgetSettings.ActiveWidgetSystemNames.Remove(widget.Metadata.SystemName);
					_settingService.SaveSetting(_widgetSettings);
				}
			}
			else
			{
				if (activate)
				{
					// mark as active
					_widgetSettings.ActiveWidgetSystemNames.Add(widget.Metadata.SystemName);
					_settingService.SaveSetting(_widgetSettings);
				}
			}

			return RedirectToAction("Providers");
		}
        
        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone)
        {
            var model = new List<RenderWidgetModel>();
 
            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
            foreach (var widget in widgets)
            {
                var widgetModel = new RenderWidgetModel();
 
                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                widget.Value.GetDisplayWidgetRoute(widgetZone, null, 0, out actionName, out controllerName, out routeValues);
                widgetModel.ActionName = actionName;
                widgetModel.ControllerName = controllerName;
                widgetModel.RouteValues = routeValues;

                model.Add(widgetModel);
            }
            return PartialView(model);
        }

        #endregion
    }
}

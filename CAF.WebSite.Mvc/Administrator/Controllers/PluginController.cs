using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Authentication.External;
using CAF.WebSite.Application.Services.Cms;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Plugins;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.WebSite.Mvc.Admin.Models.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public partial class PluginController : AdminControllerBase
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;
        private readonly ISiteService _siteService;
        //private readonly PaymentSettings _paymentSettings;
        //private readonly ShippingSettings _shippingSettings;
        //private readonly TaxSettings _taxSettings;
        //private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly WidgetSettings _widgetSettings;
        private readonly IProviderManager _providerManager;
        private readonly PluginMediator _pluginMediator;

        #endregion

        #region Constructors

        public PluginController(IPluginFinder pluginFinder,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IPermissionService permissionService,
            ILanguageService languageService,
            ISettingService settingService,
            ISiteService siteService,
            //PaymentSettings paymentSettings,
            //ShippingSettings shippingSettings,
            //TaxSettings taxSettings, 
            //ExternalAuthenticationSettings externalAuthenticationSettings, 
            WidgetSettings widgetSettings,
            IProviderManager providerManager,
            PluginMediator pluginMediator)
        {
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._languageService = languageService;
            this._settingService = settingService;
            this._siteService = siteService;
            //this._paymentSettings = paymentSettings;
            // this._shippingSettings = shippingSettings;
            // this._taxSettings = taxSettings;
            // this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._widgetSettings = widgetSettings;
            this._providerManager = providerManager;
            this._pluginMediator = pluginMediator;
            T = NullLocalizer.Instance;
        }

        #endregion

        #region Utilities

        public Localizer T { get; set; }

        [NonAction]
        protected PluginModel PreparePluginModel(PluginDescriptor pluginDescriptor, bool forList = true)
        {
            var model = pluginDescriptor.ToModel();

            model.Group = _localizationService.GetResource("Admin.Plugins.KnownGroup." + pluginDescriptor.Group);

            if (forList)
            {
                model.FriendlyName = pluginDescriptor.GetLocalizedValue(_localizationService, "FriendlyName");
                model.Description = pluginDescriptor.GetLocalizedValue(_localizationService, "Description");
            }

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.FriendlyName = pluginDescriptor.GetLocalizedValue(_localizationService, "FriendlyName", languageId, false);
                locale.Description = pluginDescriptor.GetLocalizedValue(_localizationService, "Description", languageId, false);
            });

            // Sites
            model.SelectedSiteIds = _settingService.GetSettingByKey<string>(pluginDescriptor.GetSettingKey("LimitedToSites")).ToIntArray();

            // Icon
            model.IconUrl = _pluginMediator.GetIconUrl(pluginDescriptor);

            if (pluginDescriptor.Installed)
            {
                // specify configuration URL only when a plugin is already installed
                if (pluginDescriptor.IsConfigurable)
                {
                    model.ConfigurationUrl = Url.Action("ConfigurePlugin", new { systemName = pluginDescriptor.SystemName });

                    if (!forList)
                    {
                        var configurable = pluginDescriptor.Instance() as IConfigurable;

                        string actionName;
                        string controllerName;
                        RouteValueDictionary routeValues;
                        configurable.GetConfigurationRoute(out actionName, out controllerName, out routeValues);

                        if (actionName.HasValue() && controllerName.HasValue())
                        {
                            model.ConfigurationRoute = new RouteInfo(actionName, controllerName, routeValues);
                        }
                    }
                }
            }
            return model;
        }

        [NonAction]
        protected LocalPluginsModel PrepareLocalPluginsModel()
        {
            var plugins = _pluginFinder.GetPluginDescriptors(false)
                .OrderBy(p => p.Group, PluginFileParser.KnownGroupComparer)
                .ThenBy(p => p.DisplayOrder)
                .Select(x => PreparePluginModel(x));

            var model = new LocalPluginsModel();

            model.AvailableSites = _siteService
                .GetAllSites()
                .Select(s => s.ToModel())
                .ToList();

            var groupedPlugins = from p in plugins
                                 group p by p.Group into g
                                 select g;

            foreach (var group in groupedPlugins)
            {
                foreach (var plugin in group)
                {
                    model.Groups.Add(group.Key, plugin);
                }
            }

            return model;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = PrepareLocalPluginsModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ExecuteTasks(IEnumerable<string> pluginsToInstall, IEnumerable<string> pluginsToUninstall)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                int tasksCount = 0;
                IEnumerable<PluginDescriptor> descriptors = null;

                // Uninstall first
                if (pluginsToUninstall != null && pluginsToUninstall.Any())
                {
                    descriptors = _pluginFinder.GetPluginDescriptors(false).Where(x => pluginsToUninstall.Contains(x.SystemName));
                    foreach (var d in descriptors)
                    {
                        if (d.Installed)
                        {
                            d.Instance().Uninstall();
                            tasksCount++;
                        }
                    }
                }

                // now execute installations
                if (pluginsToInstall != null && pluginsToInstall.Any())
                {
                    descriptors = _pluginFinder.GetPluginDescriptors(false).Where(x => pluginsToInstall.Contains(x.SystemName));
                    foreach (var d in descriptors)
                    {
                        if (!d.Installed)
                        {
                            d.Instance().Install();
                            tasksCount++;
                        }
                    }
                }

                // restart application
                if (tasksCount > 0)
                {
                    _webHelper.RestartAppDomain();
                }
            }
            catch (Exception exc)
            {
                NotifyError(exc);
            }

            return RedirectToAction("List");
        }

        public ActionResult ReloadList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();
            return RedirectToAction("List");
        }

        public ActionResult ConfigurePlugin(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName);
            if (descriptor == null || !descriptor.Installed || !descriptor.IsConfigurable)
                return HttpNotFound();

            var model = PreparePluginModel(descriptor, false);
            model.FriendlyName = descriptor.GetLocalizedValue(_localizationService, "FriendlyName");

            return View(model);
        }

        public ActionResult ConfigureProvider(string systemName, string listUrl)
        {
            var provider = _providerManager.GetProvider(systemName);
            if (provider == null || !provider.Metadata.IsConfigurable)
            {
                return HttpNotFound();
            }

            PermissionRecord requiredPermission = StandardPermissionProvider.AccessAdminPanel;
            var listUrl2 = Url.Action("List");

            var metadata = provider.Metadata;

            //if (metadata.ProviderType == typeof(IPaymentMethod))
            //{
            //    requiredPermission = StandardPermissionProvider.ManagePaymentMethods;
            //    listUrl2 = Url.Action("Providers", "Payment");
            //}
            //if (metadata.ProviderType == typeof(ITaxProvider))
            //{
            //    requiredPermission = StandardPermissionProvider.ManageTaxSettings;
            //    listUrl2 = Url.Action("Providers", "Tax");
            //}
            //else if (metadata.ProviderType == typeof(IShippingRateComputationMethod))
            //{
            //    requiredPermission = StandardPermissionProvider.ManageShippingSettings;
            //    listUrl2 = Url.Action("Providers", "Shipping");
            //}
            // else 
            if (metadata.ProviderType == typeof(IWidget))
            {
                requiredPermission = StandardPermissionProvider.ManageWidgets;
                listUrl2 = Url.Action("Providers", "Widget");
            }
            else if (metadata.ProviderType == typeof(IExternalAuthenticationMethod))
            {
                requiredPermission = StandardPermissionProvider.ManageExternalAuthenticationMethods;
                listUrl2 = Url.Action("Providers", "ExternalAuthentication");
            }

            if (!_permissionService.Authorize(requiredPermission))
            {
                return AccessDeniedView();
            }

            var model = _pluginMediator.ToProviderModel(provider);

            ViewBag.ListUrl = listUrl.NullEmpty() ?? listUrl2;

            return View(model);
        }

        public ActionResult EditProviderPopup(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var provider = _providerManager.GetProvider(systemName);
            if (provider == null)
                return HttpNotFound();

            var model = _pluginMediator.ToProviderModel(provider, true);

            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.FriendlyName = _pluginMediator.GetLocalizedFriendlyName(provider.Metadata, languageId, false);
                locale.Description = _pluginMediator.GetLocalizedDescription(provider.Metadata, languageId, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult EditProviderPopup(string btnId, ProviderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var provider = _providerManager.GetProvider(model.SystemName);
            if (provider == null)
                return HttpNotFound();

            var metadata = provider.Metadata;

            _pluginMediator.SetSetting(metadata, "FriendlyName", model.FriendlyName);
            _pluginMediator.SetSetting(metadata, "Description", model.Description);

            foreach (var localized in model.Locales)
            {
                _pluginMediator.SaveLocalizedValue(metadata, localized.LanguageId, "FriendlyName", localized.FriendlyName);
                _pluginMediator.SaveLocalizedValue(metadata, localized.LanguageId, "Description", localized.Description);
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            return View(model);
        }

        [HttpPost]
        public ActionResult SetSelectedSites(string pk /* SystemName */, string name, FormCollection form)
        {
            // gets called from x-editable 
            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(pk, false);
                if (pluginDescriptor == null)
                {
                    return HttpNotFound("The plugin does not exist");
                }

                string settingKey = pluginDescriptor.GetSettingKey("LimitedToSites");
                var siteIds = (form["value[]"] ?? "0").Split(',').Select(x => x.ToInt()).Where(x => x > 0).ToList();
                if (siteIds.Count > 0)
                {
                    _settingService.SetSetting<string>(settingKey, string.Join(",", siteIds));
                }
                else
                {
                    _settingService.DeleteSetting(settingKey);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(501, ex.Message);
            }

            NotifySuccess(T("Admin.Common.DataSuccessfullySaved"));
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult SortProviders(string providers)
        {
            try
            {
                var arr = providers.Split(',');
                int ordinal = 5;
                foreach (var systemName in arr)
                {
                    var provider = _providerManager.GetProvider(systemName);
                    if (provider != null)
                    {
                        _pluginMediator.SetUserDisplayOrder(provider.Metadata, ordinal);
                    }
                    ordinal += 5;
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex.Message);
                return new HttpStatusCodeResult(501, ex.Message);
            }


            NotifySuccess(T("Admin.Common.DataSuccessfullySaved"));
            return new HttpStatusCodeResult(200);
        }

        public ActionResult UpdateStringResources(string systemName, string returnUrl = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var pluginDescriptor = _pluginFinder.GetPluginDescriptors()
                .Where(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (pluginDescriptor == null)
            {
                NotifyError(_localizationService.GetResource("Admin.Configuration.Plugins.Resources.UpdateFailure"));
            }
            else
            {
                _localizationService.ImportPluginResourcesFromXml(pluginDescriptor, null, false);
                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Plugins.Resources.UpdateSuccess"));
            }

            if (returnUrl.IsEmpty())
            {
                return RedirectToAction("List");
            }

            return Redirect(returnUrl);
        }

        public ActionResult UpdateAllStringResources()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(false);

            foreach (var plugin in pluginDescriptors)
            {
                if (plugin.Installed)
                {
                    _localizationService.ImportPluginResourcesFromXml(plugin, null, false);
                }
                else
                {
                    _localizationService.DeleteLocaleStringResources(plugin.ResourceRootKey);
                }
            }

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Plugins.Resources.UpdateSuccess"));
            return RedirectToAction("List");
        }

        #endregion
    }
}

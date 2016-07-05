using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Async;
using CAF.Infrastructure.Core.Collections;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Utilities;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Security;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Admin.Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class CommonController : AdminControllerBase
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly Lazy<CurrencySettings> _currencySettings;
        private readonly Lazy<IMeasureService> _measureService;
        private readonly Lazy<MeasureSettings> _measureSettings;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly ILocalizationService _localizationService;
        private readonly IUserService _userService;
        private readonly Lazy<SecuritySettings> _securitySettings;
        private readonly Lazy<IMenuPublisher> _menuPublisher;
        private readonly Lazy<IPluginFinder> _pluginFinder;
        private readonly Lazy<IImageCache> _imageCache;
        private readonly Lazy<IDateTimeHelper> _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDbContext _dbContext;
        private readonly Func<string, ICacheManager> _cache;
        private readonly IArticleService _articleService;
        private readonly IPermissionService _permissionService;
        private readonly ICommonServices _services;
        private readonly SiteInformationSettings _siteSettings;
        private readonly static object _lock = new object();

        private readonly static object s_lock = new object();
        #endregion

        #region Constructors

        public CommonController(
            IWebHelper webHelper,
            ILanguageService languageService,
            IWorkContext workContext,
            ISiteContext siteContext,
            IUserService userService,
            ILocalizationService localizationService,
            Lazy<SecuritySettings> securitySettings,
                Lazy<IMenuPublisher> menuPublisher,
            Lazy<CurrencySettings> currencySettings,
            Lazy<MeasureSettings> measureSettings,
            Lazy<IMeasureService> measureService,
            Lazy<IDateTimeHelper> dateTimeHelper,
            Lazy<IPluginFinder> pluginFinder,
            Lazy<IImageCache> imageCache,
            IPermissionService permissionService,
            IGenericAttributeService genericAttributeService,
            IArticleService articleService,
            ICommonServices services,
            IDbContext dbContext,
            SiteInformationSettings siteSettings,
            Func<string, ICacheManager> cache)
        {
            this._webHelper = webHelper;
            this._currencySettings = currencySettings;
            this._measureSettings = measureSettings;
            this._measureService = measureService;
            this._languageService = languageService;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._userService = userService;
            this._localizationService = localizationService;
            this._securitySettings = securitySettings;
            this._menuPublisher = menuPublisher;
            this._pluginFinder = pluginFinder;
            this._genericAttributeService = genericAttributeService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._imageCache = imageCache;
            this._dbContext = dbContext;
            this._cache = cache;
            this._services = services;
            this._articleService = articleService;
            this._siteSettings = siteSettings;
        }

        #endregion

        #region Methods

        #region Navbar & Menu
        [ChildActionOnly]
        public ActionResult Navbar()
        {
            var currentUser = _services.WorkContext.CurrentUser;

            ViewBag.UserName = _services.Settings.LoadSetting<UserSettings>().UserNamesEnabled ? currentUser.UserName : currentUser.Email;
            ViewBag.Sites = _services.SiteService.GetAllSites();
            ViewBag.SiteContentShare = this._siteSettings.SiteContentShare;
            if (_services.Permissions.Authorize(StandardPermissionProvider.ManageMaintenance))
            {
                // ViewBag.CheckUpdateResult = AsyncRunner.RunSync(() => CheckUpdateAsync(false));
            }

            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var cacheManager = _services.Cache;

            var customerRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(x => x.Active).Select(x => x.Id).ToList();
            string cacheKey = string.Format("cafsite.pres.adminmenu.navigation-{0}-{1}", _services.WorkContext.WorkingLanguage.Id, string.Join(",", customerRolesIds));

            var rootNode = cacheManager.Get(cacheKey, () =>
            {
                lock (s_lock)
                {
                    return PrepareAdminMenu();
                }
            });

            return PartialView(rootNode);
        }
      
        private TreeNode<MenuItem> PrepareAdminMenu()
        {
            XmlSiteMap siteMap = new XmlSiteMap(this._cache("static"));
            siteMap.LoadFrom(_webHelper.MapPath("~/Administrator/sitemap.config"));

            var rootNode = ConvertSitemapNodeToMenuItemNode(siteMap.RootNode);

            _menuPublisher.Value.RegisterMenus(rootNode, "admin");

            // hide based on permissions
            rootNode.TraverseTree(x =>
            {
                if (!x.IsRoot)
                {
                    if (!MenuItemAccessPermitted(x.Value))
                    {
                        x.Value.Visible = false;
                    }
                }
            });

            // hide dropdown nodes when no child is visible
            rootNode.TraverseTree(x =>
            {
                if (!x.IsRoot)
                {
                    var item = x.Value;
                    if (!item.IsGroupHeader && !item.HasRoute())
                    {
                        if (!x.Children.Any(child => child.Value.Visible))
                        {
                            item.Visible = false;
                        }
                    }
                }
            });

            return rootNode;
        }

        private TreeNode<MenuItem> ConvertSitemapNodeToMenuItemNode(SiteMapNode node)
        {
            var item = new MenuItem();
            var treeNode = new TreeNode<MenuItem>(item);

            if (node.RouteName.HasValue())
            {
                item.RouteName = node.RouteName;
            }
            else if (node.ActionName.HasValue() && node.ControllerName.HasValue())
            {
                item.ActionName = node.ActionName;
                item.ControllerName = node.ControllerName;
            }
            else if (node.Url.HasValue())
            {
                item.Url = node.Url;
            }
            item.RouteValues = node.RouteValues;

            item.Visible = node.Visible;
            item.Text = node.Title;
            item.Attributes.Merge(node.Attributes);

            if (node.Attributes.ContainsKey("permissionNames"))
                item.PermissionNames = node.Attributes["permissionNames"] as string;

            if (node.Attributes.ContainsKey("id"))
                item.Id = node.Attributes["id"] as string;

            if (node.Attributes.ContainsKey("resKey"))
                item.ResKey = node.Attributes["resKey"] as string;

            if (node.Attributes.ContainsKey("iconClass"))
                item.Icon = node.Attributes["iconClass"] as string;

            if (node.Attributes.ContainsKey("imageUrl"))
                item.ImageUrl = node.Attributes["imageUrl"] as string;

            if (node.Attributes.ContainsKey("isGroupHeader"))
                item.IsGroupHeader = Boolean.Parse(node.Attributes["isGroupHeader"] as string);

            // iterate children recursively
            foreach (var childNode in node.ChildNodes)
            {
                var childTreeNode = ConvertSitemapNodeToMenuItemNode(childNode);
                treeNode.Append(childTreeNode);
            }

            return treeNode;
        }

        private bool MenuItemAccessPermitted(MenuItem item)
        {
            var result = true;

            if (_securitySettings.Value.HideAdminMenuItemsBasedOnPermissions && item.PermissionNames.HasValue())
            {
                var permitted = item.PermissionNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Any(x => _services.Permissions.Authorize(x.Trim()));
                if (!permitted)
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion

        [HttpPost]
        public JsonResult SetSelectedTab(string navId, string tabId, string path)
        {
            if (navId.HasValue() && tabId.HasValue() && path.HasValue())
            {
                var info = new SelectedTabInfo { TabId = tabId, Path = path };
                TempData["SelectedTab." + navId] = info;
            }
            return Json(new { Success = true });
        }

        public ActionResult SetSelectedSite(int? siteId, string returnUrl)
        {
            using (HttpContext.PreviewModeCookie())
            {
                _services.SiteContext.SetPreviewSite(siteId);
            }
            if (returnUrl.IsEmpty() && Request.UrlReferrer != null && Request.UrlReferrer.ToString().Length > 0)
            {
                returnUrl = Request.UrlReferrer.ToString();
            }

            return Redirect(returnUrl);
        }

        public ActionResult SystemInfo()
        {
            var model = new SystemInfoModel();
            model.AppVersion = WorkVersion.CurrentFullVersion;

            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
            }
            catch (Exception) { }
            try
            {
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
            }
            catch (Exception) { }
            try
            {
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch (Exception) { }

            model.ServerTimeZone = TimeZone.CurrentTimeZone.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            model.HttpHost = _webHelper.ServerVariables("HTTP_HOST");
            //Environment.GetEnvironmentVariable("USERNAME");

            try
            {
                var mbSize = _dbContext.SqlQuery<decimal>("Select Sum(size)/128.0 From sysfiles").FirstOrDefault();
                model.DatabaseSize = Convert.ToDouble(mbSize);
            }
            catch (Exception) { }

            try
            {
                if (DataSettings.Current.IsValid())
                    model.DataProviderFriendlyName = DataSettings.Current.ProviderFriendlyName;
            }
            catch (Exception) { }

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fi = new FileInfo(assembly.Location);
                model.AppDate = fi.LastWriteTime.ToLocalTime();
            }
            catch (Exception) { }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                model.LoadedAssemblies.Add(new SystemInfoModel.LoadedAssembly()
                {
                    FullName = assembly.FullName,
                    //we cannot use Location property in medium trust
                    //Location = assembly.Location
                });
            }
            return View(model);
        }

        public ActionResult Warnings()
        {
            var model = new List<SystemWarningModel>();

            //store URL
            var currentSiteUrl = _siteContext.CurrentSite.Url.EnsureEndsWith("/");
            if (currentSiteUrl.HasValue() && (currentSiteUrl.IsCaseInsensitiveEqual(_webHelper.GetSiteLocation(false)) || currentSiteUrl.IsCaseInsensitiveEqual(_webHelper.GetSiteLocation(true))))
                model.Add(new SystemWarningModel()
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.URL.Match")
                });
            else
                model.Add(new SystemWarningModel()
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.URL.NoMatch"), currentSiteUrl, _webHelper.GetSiteLocation(false))
                });


            //primary exchange rate currency
            //var perCurrency = _currencyService.Value.GetCurrencyById(_currencySettings.Value.PrimaryExchangeRateCurrencyId);
            //if (perCurrency != null)
            //{
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Pass,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.Set"),
            //    });
            //    if (perCurrency.Rate != 1)
            //    {
            //        model.Add(new SystemWarningModel()
            //        {
            //            Level = SystemWarningLevel.Fail,
            //            Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.Rate1")
            //        });
            //    }
            //}
            //else
            //{
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Fail,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.NotSet")
            //    });
            //}

            //primary store currency
            //var pscCurrency = _currencyService.Value.GetCurrencyById(_currencySettings.Value.PrimarySiteCurrencyId);
            //if (pscCurrency != null)
            //{
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Pass,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.PrimaryCurrency.Set"),
            //    });
            //}
            //else
            //{
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Fail,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.PrimaryCurrency.NotSet")
            //    });
            //}


            //base measure weight
            //var bWeight = _measureService.Value.GetMeasureWeightById(_measureSettings.Value.BaseWeightId);
            //if (bWeight != null)
            //{
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Pass,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.Set"),
            //    });

            //    if (bWeight.Ratio != 1)
            //    {
            //        model.Add(new SystemWarningModel()
            //        {
            //            Level = SystemWarningLevel.Fail,
            //            Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.Ratio1")
            //        });
            //    }
            //}
            //else
            //{
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Fail,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.DefaultWeight.NotSet")
            //    });
            //}


            //base dimension weight
            var bDimension = _measureService.Value.GetMeasureDimensionById(_measureSettings.Value.BaseDimensionId);
            if (bDimension != null)
            {
                model.Add(new SystemWarningModel()
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.Set"),
                });

                if (bDimension.Ratio != 1)
                {
                    model.Add(new SystemWarningModel()
                    {
                        Level = SystemWarningLevel.Fail,
                        Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.Ratio1")
                    });
                }
            }
            else
            {
                model.Add(new SystemWarningModel()
                {
                    Level = SystemWarningLevel.Fail,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DefaultDimension.NotSet")
                });
            }

            // shipping rate coputation methods
            //if (_shippingService.Value.LoadActiveShippingRateComputationMethods()
            //    .Where(x => x.Value.ShippingRateComputationMethodType == ShippingRateComputationMethodType.Offline)
            //    .Count() > 1)
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Warning,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.Shipping.OnlyOneOffline")
            //    });

            ////payment methods
            //if (_paymentService.Value.LoadActivePaymentMethods()
            //    .Count() > 0)
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Pass,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.PaymentMethods.OK")
            //    });
            //else
            //    model.Add(new SystemWarningModel()
            //    {
            //        Level = SystemWarningLevel.Fail,
            //        Text = _localizationService.GetResource("Admin.System.Warnings.PaymentMethods.NoActive")
            //    });

            //incompatible plugins
            if (PluginManager.IncompatiblePlugins != null)
                foreach (var pluginName in PluginManager.IncompatiblePlugins)
                    model.Add(new SystemWarningModel()
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.IncompatiblePlugin"), pluginName)
                    });

            //validate write permissions (the same procedure like during installation)
            var dirPermissionsOk = true;
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite(_webHelper);
            foreach (string dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                {
                    model.Add(new SystemWarningModel()
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.Wrong"), WindowsIdentity.GetCurrent().Name, dir)
                    });
                    dirPermissionsOk = false;
                }
            if (dirPermissionsOk)
                model.Add(new SystemWarningModel()
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.OK")
                });

            var filePermissionsOk = true;
            var filesToCheck = FilePermissionHelper.GetFilesWrite(_webHelper);
            foreach (string file in filesToCheck)
                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                {
                    model.Add(new SystemWarningModel()
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(_localizationService.GetResource("Admin.System.Warnings.FilePermission.Wrong"), WindowsIdentity.GetCurrent().Name, file)
                    });
                    filePermissionsOk = false;
                }
            if (filePermissionsOk)
                model.Add(new SystemWarningModel()
                {
                    Level = SystemWarningLevel.Pass,
                    Text = _localizationService.GetResource("Admin.System.Warnings.FilePermission.OK")
                });


            return View(model);
        }
        public ActionResult Maintenance()
        {
            var model = new MaintenanceModel();
            model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
            model.DeleteGuests.OnlyWithoutShoppingCart = true;

            // image cache stats
            long imageCacheFileCount = 0;
            long imageCacheTotalSize = 0;
            _imageCache.Value.CacheStatistics(out imageCacheFileCount, out imageCacheTotalSize);
            model.DeleteImageCache.FileCount = imageCacheFileCount;
            model.DeleteImageCache.TotalSize = Prettifier.BytesToString(imageCacheTotalSize);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-image-cache")]
        public ActionResult MaintenanceDeleteImageCache()
        {

            _imageCache.Value.DeleteCachedImages();

            return RedirectToAction("Maintenance");
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-guests")]
        public ActionResult MaintenanceDeleteGuests(MaintenanceModel model)
        {


            DateTime? startDateValue = (model.DeleteGuests.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.Value.ConvertToUtcTime(model.DeleteGuests.StartDate.Value, _dateTimeHelper.Value.CurrentTimeZone);

            DateTime? endDateValue = (model.DeleteGuests.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.Value.ConvertToUtcTime(model.DeleteGuests.EndDate.Value, _dateTimeHelper.Value.CurrentTimeZone).AddDays(1);

            model.DeleteGuests.NumberOfDeletedUsers = _userService.DeleteGuestUsers(startDateValue, endDateValue, model.DeleteGuests.OnlyWithoutShoppingCart);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-exported-files")]
        public ActionResult MaintenanceDeleteFiles(MaintenanceModel model)
        {


            DateTime? startDateValue = (model.DeleteExportedFiles.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.Value.ConvertToUtcTime(model.DeleteExportedFiles.StartDate.Value, _dateTimeHelper.Value.CurrentTimeZone);

            DateTime? endDateValue = (model.DeleteExportedFiles.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.Value.ConvertToUtcTime(model.DeleteExportedFiles.EndDate.Value, _dateTimeHelper.Value.CurrentTimeZone).AddDays(1);


            model.DeleteExportedFiles.NumberOfDeletedFiles = 0;
            string path = string.Format("{0}Content\\files\\exportimport\\", this.Request.PhysicalApplicationPath);
            foreach (var fullPath in System.IO.Directory.GetFiles(path))
            {
                try
                {
                    var fileName = Path.GetFileName(fullPath);
                    if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    if (fileName.Equals("placeholder", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var info = new FileInfo(fullPath);
                    if ((!startDateValue.HasValue || startDateValue.Value < info.CreationTimeUtc) &&
                        (!endDateValue.HasValue || info.CreationTimeUtc < endDateValue.Value))
                    {
                        System.IO.File.Delete(fullPath);
                        model.DeleteExportedFiles.NumberOfDeletedFiles++;
                    }
                }
                catch (Exception exc)
                {
                    NotifyError(exc, false);
                }
            }

            return View(model);
        }

        [HttpPost, ActionName("Maintenance"), ValidateInput(false)]
        [FormValueRequired("execute-sql-query")]
        public ActionResult MaintenanceExecuteSql(MaintenanceModel model)
        {


            if (model.SqlQuery.HasValue())
            {
                var dbContext = EngineContext.Current.Resolve<IDbContext>();
                try
                {
                    dbContext.ExecuteSqlThroughSmo(model.SqlQuery);

                    NotifySuccess("The sql command was executed successfully.");
                }
                catch (Exception ex)
                {
                    NotifyError("Error executing sql command: {0}".FormatCurrentUI(ex.Message));
                }
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = new LanguageSelectorModel();
            model.CurrentLanguage = _services.WorkContext.WorkingLanguage.ToModel();
            model.AvailableLanguages = _languageService
                 .GetAllLanguages(siteId: _services.SiteContext.CurrentSite.Id)
                 .Select(x => x.ToModel())
                 .ToList();
            return PartialView(model);
        }
        public ActionResult LanguageSelected(int userlanguage)
        {
            var language = _languageService.GetLanguageById(userlanguage);
            if (language != null)
            {
                _services.WorkContext.WorkingLanguage = language;
            }
            return Content(_localizationService.GetResource("Admin.Common.DataEditSuccess"));
        }


        public ActionResult ClearCache(string previousUrl)
        {
            var cacheManager = _cache("static");
            cacheManager.Clear();

            cacheManager = _cache("aspnet");
            cacheManager.Clear();

            this.NotifySuccess(_localizationService.GetResource("Admin.Common.TaskSuccessfullyProcessed"));

            if (previousUrl.HasValue())
                return Redirect(previousUrl);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult RestartApplication(string previousUrl)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            _services.WebHelper.RestartAppDomain();

            if (previousUrl.HasValue())
                return Redirect(previousUrl);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult UpdateDisplayOrder(int id, string name, string value)
        {
            switch (name)
            {
                //图片排序更新
                case "Picture":
                    var articlePicture = _articleService.GetArticleAlbumById(id);
                    if (articlePicture == null)
                        throw new ArgumentException("No article picture found with the specified id");
                    articlePicture.DisplayOrder = value.ToInt();
                    _articleService.UpdateArticleAlbum(articlePicture);
                    break;
            }

            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Common.UpdateDisplayOrder") }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Install()
        {
            var model = new InstallModel();

            return View(model);
        }
        [HttpPost, ActionName("Install")]
        [FormValueRequired("install-permissions")]
        public ActionResult InstallPermissions()
        {
            var scope = EngineContext.Current.ContainerManager.Scope();
            // Register default permissions
            var permissionProviders = new List<Type>();
            permissionProviders.Add(typeof(StandardPermissionProvider));
            foreach (var providerType in permissionProviders)
            {
                dynamic provider = Activator.CreateInstance(providerType);
                EngineContext.Current.ContainerManager.Resolve<IPermissionService>(scope: scope).InstallPermissions(provider);
            }
            this.NotifySuccess(_localizationService.GetResource("Admin.Common.Install"));

            return RedirectToAction("Install");
        }
        [HttpPost, ActionName("Install")]
        [FormValueRequired("uninstall-permissions")]
        public ActionResult UnInstallPermissions()
        {
            var scope = EngineContext.Current.ContainerManager.Scope();
            // Register default permissions
            var permissionProviders = new List<Type>();
            permissionProviders.Add(typeof(StandardPermissionProvider));
            foreach (var providerType in permissionProviders)
            {
                dynamic provider = Activator.CreateInstance(providerType);
                EngineContext.Current.ContainerManager.Resolve<IPermissionService>(scope: scope).UninstallPermissions(provider);

            }

            this.NotifySuccess(_localizationService.GetResource("Admin.Common.UnInstall"));
            return RedirectToAction("Install");
        }
        #endregion
    }
}
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Themes;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Themes;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Themes;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.WebSite.Mvc.Admin.Models.Topics;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Collections;
using CAF.WebSite.Application.WebUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Models.Themes;
using CAF.WebSite.Application.WebUI.Theming;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public partial class ThemeController : AdminControllerBase
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IThemeRegistry _themeRegistry;
        private readonly IThemeVariablesService _themeVarService;
        private readonly ISiteService _siteService;
        private readonly ICommonServices _services;
        private readonly IThemeContext _themeContext;
        private readonly Lazy<IThemeFileResolver> _themeFileResolver;

        #endregion

        #region Constructors

        public ThemeController(
            ISettingService settingService,
            IThemeRegistry themeRegistry,
            IThemeVariablesService themeVarService,
            ISiteService siteService,
            ICommonServices services,
            IThemeContext themeContext,
            Lazy<IThemeFileResolver> themeFileResolver)
        {
            this._settingService = settingService;
            this._themeVarService = themeVarService;
            this._themeRegistry = themeRegistry;
            this._siteService = siteService;

            this._services = services;
            this._themeContext = themeContext;
            this._themeFileResolver = themeFileResolver;

            this.T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int? siteId)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageThemes))
                return AccessDeniedView();

            int selectedSiteId = siteId ?? _services.SiteContext.CurrentSite.Id;
            var themeSettings = _settingService.LoadSetting<ThemeSettings>(selectedSiteId);
            var model = themeSettings.ToModel();

            var commonListItems = new List<SelectListItem> 
            {
                new SelectListItem { Value = "0", Text = T("Common.Auto") },
                new SelectListItem { Value = "1", Text = T("Common.No") },
                new SelectListItem { Value = "2", Text = T("Common.Yes") }
            };

            model.AvailableBundleOptimizationValues.AddRange(commonListItems);
            model.AvailableBundleOptimizationValues.FirstOrDefault(x => int.Parse(x.Value) == model.BundleOptimizationEnabled).Selected = true;

            // add theme configs
            model.DesktopThemes.AddRange(GetThemes(false, themeSettings));
            model.MobileThemes.AddRange(GetThemes(true, themeSettings));

            model.SiteId = selectedSiteId;
            model.AvailableSites = _siteService.GetAllSites().ToSelectListItems();

            return View(model);
        }

        private IList<ThemeManifestModel> GetThemes(bool mobile, ThemeSettings themeSettings, bool includeHidden = true)
        {
            var themes = from m in _themeRegistry.GetThemeManifests(includeHidden)
                         where m.MobileTheme == mobile
                         select PrepareThemeManifestModel(m, themeSettings);

            var sortedThemes = themes.ToArray().SortTopological(StringComparer.OrdinalIgnoreCase).Cast<ThemeManifestModel>();

            return sortedThemes.OrderByDescending(x => x.IsActive).ToList();
        }

        protected virtual ThemeManifestModel PrepareThemeManifestModel(ThemeManifest manifest, ThemeSettings themeSettings)
        {
            var model = new ThemeManifestModel
                {
                    Name = manifest.ThemeName,
                    BaseTheme = manifest.BaseThemeName,
                    Title = manifest.ThemeTitle,
                    Description = manifest.PreviewText,
                    Author = manifest.Author,
                    Url = manifest.Url,
                    Version = manifest.Version,
                    IsMobileTheme = manifest.MobileTheme,
                    SupportsRtl = manifest.SupportRtl,
                    PreviewImageUrl = manifest.PreviewImageUrl.HasValue() ? manifest.PreviewImageUrl : "{0}/{1}/preview.png".FormatInvariant(manifest.Location, manifest.ThemeName),
                    IsActive = manifest.MobileTheme ? themeSettings.DefaultMobileTheme == manifest.ThemeName : themeSettings.DefaultDesktopTheme == manifest.ThemeName,
                    State = manifest.State
                };

            if (HostingEnvironment.VirtualPathProvider.FileExists("{0}/{1}/Views/Shared/ConfigureTheme.cshtml".FormatInvariant(manifest.Location, manifest.ThemeName)))
            {
                model.IsConfigurable = true;
            }

            return model;
        }

        [HttpPost, ActionName("List")]
        public ActionResult ListPost(ThemeListModel model)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageThemes))
                return AccessDeniedView();

            var themeSettings = _settingService.LoadSetting<ThemeSettings>(model.SiteId);

            bool showRestartNote = model.MobileDevicesSupported != themeSettings.MobileDevicesSupported;

            bool mobileThemeSwitched = false;
            bool themeSwitched = themeSettings.DefaultDesktopTheme.IsCaseInsensitiveEqual(model.DefaultDesktopTheme);
            if (!themeSwitched)
            {
                themeSwitched = themeSettings.DefaultMobileTheme.IsCaseInsensitiveEqual(model.DefaultMobileTheme);
                mobileThemeSwitched = themeSwitched;
            }

            if (themeSwitched)
            {
                _services.EventPublisher.Publish<ThemeSwitchedEvent>(new ThemeSwitchedEvent
                {
                    IsMobile = mobileThemeSwitched,
                    OldTheme = mobileThemeSwitched ? themeSettings.DefaultMobileTheme : themeSettings.DefaultDesktopTheme,
                    NewTheme = mobileThemeSwitched ? model.DefaultMobileTheme : model.DefaultDesktopTheme
                });
            }

            themeSettings = model.ToEntity(themeSettings);
            _settingService.SaveSetting(themeSettings, model.SiteId);

            // activity log
            _services.UserActivity.InsertActivity("EditSettings", T("ActivityLog.EditSettings"));

            NotifySuccess(T("Admin.Configuration.Updated"));

            if (showRestartNote)
            {
                NotifyInfo(T("Admin.Common.RestartAppRequest"));
            }

            return RedirectToAction("List", new { siteId = model.SiteId });
        }

        public ActionResult Configure(string theme, int siteId)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageThemes))
                return AccessDeniedView();

            if (!_themeRegistry.ThemeManifestExists(theme))
            {
                return RedirectToAction("List", new { siteId = siteId });
            }

            var model = new ConfigureThemeModel
            {
                ThemeName = theme,
                SiteId = siteId,
                AvailableSites = _siteService.GetAllSites().ToSelectListItems()
            };

            ViewData["ConfigureThemeUrl"] = Url.Action("Configure", new { theme = theme });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Configure(string theme, int siteId, IDictionary<string, object> values, bool continueEditing)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageThemes))
                return AccessDeniedView();

            if (!_themeRegistry.ThemeManifestExists(theme))
            {
                return RedirectToAction("List", new { siteId = siteId });
            }

            // get current for later resite on parse error
            var currentVars = _themeVarService.GetThemeVariables(theme, siteId);

            // save now
            values = FixThemeVarValues(values);
            _themeVarService.SaveThemeVariables(theme, siteId, values);

            // check for parsing error
            var manifest = _themeRegistry.GetThemeManifest(theme);
            string error = ValidateLess(manifest, siteId);
            if (error.HasValue())
            {
                // resite previous vars
                try
                {
                    _themeVarService.DeleteThemeVariables(theme, siteId);
                }
                finally
                {
                    // we do it here to absolutely ensure that this gets called
                    _themeVarService.SaveThemeVariables(theme, siteId, currentVars);
                }

                TempData["LessParsingError"] = error.Trim().TrimStart('\r', '\n', '/', '*').TrimEnd('*', '/', '\r', '\n');
                TempData["OverriddenThemeVars"] = values;
                NotifyError(T("Admin.Configuration.Themes.Notifications.ConfigureError"));
                return RedirectToAction("Configure", new { theme = theme, siteId = siteId });
            }

            // activity log
            _services.UserActivity.InsertActivity("EditThemeVars", T("ActivityLog.EditThemeVars"), theme);

            NotifySuccess(T("Admin.Configuration.Themes.Notifications.ConfigureSuccess"));

            return continueEditing ?
                RedirectToAction("Configure", new { theme = theme, siteId = siteId }) :
                RedirectToAction("List", new { siteId = siteId });
        }

        private IDictionary<string, object> FixThemeVarValues(IDictionary<string, object> values)
        {
            var fixedDict = new Dictionary<string, object>();

            foreach (var kvp in values)
            {
                var value = kvp.Value;

                var strValue = string.Empty;

                var arrValue = value as string[];
                if (arrValue != null)
                {
                    strValue = strValue = arrValue.Length > 0 ? arrValue[0] : value.ToString();
                }
                else
                {
                    strValue = value.ToString();
                }

                fixedDict[kvp.Key] = strValue;
            }

            return fixedDict;
        }

        /// <summary>
        /// Validates the result LESS file by calling it's url.
        /// </summary>
        /// <param name="theme">Theme name</param>
        /// <param name="siteId">Sited Id</param>
        /// <returns>The error message when a parsing error occured, <c>null</c> otherwise</returns>
        private string ValidateLess(ThemeManifest manifest, int siteId)
        {

            string error = string.Empty;

            var virtualPath = "~/Themes/{0}/Content/theme.less".FormatCurrent(manifest.ThemeName);
            var resolver = this._themeFileResolver.Value;
            var file = resolver.Resolve(virtualPath);
            if (file != null)
            {
                virtualPath = file.ResultVirtualPath;
            }

            var url = "{0}{1}?siteId={2}&theme={3}".FormatInvariant(
                _services.WebHelper.GetSiteLocation().EnsureEndsWith("/"),
                VirtualPathUtility.ToAbsolute(virtualPath).TrimStart('/'),
                siteId,
                manifest.ThemeName);

            HttpWebRequest request = WebRequest.CreateHttp(url);
            WebResponse response = null;

            try
            {
                response = request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    var webResponse = (HttpWebResponse)ex.Response;

                    var statusCode = webResponse.StatusCode;

                    if (statusCode == HttpStatusCode.InternalServerError)
                    {
                        // catch only 500, as this indicates a parsing error.
                        var stream = webResponse.GetResponseStream();

                        using (var streamReader = new StreamReader(stream))
                        {
                            // read the content (the error message has been put there)
                            error = streamReader.ReadToEnd();
                            streamReader.Close();
                            stream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            return error;
        }

        public ActionResult ReloadThemes(int? siteId)
        {
            if (_services.Permissions.Authorize(StandardPermissionProvider.ManageThemes))
            {
                _themeRegistry.ReloadThemes();
            }

            return RedirectToAction("List", new { siteId = siteId });
        }

        public ActionResult Reset(string theme, int siteId)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageThemes))
                return AccessDeniedView();

            if (!_themeRegistry.ThemeManifestExists(theme))
            {
                return RedirectToAction("List", new { siteId = siteId });
            }

            _themeVarService.DeleteThemeVariables(theme, siteId);

            // activity log
            _services.UserActivity.InsertActivity("ResetThemeVars", T("ActivityLog.ResetThemeVars"), theme);

            NotifySuccess(T("Admin.Configuration.Themes.Notifications.ResetSuccess"));
            return RedirectToAction("Configure", new { theme = theme, siteId = siteId });
        }

        [HttpPost]
        public ActionResult ImportVariables(string theme, int siteId, FormCollection form)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageThemes))
                return AccessDeniedView();

            if (!_themeRegistry.ThemeManifestExists(theme))
            {
                return RedirectToAction("List", new { siteId = siteId });
            }

            try
            {
                var file = Request.Files["importxmlfile"];
                if (file != null && file.ContentLength > 0)
                {
                    int importedCount = 0;
                    using (var sr = new StreamReader(file.InputStream, Encoding.UTF8))
                    {
                        string content = sr.ReadToEnd();
                        importedCount = _themeVarService.ImportVariables(theme, siteId, content);
                    }

                    // activity log
                    try
                    {
                        _services.UserActivity.InsertActivity("ImportThemeVars", T("ActivityLog.ResetThemeVars"), importedCount, theme);
                    }
                    catch { }

                    NotifySuccess(T("Admin.Configuration.Themes.Notifications.ImportSuccess", importedCount));
                }
                else
                {
                    NotifyError(T("Admin.Configuration.Themes.Notifications.UploadFile"));
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }

            return RedirectToAction("Configure", new { theme = theme, siteId = siteId });
        }

        [HttpPost]
        public ActionResult ExportVariables(string theme, int siteId, FormCollection form)
        {
            if (!_services.Permissions.Authorize(StandardPermissionProvider.ManageThemes))
                return AccessDeniedView();

            if (!_themeRegistry.ThemeManifestExists(theme))
            {
                return RedirectToAction("List", new { siteId = siteId });
            }

            try
            {
                var xml = _themeVarService.ExportVariables(theme, siteId);

                if (xml.IsEmpty())
                {
                    NotifyInfo(T("Admin.Configuration.Themes.Notifications.NoExportInfo"));
                }
                else
                {
                    string profileName = form["exportprofilename"];
                    string fileName = "themevars-{0}{1}-{2}.xml".FormatCurrent(theme, profileName.HasValue() ? "-" + profileName.ToValidFileName() : "", DateTime.Now.ToString("yyyyMMdd"));

                    // activity log
                    try
                    {
                        _services.UserActivity.InsertActivity("ExportThemeVars", T("ActivityLog.ExportThemeVars"), theme);
                    }
                    catch { }

                    return new XmlDownloadResult(xml, fileName);
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }

            return RedirectToAction("Configure", new { theme = theme, siteId = siteId });
        }

        #endregion

        #region Preview

        public ActionResult Preview(string theme, int? siteId, string returnUrl)
        {
            // Initializes the preview mode

            if (!siteId.HasValue)
            {
                siteId = _services.SiteContext.CurrentSite.Id;
            }

            if (theme.IsEmpty())
            {
                theme = _settingService.LoadSetting<ThemeSettings>(siteId.Value).DefaultDesktopTheme;
            }

            if (!_themeRegistry.ThemeManifestExists(theme) || _themeRegistry.GetThemeManifest(theme).MobileTheme)
                return HttpNotFound();

            using (HttpContext.PreviewModeCookie())
            {
                _themeContext.SetPreviewTheme(theme);
                _services.SiteContext.SetPreviewSite(siteId);
            }

            if (returnUrl.IsEmpty() && Request.UrlReferrer != null && Request.UrlReferrer.ToString().Length > 0)
            {
                returnUrl = Request.UrlReferrer.ToString();
            }

            TempData["PreviewModeReturnUrl"] = returnUrl;

            return RedirectToAction("Index", "Home", new { area = (string)null });
        }

        public ActionResult PreviewTool()
        {
            // Prepares data for the preview mode (flyout) tool

            var currentTheme = _themeContext.CurrentTheme;
            ViewBag.Themes = (from m in _themeRegistry.GetThemeManifests(false)
                              where !m.MobileTheme
                              select new SelectListItem
                              {
                                  Value = m.ThemeName,
                                  Text = m.ThemeTitle,
                                  Selected = m == currentTheme
                              }).ToList();

            var currentSite = _services.SiteContext.CurrentSite;
            ViewBag.Sites = (_siteService.GetAllSites().Select(x => new SelectListItem
                         {
                             Value = x.Id.ToString(),
                             Text = x.Name,
                             Selected = x.Id == currentSite.Id
                         })).ToList();

            var themeSettings = _settingService.LoadSetting<ThemeSettings>(currentSite.Id);
            ViewBag.DisableApply = themeSettings.DefaultDesktopTheme.IsCaseInsensitiveEqual(currentTheme.ThemeName);
            var cookie = Request.Cookies["sm:PreviewToolOpen"];
            ViewBag.ToolOpen = cookie != null ? cookie.Value.ToBool() : false;

            return PartialView();
        }

        [HttpPost, ActionName("PreviewTool")]
        [FormValueRequired(FormValueRequirementRule.MatchAll, "theme", "siteId")]
        [FormValueAbsent(FormValueRequirement.StartsWith, "PreviewMode.")]
        public ActionResult PreviewToolPost(string theme, int siteId, string returnUrl)
        {
            // Refreshes the preview mode (after a select change)

            using (HttpContext.PreviewModeCookie())
            {
                _themeContext.SetPreviewTheme(theme);
                _services.SiteContext.SetPreviewSite(siteId);
            }

            return Redirect(returnUrl);
        }

        [HttpPost, ActionName("PreviewTool"), FormValueRequired("PreviewMode.Exit")]
        public ActionResult ExitPreview()
        {
            // Exits the preview mode

            using (HttpContext.PreviewModeCookie())
            {
                _themeContext.SetPreviewTheme(null);
                _services.SiteContext.SetPreviewSite(null);
            }

            var returnUrl = (string)TempData["PreviewModeReturnUrl"];
            if (returnUrl.IsEmpty())
            {
                returnUrl = Url.Action("Index", "Home", new { area = (string)null });
            }

            return Redirect(returnUrl);
        }

        [HttpPost, ActionName("PreviewTool"), FormValueRequired("PreviewMode.Apply")]
        public ActionResult ApplyPreviewTheme(string theme, int siteId)
        {
            // Applies the current previewed theme and exits the preview mode

            var themeSettings = _settingService.LoadSetting<ThemeSettings>(siteId);
            var oldTheme = themeSettings.DefaultDesktopTheme;
            themeSettings.DefaultDesktopTheme = theme;
            var themeSwitched = oldTheme.IsCaseInsensitiveEqual(theme);

            if (themeSwitched)
            {
                _services.EventPublisher.Publish<ThemeSwitchedEvent>(new ThemeSwitchedEvent
                {
                    IsMobile = false,
                    OldTheme = oldTheme,
                    NewTheme = theme
                });
            }

            _settingService.SaveSetting(themeSettings, siteId);

            _services.UserActivity.InsertActivity("EditSettings", T("ActivityLog.EditSettings"));
            NotifySuccess(T("Admin.Configuration.Updated"));

            return ExitPreview();
        }

        #endregion

        #region File
        public ActionResult Files(string theme, int? siteId, string returnUrl)
        {
           
            if (!siteId.HasValue)
            {
                siteId = _services.SiteContext.CurrentSite.Id;
            }

            if (theme.IsEmpty())
            {
                theme = _settingService.LoadSetting<ThemeSettings>(siteId.Value).DefaultDesktopTheme;
            }

            if (!_themeRegistry.ThemeManifestExists(theme) || _themeRegistry.GetThemeManifest(theme).MobileTheme)
                return HttpNotFound();

            var model = new PageTemplateModel { Name = theme, SiteId=siteId.Value };

            var cssDirectory = Path.Combine(Server.MapPath("~/Themes"), theme, "Content");
            foreach (var file in Directory.GetFiles(cssDirectory, "*.less", SearchOption.AllDirectories))
            {
                var fileName = file.Replace(cssDirectory, "").Trim('\\');
                model.CssFiles.Add(new CssFileViewModel
                {
                    Name = fileName.Replace('\\', '|'),
                    LastUpdated = System.IO.File.GetLastWriteTime(file)
                });
            }

            var viewDirectory = Path.Combine(Server.MapPath("~/Themes"), theme, "Views");
            foreach (var file in Directory.GetFiles(viewDirectory, "*.cshtml", SearchOption.AllDirectories))
            {
                var fileName = file.Replace(viewDirectory, "").Trim('\\');
                if (fileName.IndexOf('\\') < 0) continue;
                var ctrl = fileName.Split('\\')[0];
                var act = fileName.Split('\\').Last();
                model.ViewFiles.Add(new ViewFileViewModel
                {
                    Name = fileName.Replace('\\', '|'),
                    Controller = ctrl,
                    Action = act.Remove(act.LastIndexOf('.')),
                    LastUpdated = System.IO.File.GetLastWriteTime(file)
                });
            }
            return View(model);
        }

       
        public ContentResult LoadFile(string theme, string name)
        {
            if (theme.Contains(".") || theme.Contains("/") || theme.Contains("\\")
                || name.Contains("..") || name.Contains("/") || name.Contains("\\"))
                return null;

            name = name.Replace('|', '\\');

            string filePath;
            if (name.ToLowerInvariant().EndsWith(".cshtml"))
                filePath = Path.Combine(Server.MapPath("~/Themes"), theme, "Views", name);
            else if (name.ToLowerInvariant().EndsWith(".css") || name.ToLowerInvariant().EndsWith(".less"))
                filePath = Path.Combine(Server.MapPath("~/Themes"), theme, "Content", name);
            else return
                    null;

            var result = new ContentResult();
            result.Content = System.IO.File.ReadAllText(filePath);

            return result;
        }

        [ValidateInput(false)]
        public JsonResult SaveFile(string theme, string name, string contents)
        {
            if (theme.Contains(".") || theme.Contains("/") || theme.Contains("\\")
                || name.Contains("..") || name.Contains("/") || name.Contains("\\"))
                return null;

            name = name.Replace('|', '\\');

            string filePath;
            if (name.ToLowerInvariant().EndsWith(".cshtml"))
                filePath = Path.Combine(Server.MapPath("~/Themes"), theme, "Views", name);
            else if (name.ToLowerInvariant().EndsWith(".css") || name.ToLowerInvariant().EndsWith(".less"))
                filePath = Path.Combine(Server.MapPath("~/Themes"), theme, "Content", name);
            else return
                    null;

            System.IO.File.WriteAllText(filePath, contents, Encoding.UTF8);
            object data = new
            {
                status = 1,
                msg = ""
            };
            return new JsonResult { Data = data };
        }
        public FilePathResult DownloadFile(string theme, string name)
        {
            if (theme.Contains(".") || theme.Contains("/") || theme.Contains("\\")
                || name.Contains("..") || name.Contains("/") || name.Contains("\\"))
                return null;

            name = name.Replace('|', '\\');
            string filePath="";
            if (name.ToLowerInvariant().EndsWith(".cshtml"))
                filePath = Path.Combine(Server.MapPath("~/Themes"), theme, "Views", name);
            else if (name.ToLowerInvariant().EndsWith(".css") || name.ToLowerInvariant().EndsWith(".less"))
                filePath = Path.Combine(Server.MapPath("~/Themes"), theme, "Content", name);
            var result = new FilePathResult(filePath, "application/octet-stream");
            result.FileDownloadName = Path.GetFileName(filePath);
            return result;
        }

        [HttpPost]
        public ActionResult UploadFile(string theme, string name, HttpPostedFileBase file)
        {
            if (theme.Contains(".") || theme.Contains("/") || theme.Contains("\\")
                || name.Contains("..") || name.Contains("/") || name.Contains("\\"))
                return null;

            name = name.Replace('|', '\\');

            string filePath = "";
            if (name.ToLowerInvariant().EndsWith(".cshtml"))
                filePath = Path.Combine(Server.MapPath("~/Themes"), theme, "Views", name);
            else if (name.ToLowerInvariant().EndsWith(".css") || name.ToLowerInvariant().EndsWith(".less"))
                filePath = Path.Combine(Server.MapPath("~/Themes"), theme, "Content", name);

            if (file == null || file.ContentLength <= 0)
                return null;

            file.SaveAs(filePath);

            return RedirectToAction("Files", new { name = theme, msg = "The file has been uploaded successfully" });
        }

        public ActionResult Delete(string theme)
        {
            if (string.IsNullOrWhiteSpace(theme) || theme.Contains(".") || theme.Contains("/") || theme.Contains("\\")
                || theme == "Shared")
                return null;

            var filePath = Path.Combine(Server.MapPath("~/Themes"), theme,"Views");
            if (Directory.Exists(filePath))
                Directory.Delete(filePath, true);

            filePath = Path.Combine(Server.MapPath("~/Themes"), "Content", theme);
            if (Directory.Exists(filePath))
                Directory.Delete(filePath, true);

            return RedirectToAction("Files");
        }

        #endregion
    }
}

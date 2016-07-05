
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Plugins;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Mvc.Admin.Controllers;
using CAF.WebSite.Mvc.Admin.Models.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.Services;
using System.Globalization;
using CAF.WebSite.Application.Services.Directory;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public partial class LanguageController : AdminControllerBase
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ISiteService _siteService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly ICommonServices _services;
        private readonly ICountryService _countryService;
        #endregion

        #region Constructors

        public LanguageController(ILanguageService languageService,
            ILocalizationService localizationService,
            ISiteService siteService,
            ISiteMappingService siteMappingService,
            IPermissionService permissionService,
            IWebHelper webHelper,
            AdminAreaSettings adminAreaSettings,
            IPluginFinder pluginFinder,
                ICountryService countryService,
            ICommonServices services)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._siteService = siteService;
            this._siteMappingService = siteMappingService;
            this._permissionService = permissionService;
            this._webHelper = webHelper;
            this._adminAreaSettings = adminAreaSettings;
            this._pluginFinder = pluginFinder;
            this._countryService = countryService;
            this._services = services;
        }

        #endregion

        #region Utilities

        [NonAction]
        private void PrepareFlagsModel(LanguageModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.FlagFileNames = System.IO.Directory
                .EnumerateFiles(_webHelper.MapPath("~/Content/Images/flags/"), "*.png", SearchOption.TopDirectoryOnly)
                .Select(System.IO.Path.GetFileName)
                .ToList();
        }

        [NonAction]
        private void PrepareSitesMappingModel(LanguageModel model, Language language, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableSites = _siteService
                .GetAllSites()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (language != null)
                {
                    model.SelectedSiteIds = _siteMappingService.GetSitesIdsWithAccess(language);
                }
                else
                {
                    model.SelectedSiteIds = new int[0];
                }
            }
        }
        private void PrepareResourceModel(LanguageResourceModel model, LocaleStringResource resource, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (!excludeProperties)
            {
                if (resource != null)
                {
                    model.Name = resource.ResourceName;
                    model.Value = resource.ResourceValue;
                    model.Id = resource.Id;
                    model.LanguageId = resource.LanguageId;
                }
            }
            else
            {
                if (model != null)
                {
                    resource.ResourceName = model.Name;
                    resource.ResourceValue = model.Value;
                }
            }

        }

        private void PrepareLanguageModel(LanguageModel model, Language language, bool excludeProperties)
        {
            var languageId = _services.WorkContext.WorkingLanguage.Id;

            var allCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .OrderBy(x => x.DisplayName)
                .ToList();


            var allCountryNames = _countryService.GetAllCountries(true)
                .ToDictionary(x => x.TwoLetterIsoCode.EmptyNull().ToLower(), x => x.GetLocalized(y => y.Name, languageId, true, false));

            model.AvailableCultures = allCultures
                .Select(x => new SelectListItem { Text = "{0} [{1}]".FormatInvariant(x.DisplayName, x.IetfLanguageTag), Value = x.IetfLanguageTag })
                .ToList();

            model.AvailableTwoLetterLanguageCodes = new List<SelectListItem>();
            model.AvailableFlags = new List<SelectListItem>();

            foreach (var item in allCultures)
            {
                if (!model.AvailableTwoLetterLanguageCodes.Any(x => x.Value.IsCaseInsensitiveEqual(item.TwoLetterISOLanguageName)))
                {
                    // display language name is not provided by net framework
                    var index = item.DisplayName.EmptyNull().IndexOf(" (");

                    if (index == -1)
                        index = item.DisplayName.EmptyNull().IndexOf(" [");

                    var displayName = "{0} [{1}]".FormatInvariant(
                        index == -1 ? item.DisplayName : item.DisplayName.Substring(0, index),
                        item.TwoLetterISOLanguageName);

                    model.AvailableTwoLetterLanguageCodes.Add(new SelectListItem { Text = displayName, Value = item.TwoLetterISOLanguageName });
                }
            }

            foreach (var path in Directory.EnumerateFiles(_services.WebHelper.MapPath("~/Content/Images/flags/"), "*.png", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileNameWithoutExtension(path).EmptyNull().ToLower();
                string countryDescription = null;

                if (allCountryNames.ContainsKey(name))
                    countryDescription = "{0} [{1}]".FormatInvariant(allCountryNames[name], name);

                if (countryDescription.IsEmpty())
                    countryDescription = name;

                model.AvailableFlags.Add(new SelectListItem { Text = countryDescription, Value = Path.GetFileName(path) });
            }

            model.AvailableFlags = model.AvailableFlags.OrderBy(x => x.Text).ToList();

 
        }
        #endregion

        #region Languages

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();
            var languages = _languageService.GetAllLanguages(true).AsQueryable();

            return DataTablesResult.Create(languages.Select(a =>
              new LanguageModel()
              {
                  Id = a.Id,
                  Name = a.Name,
                  LanguageCulture = a.LanguageCulture,
                  UniqueSeoCode = a.UniqueSeoCode,
                  FlagImageFileName = a.FlagImageFileName,
                  Rtl = a.Rtl,
                  Published = a.Published,
                  DisplayOrder = a.DisplayOrder
              }), dataTableParam
           );

        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var model = new LanguageModel();
            PrepareLanguageModel(model, null, false);
            //flags
            PrepareFlagsModel(model);
            //Sites
            PrepareSitesMappingModel(model, null, false);
            //default values
            //model.Published = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var language = model.ToEntity();
                _languageService.InsertLanguage(language);

                //Sites
                _siteMappingService.SaveSiteMappings<Language>(language, model.SelectedSiteIds);

                var plugins = _pluginFinder.GetPluginDescriptors(true);
                var filterLanguages = new List<Language>() { language };

                foreach (var plugin in plugins)
                {
                    _localizationService.ImportPluginResourcesFromXml(plugin, null, false, filterLanguages);
                }

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Languages.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("List");
            }
            PrepareLanguageModel(model, null, true);
            //flags
            PrepareFlagsModel(model);

            //Sites
            PrepareSitesMappingModel(model, null, true);

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = language.ToModel();
            PrepareLanguageModel(model, language, false);
            //flags
            PrepareFlagsModel(model);
            //Sites
            PrepareSitesMappingModel(model, language, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(model.Id);
            if (language == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = _languageService.GetAllLanguages();
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id &&
                    !model.Published)
                {
                    NotifyError("At least one published language is required.");
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                //update
                language = model.ToEntity(language);
                _languageService.UpdateLanguage(language);

                //Sites
                _siteMappingService.SaveSiteMappings<Language>(language, model.SelectedSiteIds);

                //notification
                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Languages.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("List");
            }
            PrepareLanguageModel(model, language, true);
            //flags
            PrepareFlagsModel(model);

            //Sites
            PrepareSitesMappingModel(model, language, true);

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //ensure we have at least one published language
            var allLanguages = _languageService.GetAllLanguages();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
            {
                NotifyError("At least one published language is required.");
                return RedirectToAction("Edit", new { id = language.Id });
            }

            //delete
            _languageService.DeleteLanguage(language);

            //notification
            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Languages.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Resources

        public ActionResult Resources(int languageId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true)
                .Select(x => new SelectListItem
                {
                    Selected = (x.Id.Equals(languageId)),
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            var language = _languageService.GetLanguageById(languageId);
            ViewBag.LanguageId = languageId;
            ViewBag.LanguageName = language.Name;

            // var resources = _localizationService
            //     .GetResourceValues(languageId, true)
            //     .Where(x => x.Key != "!!___EOF___!!" && x.Value != null)
            //     .OrderBy(x => x.Key)
            //     .ToList();
            // var languages = resources
            //         .Take(_adminAreaSettings.GridPageSize).AsQueryable();

            // return DataTablesResult.Create(languages.Select(a =>
            //   new LanguageResourceModel()
            //   {
            //       Id = a.Value.Item1,
            //       LanguageId = languageId,
            //       LanguageName = language.Name,
            //       Name = a.Key,
            //       Value = a.Value.Item2.EmptyNull()
            //   }), null
            //);

            return View();
        }

        [HttpPost]
        public ActionResult Resources(DataTablesParam dataTableParam, int languageId, string resourceName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();
            // var languageId = dataTableParam.bQueryModel.Items.FirstOrDefault().Value.ToString().ToInt();
            var language = _languageService.GetLanguageById(languageId);

            var resources = _localizationService
                .GetResourceValues(languageId, resourceName, true)
                .OrderBy(x => x.Key)
                .Where(x => x.Key != "!!___EOF___!!" && x.Value != null)
                .ToList().AsQueryable();
            if (!resourceName.IsEmpty())
                resources = resources.Where(r => r.Key.Contains(resourceName));
            return DataTablesResult.Create(resources.Select(x =>
             new LanguageResourceModel()
             {
                 LanguageId = languageId,
                 LanguageName = language.Name,
                 Id = x.Value.Item1,
                 Name = x.Key,
                 Value = x.Value.Item2.EmptyNull(),
             }), dataTableParam
          );

        }

        //create
        public ActionResult OptionCreatePopup(int languageId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var model = new LanguageResourceModel();
            model.LanguageId = languageId;


            return View(model);
        }
        [HttpPost]
        public ActionResult OptionCreatePopup(string btnId, string formId, LanguageResourceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var res = _localizationService.GetLocaleStringResourceByName(model.Name, model.LanguageId, false);
            if (res == null)
            {
                var resource = new LocaleStringResource { LanguageId = model.LanguageId };
                resource.ResourceName = model.Name;
                resource.ResourceValue = model.Value;
                resource.IsTouched = true;
                _localizationService.InsertLocaleStringResource(resource);
            }
            else
            {
                return Content(string.Format(_localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), model.Name));
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult OptionEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var resource = _localizationService.GetLocaleStringResourceById(id);
            if (resource == null)
                //No specification attribute option found with the specified id
                return RedirectToAction("List");

            var model = new LanguageResourceModel();
            PrepareResourceModel(model, resource, false);

            return View(model);
        }

        [HttpPost]
        public ActionResult OptionEditPopup(string btnId, string formId, LanguageResourceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var resource = _localizationService.GetLocaleStringResourceById(model.Id);
            // if the resourceName changed, ensure it isn't being used by another resource
            if (!resource.ResourceName.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var res = _localizationService.GetLocaleStringResourceByName(model.Name, model.LanguageId, false);
                if (res != null && res.Id != resource.Id)
                {
                    return Content(string.Format(_localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName));
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            resource.ResourceName = model.Name;
            resource.ResourceValue = model.Value;
            resource.IsTouched = true;
            _localizationService.UpdateLocaleStringResource(resource);

            //If we got this far, something failed, redisplay form
            return View(model);


        }


        public ActionResult ResourceDelete(int id, int languageId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var resource = _localizationService.GetLocaleStringResourceById(id);
            if (resource == null)
                throw new ArgumentException("No resource found with the specified id");
            _localizationService.DeleteLocaleStringResource(resource);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            // return Resources(languageId);
        }

        #endregion

        #region Export / Import

        public ActionResult ExportXml(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            try
            {
                var xml = _localizationService.ExportResourcesToXml(language);
                return new XmlDownloadResult(xml, "language-pack-{0}.xml".FormatInvariant(language.UniqueSeoCode));
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ImportXml(int id, FormCollection form, ImportModeFlags mode, bool updateTouched)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            try
            {
                var file = Request.Files["importxmlfile"];
                if (file != null && file.ContentLength > 0)
                {
                    using (var sr = new StreamReader(file.InputStream, Encoding.UTF8))
                    {
                        string content = sr.ReadToEnd();
                        _localizationService.ImportResourcesFromXml(language, content, mode: mode, updateTouchedResources: updateTouched);
                    }

                }
                else
                {
                    NotifyError(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Languages.Imported"));
                return RedirectToAction("Edit", new { id = language.Id });
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("Edit", new { id = language.Id });
            }

        }

        #endregion
    }
}

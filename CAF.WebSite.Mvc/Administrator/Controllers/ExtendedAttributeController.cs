using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.ExtendedAttributes;
using System;
using System.Linq;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class ExtendedAttributeController : AdminControllerBase
    {
        #region Fields

        private readonly IExtendedAttributeService _extendedAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors

        public ExtendedAttributeController(IExtendedAttributeService extendedAttributeService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IUserActivityService userActivityService,
            IMeasureService measureService, MeasureSettings measureSettings,
            IPermissionService permissionService)
        {
            this._extendedAttributeService = extendedAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._userActivityService = userActivityService;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        [NonAction]
        public void UpdateAttributeLocales(ExtendedAttribute extendedAttribute, ExtendedAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(extendedAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(extendedAttribute,
                                                             x => x.Title,
                                                             localized.Title,
                                                             localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(extendedAttribute,
                                                               x => x.TextPrompt,
                                                               localized.TextPrompt,
                                                               localized.LanguageId);

            }
        }

        [NonAction]
        public void UpdateValueLocales(ExtendedAttributeValue extendedAttributeValue, ExtendedAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(extendedAttributeValue,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        private void PrepareExtendedAttributeModel(ExtendedAttributeModel model, ExtendedAttribute extendedAttribute, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            ////tax categories
            //var taxCategories = _taxCategoryService.GetAllTaxCategories();
            //foreach (var tc in taxCategories)
            //    model.AvailableTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString(), Selected = extendedAttribute != null && !excludeProperties && tc.Id == extendedAttribute.TaxCategoryId });
        }

        #endregion

        #region Extended attributes

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var extendedAttributes = _extendedAttributeService.GetAllExtendedAttributes(true);

            var total = extendedAttributes.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = extendedAttributes.Select(x =>
                {
                    var caModel = x.ToModel();
                    caModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return caModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        //create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new ExtendedAttributeModel();
            model.IsActive = true;

            //locales
            AddLocales(_languageService, model.Locales);
            PrepareExtendedAttributeModel(model, null, true);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(ExtendedAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Name))
            {
                var ext2 = _extendedAttributeService.GetExtendedAttributeByName(model.Name);
                if (ext2 != null)
                    ModelState.AddModelError("", "Name is already registered");
            }

            if (ModelState.IsValid)
            {
                var extendedAttribute = model.ToEntity();
                _extendedAttributeService.InsertExtendedAttribute(extendedAttribute);
                UpdateAttributeLocales(extendedAttribute, model);

                //activity log
                _userActivityService.InsertActivity("AddNewExtendedAttribute", _localizationService.GetResource("ActivityLog.AddNewExtendedAttribute"), extendedAttribute.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.ExtendedAttributes.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = extendedAttribute.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareExtendedAttributeModel(model, null, true);
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var extendedAttribute = _extendedAttributeService.GetExtendedAttributeById(id);
            if (extendedAttribute == null)
                //No extended attribute found with the specified id
                return RedirectToAction("List");

            var model = extendedAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = extendedAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.TextPrompt = extendedAttribute.GetLocalized(x => x.TextPrompt, languageId, false, false);
            });
            PrepareExtendedAttributeModel(model, extendedAttribute, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(ExtendedAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var extendedAttribute = _extendedAttributeService.GetExtendedAttributeById(model.Id);
            if (extendedAttribute == null)
                //No extended attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                extendedAttribute = model.ToEntity(extendedAttribute);
                _extendedAttributeService.UpdateExtendedAttribute(extendedAttribute);

                UpdateAttributeLocales(extendedAttribute, model);

                //activity log
                _userActivityService.InsertActivity("EditExtendedAttribute", _localizationService.GetResource("ActivityLog.EditExtendedAttribute"), extendedAttribute.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.ExtendedAttributes.Updated"));
                return continueEditing ? RedirectToAction("Edit", extendedAttribute.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareExtendedAttributeModel(model, extendedAttribute, true);
            return View(model);
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var extendedAttribute = _extendedAttributeService.GetExtendedAttributeById(id);
            _extendedAttributeService.DeleteExtendedAttribute(extendedAttribute);

            //activity log
            _userActivityService.InsertActivity("DeleteExtendedAttribute", _localizationService.GetResource("ActivityLog.DeleteExtendedAttribute"), extendedAttribute.Name);

            NotifySuccess(_localizationService.GetResource("Admin.Catalog.Attributes.ExtendedAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Extended attribute values

        //list
        [HttpPost]
        public ActionResult ValueList(int extendedAttributeId, DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var values = _extendedAttributeService.GetExtendedAttributeValues(extendedAttributeId);
 
            var total = values.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = values.Select(x =>
                {
                    var model = x.ToModel();
                    //locales
                    //AddLocales(_languageService, model.Locales, (locale, languageId) =>
                    //{
                    //    locale.Name = x.GetLocalized(y => y.Name, languageId, false, false);
                    //});
                    return model;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        //create
        public ActionResult ValueCreatePopup(int extendedAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new ExtendedAttributeValueModel();
            model.ExtendedAttributeId = extendedAttributeId;
           
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult ValueCreatePopup(string btnId, string formId, ExtendedAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var extendedAttribute = _extendedAttributeService.GetExtendedAttributeById(model.ExtendedAttributeId);
            if (extendedAttribute == null)
                //No extended attribute found with the specified id
                return RedirectToAction("List");

          
            if (ModelState.IsValid)
            {
                var sao = model.ToEntity();

                _extendedAttributeService.InsertExtendedAttributeValue(sao);
                UpdateValueLocales(sao, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var cav = _extendedAttributeService.GetExtendedAttributeValueById(id);
            if (cav == null)
                //No extended attribute value found with the specified id
                return RedirectToAction("List");

            var model = cav.ToModel();
         
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = cav.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult ValueEditPopup(string btnId, string formId, ExtendedAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var cav = _extendedAttributeService.GetExtendedAttributeValueById(model.Id);
            if (cav == null)
                //No extended attribute value found with the specified id
                return RedirectToAction("List");

         
            if (ModelState.IsValid)
            {
                cav = model.ToEntity(cav);
                _extendedAttributeService.UpdateExtendedAttributeValue(cav);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        public ActionResult ValueDelete(int valueId, int extendedAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var cav = _extendedAttributeService.GetExtendedAttributeValueById(valueId);
            if (cav == null)
                throw new ArgumentException("No extended attribute value found with the specified id");
            _extendedAttributeService.DeleteExtendedAttributeValue(cav);


            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Catalog.Attributes.ExtendedAttributesValue.Deleted") }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}

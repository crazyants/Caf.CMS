using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Mvc.Admin.Models.ModelTemplates;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Infrastructure.Core.Events;
using CAF.WebSite.Application.Services.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Articles;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class ModelTemplateController : AdminControllerBase
    {
        #region Fields
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISiteService _storeService;
        private readonly ISiteMappingService _storeMappingService;
        private readonly IEventPublisher _eventPublisher;
        #endregion Fields

        #region Constructors

        public ModelTemplateController(ILanguageService languageService,
            IModelTemplateService modelTemplateService, IUrlRecordService urlRecordService,
            ILocalizedEntityService localizedEntityService, ILocalizationService localizationService,
            IPermissionService permissionService, ISiteService storeService,
            ISiteMappingService storeMappingService, IEventPublisher eventPublisher)
        {
            this._modelTemplateService = modelTemplateService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._urlRecordService = urlRecordService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities


        #endregion

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageModelTemplates))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageModelTemplates))
                return AccessDeniedView();

            var modelTemplates = _modelTemplateService.GetAllModelTemplates();
            return DataTablesResult.Create(modelTemplates.Select(x => x.ToModel()).AsQueryable(), dataTableParam);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageModelTemplates))
                return AccessDeniedView();

            var model = new ModelTemplateModel();

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(ModelTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageModelTemplates))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {

                var modelTemplate = model.ToEntity();

                _modelTemplateService.InsertModelTemplate(modelTemplate);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.ModelTemplates.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = modelTemplate.Id }) : RedirectToAction("List");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageModelTemplates))
                return AccessDeniedView();

            var modelTemplate = _modelTemplateService.GetModelTemplateById(id);
            if (modelTemplate == null)
                //No modelTemplate found with the specified id
                return RedirectToAction("List");

            var model = modelTemplate.ToModel();


            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(ModelTemplateModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageModelTemplates))
                return AccessDeniedView();

            var modelTemplate = _modelTemplateService.GetModelTemplateById(model.Id);
            if (modelTemplate == null)
                //No modelTemplate found with the specified id
                return RedirectToAction("List");




            if (ModelState.IsValid)
            {
                modelTemplate = model.ToEntity(modelTemplate);

                _modelTemplateService.UpdateModelTemplate(modelTemplate);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.ModelTemplates.Updated"));
                return continueEditing ? RedirectToAction("Edit", modelTemplate.Id) : RedirectToAction("List");
            }


            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageModelTemplates))
                return AccessDeniedView();

            var modelTemplate = _modelTemplateService.GetModelTemplateById(id);
            if (modelTemplate == null)
                //No modelTemplate found with the specified id
                return RedirectToAction("List");

            _modelTemplateService.DeleteModelTemplate(modelTemplate);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.ModelTemplates.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}

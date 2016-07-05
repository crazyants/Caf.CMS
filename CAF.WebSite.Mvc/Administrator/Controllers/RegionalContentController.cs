using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.RegionalContents;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Admin.Models.RegionalContents;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Tables;
using CAF.Infrastructure.Core.Domain.Cms.RegionalContents;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class RegionalContentController : AdminControllerBase
    {
        #region Fields

        private readonly IRegionalContentService _regionalContentService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISiteService _siteService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IPictureService _pictureService;
        #endregion Fields

        #region Constructors

        public RegionalContentController(IRegionalContentService regionalContentService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, ILocalizationService localizationService,
            IPermissionService permissionService, ISiteService siteService, IPictureService pictureService,
            ISiteMappingService siteMappingService)
        {
            this._pictureService = pictureService;
            this._regionalContentService = regionalContentService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._siteService = siteService;
            this._siteMappingService = siteMappingService;
        }

        #endregion

        #region Utilities

        [NonAction]
        private void PrepareSitesMappingModel(RegionalContentModel model, RegionalContent newsItem, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableSites = _siteService
                .GetAllSites()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (newsItem != null)
                {
                    model.SelectedSiteIds = _siteMappingService.GetSitesIdsWithAccess(newsItem);
                }
                else
                {
                    model.SelectedSiteIds = new int[0];
                }
            }
        }

        #endregion

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRegionalContents))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRegionalContents))
                return AccessDeniedView();
            var regionalContents = _regionalContentService.GetAllRegionalContents(0, 0, dataTableParam.PageIndex, dataTableParam.PageSize);
            var regionalContentsQueryable = regionalContents
              .Select(x =>
              {
                  var regionalContent = x.ToModel();
                   
                  return regionalContent;
              })
              .ToList().AsQueryable();
            return DataTablesResult.Create(regionalContentsQueryable, dataTableParam);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRegionalContents))
                return AccessDeniedView();
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = new RegionalContentModel();
            //Sites
            PrepareSitesMappingModel(model, null, false);
         
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [ValidateInput(false)]
        public ActionResult Create(RegionalContentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRegionalContents))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {

                var regionalContent = model.ToEntity();
                regionalContent.AddEntitySysParam();
                _regionalContentService.InsertRegionalContent(regionalContent);
                //Sites
                _siteMappingService.SaveSiteMappings<RegionalContent>(regionalContent, model.SelectedSiteIds);
                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.RegionalContents.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = regionalContent.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            //Sites
            PrepareSitesMappingModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRegionalContents))
                return AccessDeniedView();

            var regionalContent = _regionalContentService.GetRegionalContentById(id);
            if (regionalContent == null)
                //No regionalContent found with the specified id
                return RedirectToAction("List");

         
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            var model = regionalContent.ToModel();

            //sites
            PrepareSitesMappingModel(model, regionalContent, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [ValidateInput(false)]
        public ActionResult Edit(RegionalContentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRegionalContents))
                return AccessDeniedView();

            var regionalContent = _regionalContentService.GetRegionalContentById(model.Id);
            if (regionalContent == null)
                //No regionalContent found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                regionalContent = model.ToEntity(regionalContent);
                regionalContent.AddEntitySysParam(false, true);
                _regionalContentService.UpdateRegionalContent(regionalContent);

                //Sites
                _siteMappingService.SaveSiteMappings<RegionalContent>(regionalContent, model.SelectedSiteIds);
                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.RegionalContents.Updated"));
                return continueEditing ? RedirectToAction("Edit", regionalContent.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            //sites
            PrepareSitesMappingModel(model, regionalContent, true);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRegionalContents))
                return AccessDeniedView();

            var regionalContent = _regionalContentService.GetRegionalContentById(id);
            if (regionalContent == null)
                //No regionalContent found with the specified id
                return RedirectToAction("List");

            _regionalContentService.DeleteRegionalContent(regionalContent);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.RegionalContents.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }

    
}

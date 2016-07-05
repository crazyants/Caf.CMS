using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.Mvc.JQuery.Datatables.Core;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public partial class SiteController : AdminControllerBase
    {
        private readonly ISiteService _siteService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        public SiteController(ISiteService siteService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._siteService = siteService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }
        // GET: Edit
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            return View();
        }

        public ActionResult AllSites(string label, int selectedId = 0)
        {
            var sites = _siteService.GetAllSites();

            if (label.HasValue())
            {
                sites.Insert(0, new Site { Name = label, Id = 0 });
            }

            var list =
                from m in sites
                select new
                {
                    id = m.Id.ToString(),
                    text = m.Name,
                    selected = m.Id == selectedId
                };

            return new JsonResult { Data = list.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();
            var siteModels = _siteService.GetAllSites().AsQueryable();

            return DataTablesResult.Create(siteModels.Select(a =>
              new SiteModel()
              {
                  Id = a.Id,
                  Name = a.Name,
                  Url = a.Url,
                  ContentDeliveryNetwork = a.ContentDeliveryNetwork,
                  Hosts = a.Hosts,
                  DisplayOrder = a.DisplayOrder,
              }), dataTableParam
           );
        }

        public ActionResult Create()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            var model = new SiteModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(SiteModel model, bool continueEditing)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var site = model.ToEntity();
                //ensure we have "/" at the end
                site.Url = site.Url.EnsureEndsWith("/");
                _siteService.InsertSite(site);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Sites.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = site.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            var site = _siteService.GetSiteById(id);
            if (site == null)
                //No site found with the specified id
                return RedirectToAction("List");

            var model = site.ToModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(SiteModel model, bool continueEditing)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            var site = _siteService.GetSiteById(model.Id);
            if (site == null)
                //No site found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                site = model.ToEntity(site);
                //ensure we have "/" at the end
                site.Url = site.Url.EnsureEndsWith("/");
                _siteService.UpdateSite(site);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Sites.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = site.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            var site = _siteService.GetSiteById(id);
            if (site == null)
                //No site found with the specified id
                return RedirectToAction("List");

            try
            {
                _siteService.DeleteSite(site);

                //when we delete a site we should also ensure that all "per site" settings will also be deleted
                var settingsToDelete = _settingService
                    .GetAllSettings()
                    .Where(s => s.SiteId == id)
                    .ToList();
                foreach (var setting in settingsToDelete)
                    _settingService.DeleteSetting(setting);
                //when we had two sites and now have only one site, we also should delete all "per site" settings
                var allSites = _siteService.GetAllSites();
                if (allSites.Count == 1)
                {
                    settingsToDelete = _settingService
                        .GetAllSettings()
                        .Where(s => s.SiteId == allSites[0].Id)
                        .ToList();
                    foreach (var setting in settingsToDelete)
                        _settingService.DeleteSetting(setting);
                }

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Sites.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("Edit", new { id = site.Id });
            }
        }
    }
}

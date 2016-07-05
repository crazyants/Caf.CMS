using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Clients;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Cms.Clients;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Clients;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class ClientController : AdminControllerBase
    {
        #region Fields


        private readonly IClientService _clientService;
        private readonly ISiteService _storeService;
        private readonly ISiteMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IWorkContext _workContext;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Constructors

        public ClientController(IClientService clientService,

            ISiteService storeService, ISiteMappingService storeMappingService,
            IUrlRecordService urlRecordService, IPictureService pictureService,
            ILanguageService languageService, ILocalizationService localizationService, ILocalizedEntityService localizedEntityService,
            IWorkContext workContext, IDateTimeHelper dateTimeHelper,
            IUserActivityService userActivityService, IPermissionService permissionService,
            AdminAreaSettings adminAreaSettings)
        {
            this._clientService = clientService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._workContext = workContext;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected void UpdateLocales(Client client, ClientModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(client,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(client,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(client,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(client,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(client,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = client.ValidateSeName(localized.SeName, localized.Name, false, localized.LanguageId);
                _urlRecordService.SaveSlug(client, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected void UpdatePictureSeoNames(Client client)
        {
            var picture = _pictureService.GetPictureById(client.PictureId.GetValueOrDefault());
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(client.Name));
        }

        [NonAction]
        private void PrepareClientModel(ClientModel model, Client client, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableSites = _storeService
                .GetAllSites()
                .Select(s => s.ToModel())
                .ToList();

            if (!excludeProperties)
            {
                if (client != null)
                {
                    model.SelectedSiteIds = _storeMappingService.GetSitesIdsWithAccess(client);
                }
                else
                {
                    model.SelectedSiteIds = new int[0];
                }
            }

            if (client != null)
            {
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(client.CreatedOnUtc, DateTimeKind.Utc);
                if (client.ModifiedOnUtc.HasValue) model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(client.ModifiedOnUtc.Value, DateTimeKind.Utc);
            }
        }

        #endregion

        #region List

        //ajax
        public ActionResult AllClients(string label, int selectedId)
        {
            var clients = _clientService.GetAllClients(true);
            if (label.HasValue())
            {
                clients.Insert(0, new Client { Name = label, Id = 0 });
            }

            var list = from m in clients
                       select new
                       {
                           id = m.Id.ToString(),
                           text = m.Name,
                           selected = m.Id == selectedId
                       };

            return new JsonResult { Data = list.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

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
        public ActionResult List(DataTablesParam dataTableParam, ClientListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var clients = _clientService.GetAllClients(model.SearchClientName,
                dataTableParam.PageIndex, dataTableParam.PageSize, true);

            var total = clients.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = clients.Select(x => x.ToModel()).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            }; ;
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new ClientModel();

            //locales
            AddLocales(_languageService, model.Locales);
            PrepareClientModel(model, null, false);

            model.Published = true;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(ClientModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var client = model.ToEntity();
                client.AddEntitySysParam(true, true);

                _clientService.InsertClient(client);

                //search engine name
                model.SeName = client.ValidateSeName(model.SeName, client.Name, true);
                _urlRecordService.SaveSlug(client, model.SeName, 0);

                //locales
                UpdateLocales(client, model);

                //update picture seo file name
                UpdatePictureSeoNames(client);

                //Sites
                _storeMappingService.SaveSiteMappings<Client>(client, model.SelectedSiteIds);

                //activity log
                _userActivityService.InsertActivity("AddNewClient", _localizationService.GetResource("ActivityLog.AddNewClient"), client.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Clients.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = client.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareClientModel(model, null, true);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var client = _clientService.GetClientById(id);
            if (client == null || client.Deleted)
                return RedirectToAction("List");

            var model = client.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = client.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = client.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = client.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = client.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = client.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = client.GetSeName(languageId, false, false);
            });
         
            PrepareClientModel(model, client, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(ClientModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var client = _clientService.GetClientById(model.Id);
            if (client == null || client.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = client.PictureId.GetValueOrDefault();
                client = model.ToEntity(client);
                client.AddEntitySysParam(false, true);
                _clientService.UpdateClient(client);

                //search engine name
                model.SeName = client.ValidateSeName(model.SeName, client.Name, true);
                _urlRecordService.SaveSlug(client, model.SeName, 0);

                //locales
                UpdateLocales(client, model);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != client.PictureId.GetValueOrDefault())
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(client);

                //Sites
                _storeMappingService.SaveSiteMappings<Client>(client, model.SelectedSiteIds);

                //activity log
                _userActivityService.InsertActivity("EditClient", _localizationService.GetResource("ActivityLog.EditClient"), client.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Catalog.Clients.Updated"));
                return continueEditing ? RedirectToAction("Edit", client.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareClientModel(model, client, true);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var client = _clientService.GetClientById(id);
            if (client == null)
                return RedirectToAction("List");

            _clientService.DeleteClient(client);

            //activity log
            _userActivityService.InsertActivity("DeleteClient", _localizationService.GetResource("ActivityLog.DeleteClient"), client.Name);

            NotifySuccess(_localizationService.GetResource("Admin.Catalog.Clients.Deleted"));
            return RedirectToAction("List");
        }

        #endregion


    }
}

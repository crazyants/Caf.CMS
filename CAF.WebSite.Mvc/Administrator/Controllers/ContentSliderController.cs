using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Mvc.Admin.Models.Settings;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;


namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class ContentSliderController :  AdminControllerBase
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IPictureService _pictureService;
        private readonly ContentSliderSettings _contentSliderSettings;
		private readonly ISiteService _siteService;

        #endregion

        #region Constructors

        public ContentSliderController( ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ILocalizedEntityService localizedEntityService, 
            ILanguageService languageService,
            IPictureService pictureService,
            ContentSliderSettings contentSliderSettings,
			ISiteService siteService,
            IWorkContext workContext)
        {
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._pictureService = pictureService;
            this._contentSliderSettings = contentSliderSettings;
			this._siteService = siteService;
            this._workContext = workContext;
        }
        
        #endregion

        #region Methods

		private ContentSliderSettingsModel PrepareContentSliderSettingsModel(ContentSliderSettingsModel modelIn = null)
		{
			int rowIndex = 0;
			var model = _contentSliderSettings.ToModel();

			model.AvailableSites.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
			foreach (var s in _siteService.GetAllSites())
			{
				model.AvailableSites.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });
			}

			foreach (var slide in model.Slides)
			{
				slide.SlideIndex = rowIndex++;

				var language = _languageService.GetLanguageByCulture(slide.LanguageCulture);
				if (language != null)
				{
					slide.LanguageName = language.Name;
				}
				else
				{
					var seoCode = _workContext.GetDefaultLanguageSeoCode();
					slide.LanguageName = _languageService.GetLanguageBySeoCode(seoCode).Name;
				}
			}

			// note order: first SlideIndex then search filter.
			if (modelIn != null && modelIn.SearchSiteId > 0)
			{
				model.Slides = model.Slides
					.Where(x => x.LimitedToSites && x.SelectedSiteIds != null && x.SelectedSiteIds.Contains(modelIn.SearchSiteId))
					.ToList();
			}

			return model;
		}

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContentSlider))
                return AccessDeniedView();

			var model = PrepareContentSliderSettingsModel();

            return View(model);
        }

		[HttpPost]
		public ActionResult SlideList(DataTablesParam dataTableParam, ContentSliderSettingsModel model)
		{
		 
			var viewModel = PrepareContentSliderSettingsModel(model);

            var contentSliderSettingsModels = viewModel.Slides.OrderBy(s => s.LanguageName).ThenBy(s => s.DisplayOrder);

            var total = contentSliderSettingsModels.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = contentSliderSettingsModels.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
		}
        
        [HttpPost]
        public ActionResult Index(ContentSliderSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContentSlider))
                return AccessDeniedView();

            _contentSliderSettings.IsActive = model.IsActive;
            _contentSliderSettings.ContentSliderHeight = model.ContentSliderHeight;
            _contentSliderSettings.BackgroundPictureId = model.BackgroundPictureId;
            _contentSliderSettings.BackgroundPictureUrl = _pictureService.GetPictureUrl(model.BackgroundPictureId);
            _contentSliderSettings.AutoPlay = model.AutoPlay;
            _contentSliderSettings.AutoPlayDelay = model.AutoPlayDelay;

            _settingService.SaveSetting(_contentSliderSettings);

			var viewModel = PrepareContentSliderSettingsModel(model);

            return View(viewModel);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult CreateSlide()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContentSlider))
                return AccessDeniedView();

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = new ContentSliderSlideModel();
			model.AvailableSites = _siteService.GetAllSites().Select(s => s.ToModel()).ToList();

			var lastSlide = _contentSliderSettings.Slides.OrderByDescending(x => x.DisplayOrder).FirstOrDefault();
			if (lastSlide != null)
				model.DisplayOrder = lastSlide.DisplayOrder + 1;
            
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSlide(ContentSliderSlideModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContentSlider))
                return AccessDeniedView();

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            if (ModelState.IsValid)
            {
                model.PictureUrl = _pictureService.GetPictureUrl(model.PictureId);
                model.BackgroundPictureId = model.BackgroundPictureId;
                model.BackgroundPictureUrl = _pictureService.GetPictureUrl(model.BackgroundPictureId);
                model.LanguageName = _languageService.GetLanguageByCulture(model.LanguageCulture).Name;
                _contentSliderSettings.Slides.Add(model.ToEntity());
                _settingService.SaveSetting(_contentSliderSettings);
                
                NotifySuccess(_localizationService.GetResource("Admin.Configuration.ContentSlider.Slide.Added"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult EditSlide(int index)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContentSlider))
                return AccessDeniedView();

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);

            var model = _contentSliderSettings.Slides[index].ToModel();

			if (model == null)
				return RedirectToAction("Index");

            model.Id = index;
			model.AvailableSites = _siteService.GetAllSites().Select(s => s.ToModel()).ToList();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult EditSlide(ContentSliderSlideModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContentSlider))
                return AccessDeniedView();

            if (model == null)
                //No slide found
                return RedirectToAction("Index");

            var index = Request.QueryString["index"].ToInt();

            if (ModelState.IsValid)
            {
                Language lang = null;
                int langId = model.LanguageCulture.ToInt();

                if (langId > 0)
                {
                    lang = _languageService.GetLanguageById(langId);
                }
                else
                {
                    lang = _languageService.GetLanguageByCulture(model.LanguageCulture);
                }

                if (lang != null)
                {
                    model.LanguageName = lang.Name;
                }

                model.PictureUrl = _pictureService.GetPictureUrl(model.PictureId);
                model.BackgroundPictureId = model.BackgroundPictureId;
                model.BackgroundPictureUrl = _pictureService.GetPictureUrl(model.BackgroundPictureId);
                //delete an old picture (if deleted or updated)
                int prevPictureId = _contentSliderSettings.Slides[index].PictureId;
                if (prevPictureId > 0 && prevPictureId != model.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                int prevBackgroundPictureId = _contentSliderSettings.Slides[index].BackgroundPictureId;
                if (prevBackgroundPictureId > 0 && prevBackgroundPictureId != model.BackgroundPictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevBackgroundPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                _contentSliderSettings.Slides[index] = model.ToEntity();
                _settingService.SaveSetting(_contentSliderSettings);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.ContentSlider.Slide.Updated"));

                return continueEditing ? RedirectToAction("EditSlide", new { index = index }) : RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost, ActionName("DeleteSlide")]
        public ActionResult DeleteSlideConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContentSlider))
                return AccessDeniedView();

            try
            {
                var slidePicture = _pictureService.GetPictureById(_contentSliderSettings.Slides[id].PictureId);
                if (slidePicture != null)
                    _pictureService.DeletePicture(slidePicture);

                _contentSliderSettings.Slides.RemoveAt(id);
                _settingService.SaveSetting(_contentSliderSettings);
                NotifySuccess(_localizationService.GetResource("Admin.Configuration.ContentSlider.Slide.Deleted"));
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("Edit", new { index = id });
            }
        }

        #endregion
    }
}

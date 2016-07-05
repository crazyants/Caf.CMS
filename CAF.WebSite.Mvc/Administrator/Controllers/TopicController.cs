using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Topics;
using CAF.Infrastructure.Core.Domain.Cms.Topic;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Mvc.Admin.Models.Topics;
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

    public class TopicController : AdminControllerBase
    {
        #region Fields
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
		private readonly ISiteService _storeService;
		private readonly ISiteMappingService _storeMappingService;
        private readonly IEventPublisher _eventPublisher;
        #endregion Fields

        #region Constructors

        public TopicController(ITopicService topicService, ILanguageService languageService,
            IModelTemplateService modelTemplateService, IUrlRecordService urlRecordService,
            ILocalizedEntityService localizedEntityService, ILocalizationService localizationService,
			IPermissionService permissionService, ISiteService storeService,
            ISiteMappingService storeMappingService, IEventPublisher eventPublisher)
        {
            this._modelTemplateService = modelTemplateService;
            this._topicService = topicService;
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

        [NonAction]
        public void UpdateLocales(Topic topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(topic,
                                                               x => x.Title,
                                                               localized.Title,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);
            }
        }

        [NonAction]
        protected void PrepareTopicModel(TopicModel model, Topic topic)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            #region templates

            var templates = _modelTemplateService.GetAllModelTemplates();
            var singleTemplate = templates.Where(p => p.TemplageTypeId == (int)TemplateTypeFormat.SinglePage).ToList();
            foreach (var template in singleTemplate)
            {
                model.AvailableTopicTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }

            #endregion
        }

		[NonAction]
		private void PrepareSitesMappingModel(TopicModel model, Topic topic, bool excludeProperties)
		{
			if (model == null)
				throw new ArgumentNullException("model");

			model.AvailableSites = _storeService
                .GetAllSites()
				.Select(s => s.ToModel())
				.ToList();
			if (!excludeProperties)
			{
				if (topic != null)
				{
					model.SelectedSiteIds = _storeMappingService.GetSitesIdsWithAccess(topic);
				}
				else
				{
					model.SelectedSiteIds = new int[0];
				}
			}
		}

		[NonAction]
		protected void SaveSiteMappings(Topic topic, TopicModel model)
		{
			var existingSiteMappings = _storeMappingService.GetSiteMappings(topic);
			var allSites = _storeService.GetAllSites();
			foreach (var store in allSites)
			{
				if (model.SelectedSiteIds != null && model.SelectedSiteIds.Contains(store.Id))
				{
					//new role
					if (existingSiteMappings.Where(sm => sm.SiteId == store.Id).Count() == 0)
						_storeMappingService.InsertSiteMapping(topic, store.Id);
				}
				else
				{
					//removed role
					var storeMappingToDelete = existingSiteMappings.Where(sm => sm.SiteId == store.Id).FirstOrDefault();
					if (storeMappingToDelete != null)
						_storeMappingService.DeleteSiteMapping(storeMappingToDelete);
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

			var model = new TopicListModel();
			//stores
			model.AvailableSites.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
			foreach (var s in _storeService.GetAllSites())
				model.AvailableSites.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });

			return View(model);
        }

        [HttpPost]
		public ActionResult List(DataTablesParam dataTableParam,TopicListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();
 
          //  var SearchSiteId = dataTableParam.bQueryModel.Items.FirstOrDefault().Value.ToString().ToInt();
            var topics = _topicService.GetAllTopics(model.SearchSiteId);
            return DataTablesResult.Create(topics.Select(x => x.ToModel()).AsQueryable(), dataTableParam);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var model = new TopicModel();
            PrepareTopicModel(model,null);
			//Sites
			PrepareSitesMappingModel(model, null, false);
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(TopicModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (!model.IsPasswordProtected)
                {
                    model.Password = null;
                }

                var topic = model.ToEntity();
                topic.AddEntitySysParam();
                _topicService.InsertTopic(topic);

                // SEO
                topic.SystemName = topic.ValidateSeName(model.SystemName, model.Title, true);
                _urlRecordService.SaveSlug(topic, model.SystemName, 0);

                //locales
                UpdateLocales(topic, model);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = topic.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareTopicModel(model, null);
			//Sites
			PrepareSitesMappingModel(model, null, true);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            var model = topic.ToModel();
            model.Url = Url.RouteUrl("Topic", new { SystemName = topic.SystemName }, "http");
            PrepareTopicModel(model, null);
			//Site
			PrepareSitesMappingModel(model, topic, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = topic.GetLocalized(x => x.Title, languageId, false, false);
                locale.Body = topic.GetLocalized(x => x.Body, languageId, false, false);
                locale.MetaKeywords = topic.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = topic.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = topic.GetLocalized(x => x.MetaTitle, languageId, false, false);
            });

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(TopicModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(model.Id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            model.Url = Url.RouteUrl("Topic", new { SystemName = topic.SystemName }, "http");

            if (!model.IsPasswordProtected)
            {
                model.Password = null;
            }

            if (ModelState.IsValid)
            {
                topic = model.ToEntity(topic);
                topic.AddEntitySysParam(false, true);

                _topicService.UpdateTopic(topic);

                // SEO
                topic.SystemName = topic.ValidateSeName(model.SystemName, model.Title, true);
                _urlRecordService.SaveSlug(topic, model.SystemName, 0);

				//Sites
				SaveSiteMappings(topic, model);
                //locales
                UpdateLocales(topic, model);

                _eventPublisher.Publish(new ModelBoundEvent(model, topic, form));

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));
                return continueEditing ? RedirectToAction("Edit", topic.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareTopicModel(model, null);

			//Site
			PrepareSitesMappingModel(model, topic, true);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            _topicService.DeleteTopic(topic);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));
            return RedirectToAction("List");
        }
        
        #endregion
    }
}

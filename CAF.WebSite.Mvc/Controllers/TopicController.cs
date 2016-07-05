using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Topics;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Security;
using CAF.WebSite.Mvc.Models.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Controllers
{
    /// <summary>
    /// 单页
    /// </summary>
    public partial class TopicController : PublicControllerBase
    {
        #region Fields

        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;
        private readonly IModelTemplateService _modelTemplateService;
        #endregion

        #region Constructors

        public TopicController(ITopicService topicService,
            ILocalizationService localizationService,
             IModelTemplateService modelTemplateService,
            IWorkContext workContext,
            ISiteContext siteContext,
            ICacheManager cacheManager)
        {
            this._modelTemplateService = modelTemplateService;
            this._topicService = topicService;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected TopicModel PrepareTopicModel(string systemName)
        {
            //load by site
            var topic = _topicService.GetTopicBySystemName(systemName, _siteContext.CurrentSite.Id);
            if (topic == null)
                return null;

            var titleTag = "h3";
            if (topic.TitleTag != null)
                titleTag = topic.TitleTag;
            else if (!topic.RenderAsWidget)
                titleTag = "h1";

            var model = new TopicModel()
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Title),
                Body = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Body),
                MetaKeywords = topic.GetLocalized(x => x.MetaKeywords),
                MetaDescription = topic.GetLocalized(x => x.MetaDescription),
                MetaTitle = topic.GetLocalized(x => x.MetaTitle),
                TitleTag = titleTag,
            };
            //template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.ARTICLE_TEMPLATE_MODEL_KEY, topic.TopicTemplateId);
            model.TopicTemplateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _modelTemplateService.GetModelTemplateById(topic.TopicTemplateId);
                if (template == null)
                    template = _modelTemplateService.GetAllModelTemplates().FirstOrDefault();
                return template.ViewPath;
            });
            return model;
        }
     
        #endregion

        #region Methods
     
        /// <summary>
        /// 单页明细
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns></returns>
        public ActionResult TopicDetails(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, _siteContext.CurrentSite.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return HttpNotFound();
            return View(cacheModel.TopicTemplateViewPath, cacheModel);
        }
        /// <summary>
        /// 单页明细弹出
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns></returns>
        public ActionResult TopicDetailsPopup(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, _siteContext.CurrentSite.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return HttpNotFound();

            ViewBag.IsPopup = true;
            return View("TopicDetails", cacheModel);
        }
        /// <summary>
        /// 单页明细-部分视图
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <param name="bodyOnly">只读主体</param>
        /// <returns></returns>
        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult TopicBlock(string systemName, bool bodyOnly = false)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, _siteContext.CurrentSite.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return Content("");

            ViewBag.BodyOnly = bodyOnly;
            return PartialView(cacheModel);
        }
        /// <summary>
        /// 单页明细
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <param name="partialView">页面名称</param>
        /// <param name="bodyOnly">只读主体</param>
        /// <returns></returns>
        public ActionResult TopicBlockPartial(string systemName, string partialView, bool bodyOnly = false)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, _siteContext.CurrentSite.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return HttpNotFound();
            ViewBag.BodyOnly = bodyOnly;
            return PartialView(partialView, cacheModel);
        }

        [ChildActionOnly]
        public ActionResult TopicWidget(TopicWidgetModel model)
        {
            return PartialView(model);
        }
        /// <summary>
        /// 密码访问验证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult Authenticate(int id, string password)
        {
            var authResult = false;
            var title = string.Empty;
            var body = string.Empty;
            var error = string.Empty;

            var topic = _topicService.GetTopicById(id);

            if (topic != null)
            {
                if (topic.Password != null && topic.Password.Equals(password))
                {
                    authResult = true;
                    title = topic.GetLocalized(x => x.Title);
                    body = topic.GetLocalized(x => x.Body);
                }
                else
                {
                    error = _localizationService.GetResource("Topic.WrongPassword");
                }
            }
            return Json(new { Authenticated = authResult, Title = title, Body = body, Error = error });
        }

        #endregion
    }
}
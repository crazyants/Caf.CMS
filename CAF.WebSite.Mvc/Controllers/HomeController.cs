
using CAF.WebSite.Mvc.Models.Users;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.IO;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Authentication;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI.Security;
using CAF.WebSite.Mvc.Models.Common;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Topics;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Html;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Cms;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Models.Topics;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Mvc.Models.Catalog;
using CAF.WebSite.Mvc.Models.Feedbacks;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Feedbacks;
using CAF.WebSite.Mvc.Models.Articles;


namespace CAF.WebSite.Mvc.Controllers
{

    public class HomeController : BaseController
    {
        #region Fields
        private readonly ICommonServices _services;
        private readonly Lazy<IArticleCategoryService> _articleCategoryService;
        private readonly Lazy<IArticleService> _articleService;
        private readonly Lazy<IFeedbackService> _feedbackService;
        private readonly Lazy<IWebHelper> _webHelper;
        private readonly Lazy<ITopicService> _topicService;
        private readonly Lazy<IQueuedEmailService> _queuedEmailService;
        private readonly Lazy<IEmailAccountService> _emailAccountService;
        private readonly Lazy<ISitemapGenerator> _sitemapGenerator;
        private readonly Lazy<CaptchaSettings> _captchaSettings;
        private readonly Lazy<CommonSettings> _commonSettings;

        #endregion

        #region Constructors
        public HomeController(
                ICommonServices services,
                Lazy<IArticleCategoryService> articleCategoryService,
                Lazy<IArticleService> articleService,
                Lazy<IFeedbackService> feedbackService,
                Lazy<IWebHelper> webHelper,
                Lazy<ITopicService> topicService,
                Lazy<IQueuedEmailService> queuedEmailService,
                Lazy<IEmailAccountService> emailAccountService,
                Lazy<ISitemapGenerator> sitemapGenerator,
                Lazy<CaptchaSettings> captchaSettings,
                Lazy<CommonSettings> commonSettings)
        {
            this._services = services;
            this._articleCategoryService = articleCategoryService;
            this._feedbackService = feedbackService;
            this._webHelper = webHelper;
            this._articleService = articleService;
            this._topicService = topicService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._sitemapGenerator = sitemapGenerator;
            this._captchaSettings = captchaSettings;
            this._commonSettings = commonSettings;

            T = NullLocalizer.Instance;
        }
        #endregion

        public Localizer T { get; set; }
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 首页幻灯片
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ContentSlider()
        {
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var settings = _services.Settings.LoadSetting<ContentSliderSettings>();

            settings.BackgroundPictureUrl = pictureService.GetPictureUrl(settings.BackgroundPictureId, 0, false);

            var slides = settings.Slides
                .Where(s =>
                    s.LanguageCulture == _services.WorkContext.WorkingLanguage.LanguageCulture &&
                    (!s.LimitedToSites || (s.SelectedSiteIds != null && s.SelectedSiteIds.Contains(_services.SiteContext.CurrentSite.Id)))
                )
                .OrderBy(s => s.DisplayOrder);

            foreach (var slide in slides)
            {
                slide.PictureUrl = pictureService.GetPictureUrl(slide.PictureId, 0, false);
                slide.BackgroundPictureUrl = pictureService.GetPictureUrl(slide.BackgroundPictureId, 0, false);
                slide.Button1.Url = CheckButtonUrl(slide.Button1.Url);
                slide.Button2.Url = CheckButtonUrl(slide.Button2.Url);
                slide.Button3.Url = CheckButtonUrl(slide.Button3.Url);
            }

            settings.Slides = slides.ToList();

            return PartialView(settings);
        }

        #region 联系我们
        
        /// <summary>
        /// 联系我们
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult ContactUs()
        {
            var model = new ContactUsModel()
            {
                Email = _services.WorkContext.CurrentUser.Email,
                FullName = _services.WorkContext.CurrentUser.GetFullName(),
                DisplayCaptcha = _captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage
            };

            return View(model);
        }
        /// <summary>
        /// 联系我们留言
        /// </summary>
        /// <param name="model"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [HttpPost, ActionName("ContactUs")]
        [ValidateMvcCaptcha]
        public ActionResult ContactUsSend(ContactUsModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", T("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;
                string subject = T("ContactUs.EmailSubject", _services.SiteContext.CurrentSite.Name);

                var emailAccount = _emailAccountService.Value.GetEmailAccountById(EngineContext.Current.Resolve<EmailAccountSettings>().DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.Value.GetAllEmailAccounts().FirstOrDefault();

                string from = null;
                string fromName = null;
                string body = HtmlUtils.FormatText(model.Enquiry, false, true, false, false, false, false);
                //required for some SMTP servers
                if (_commonSettings.Value.UseSystemEmailForContactUsForm)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                        Server.HtmlEncode(fullName),
                        Server.HtmlEncode(email), body);
                }
                else
                {
                    from = email;
                    fromName = fullName;
                }
                _queuedEmailService.Value.InsertQueuedEmail(new QueuedEmail
                {
                    From = from,
                    FromName = fromName,
                    To = emailAccount.Email,
                    ToName = emailAccount.DisplayName,
                    Priority = 5,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    ReplyTo = email,
                    ReplyToName = fullName
                });

                model.SuccessfullySent = true;
                model.Result = T("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                _services.UserActivity.InsertActivity("PublicSite.ContactUs", T("ActivityLog.PublicSite.ContactUs"));

                return View(model);
            }

            model.DisplayCaptcha = _captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage;
            return View(model);
        }

        #endregion

        #region 留言
        /// <留言>
        /// 联系我们
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Feedback()
        {
            var model = new FeedbackModel()
            {
                DisplayCaptcha = _captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage
            };

            return View(model);
        }
        /// <summary>
        /// 留言
        /// </summary>
        /// <param name="model"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Feedback")]
        [ValidateMvcCaptcha]
        public ActionResult FeedbackSend(FeedbackModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", T("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {

                var feedback = new Feedback()
                {
                    UserEmail = model.UserEmail.Trim(),
                    Title = model.Title,
                    Content = model.Content,
                    UserName = model.UserName.Trim(),
                    AddTime = DateTime.UtcNow,
                    ReplyTime=DateTime .UtcNow,
                    UserQQ = model.UserQQ,
                    UserTel = model.UserTel,
                    IPAddress = _webHelper.Value.GetCurrentIpAddress()
                };
                this._feedbackService.Value.InsertFeedback(feedback);
                //activity log
                _services.UserActivity.InsertActivity("PublicSite.Feedback", T("ActivityLog.PublicSite.Feedback"));

                return View(model);
            }

            model.DisplayCaptcha = _captchaSettings.Value.Enabled && _captchaSettings.Value.ShowOnContactUsPage;
            return View(model);
        }
        #endregion

        /// <summary>
        /// 网站关闭提示
        /// </summary>
        /// <returns></returns>
        public ActionResult SiteClosed()
        {
            return View();
        }
        /// <summary>
        /// 网站地图Seo
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult SitemapSeo()
        {
            if (!_commonSettings.Value.SitemapEnabled)
                return HttpNotFound();

            var roleIds = _services.WorkContext.CurrentUser.UserRoles.Where(x => x.Active).Select(x => x.Id).ToList();
            string cacheKey = ModelCacheEventConsumer.SITEMAP_XML_MODEL_KEY.FormatInvariant(_services.WorkContext.WorkingLanguage.Id, string.Join(",", roleIds), _services.SiteContext.CurrentSite.Id);
            var sitemap = _services.Cache.Get(cacheKey, () =>
            {
                return _sitemapGenerator.Value.Generate(this.Url);
            }, 120);

            return Content(sitemap, "text/xml");
        }
        /// <summary>
        /// 网站地图
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.No)]
        public ActionResult Sitemap()
        {
            if (!_commonSettings.Value.SitemapEnabled)
                return HttpNotFound();

            var roleIds = _services.WorkContext.CurrentUser.UserRoles.Where(x => x.Active).Select(x => x.Id).ToList();
            string cacheKey = ModelCacheEventConsumer.SITEMAP_PAGE_MODEL_KEY.FormatInvariant(_services.WorkContext.WorkingLanguage.Id, string.Join(",", roleIds), _services.SiteContext.CurrentSite.Id);

            var result = _services.Cache.Get(cacheKey, () =>
            {
                var model = new SitemapModel();
                if (_commonSettings.Value.SitemapIncludeCategories)
                {
                    var categories = _articleCategoryService.Value.GetAllCategories();
                    model.ArticleCategories = categories.Select(x => x.ToModel()).ToList();
                }
                if (_commonSettings.Value.SitemapIncludeProducts)
                {
                    //limit articel to 200 until paging is supported on this page
                    var articleSearchContext = new ArticleSearchContext();

                    articleSearchContext.OrderBy = ArticleSortingEnum.Position;
                    articleSearchContext.PageSize = 200;
                    articleSearchContext.SiteId = _services.SiteContext.CurrentSiteIdIfMultiSiteMode;
                    articleSearchContext.VisibleIndividuallyOnly = true;

                    var articels = _articleService.Value.SearchArticles(articleSearchContext);

                    model.Articles = articels.Select(articel => new ArticlePostModel()
                    {
                        Id = articel.Id,
                        Title = articel.GetLocalized(x => x.Title).EmptyNull(),
                        ShortContent = articel.GetLocalized(x => x.ShortContent),
                        FullContent = articel.GetLocalized(x => x.FullContent),
                        SeName = articel.GetSeName(),
                    }).ToList();
                }
                if (_commonSettings.Value.SitemapIncludeTopics)
                {
                    var topics = _topicService.Value.GetAllTopics(_services.SiteContext.CurrentSite.Id)
                         .ToList()
                         .FindAll(t => t.IncludeInSitemap);

                    model.Topics = topics.Select(topic => new TopicModel()
                    {
                        Id = topic.Id,
                        SystemName = topic.SystemName,
                        IncludeInSitemap = topic.IncludeInSitemap,
                        IsPasswordProtected = topic.IsPasswordProtected,
                        Title = topic.GetLocalized(x => x.Title),
                    })
                    .ToList();
                }
                return model;
            });

            return View(result);
        }

        #region helper functions
        /// <summary>
        /// 判断按钮是否URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string CheckButtonUrl(string url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                if (url.StartsWith("//") || url.StartsWith("/") || url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    //  //www.domain.de/dir
                    //  http://www.domain.de/dir
                    // nothing needs to be done
                    return url;
                }
                else if (url.StartsWith("~/"))
                {
                    //  ~/directory
                    return Url.Content(url);
                }
                else
                {
                    //  directory
                    return Url.Content("~/" + url);
                }
            }

            return url.EmptyNull();
        }

        #endregion helper functions
    }
}
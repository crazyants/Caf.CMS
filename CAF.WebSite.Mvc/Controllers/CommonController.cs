using CAF.WebSite.Application.WebUI.Themes;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Collections;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.WebSite.Application.WebUI.UI;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.WebSite.Application.Services.Forums;
using CAF.Infrastructure.Core.Themes;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Topics;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System.Drawing;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.Services.Security;
using System.Text;
using CAF.Infrastructure.Core.Domain.Seo;
using System.Globalization;
using CAF.WebSite.Application.Services.Links;
using CAF.WebSite.Mvc.Models.Media;
using CAF.WebSite.Application.Services.RegionalContents;
using CAF.WebSite.Application.WebUI.Theming;

namespace CAF.WebSite.Mvc.Controllers
{
    public class CommonController : PublicControllerBase
    {
        #region Fields
        private readonly ITopicService _topicService;
        private readonly Lazy<ILanguageService> _languageService;
 
        private readonly Lazy<IVisitRecordService> _visitRecordService;
 
        private readonly Lazy<IThemeRegistry> _themeRegistry;
        private readonly Lazy<IForumService> _forumservice;
        private readonly Lazy<IGenericAttributeService> _genericAttributeService;
        private readonly Lazy<IMobileDeviceHelper> _mobileDeviceHelper;
        private readonly Lazy<ILinkService> _linkService;
        private readonly Lazy<IRegionalContentService> _regionalContentService;
        private readonly static string[] s_hints = new string[] { "CafCms" };

        private readonly IPageAssetsBuilder _pageAssetsBuilder;
        private readonly Lazy<IPictureService> _pictureService;
        private readonly ICommonServices _services;

        private readonly UserSettings _userSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CommonSettings _commonSettings;
        private readonly ThemeSettings _themeSettings;
        private readonly ForumSettings _forumSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly Lazy<SecuritySettings> _securitySettings;
        private readonly MediaSettings _mediaSettings;
        private readonly static object _lock = new object();	// codehint: sm-add
        #endregion

        #region Constructors

        public CommonController(
            ITopicService topicService,
            Lazy<ILanguageService> languageService,
            Lazy<IVisitRecordService> visitRecordService,
            Lazy<IThemeRegistry> themeRegistry,
            Lazy<IForumService> forumService,
            Lazy<IGenericAttributeService> genericAttributeService,
            Lazy<IMobileDeviceHelper> mobileDeviceHelper,
            Lazy<ILinkService> linkService,
            Lazy<IRegionalContentService> regionalContentService,
            ThemeSettings themeSettings,
            IPageAssetsBuilder pageAssetsBuilder,
            Lazy<IPictureService> pictureService,
            ICommonServices services,
            UserSettings userSettings,
            TaxSettings taxSettings,
            MediaSettings mediaSettings,
            EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings,
            ForumSettings forumSettings,
            LocalizationSettings localizationSettings,
            Lazy<SecuritySettings> securitySettings)
        {
            this._topicService = topicService;
            this._languageService = languageService;
            this._visitRecordService = visitRecordService;
            this._themeRegistry = themeRegistry;
            this._forumservice = forumService;
            this._genericAttributeService = genericAttributeService;
            this._mobileDeviceHelper = mobileDeviceHelper;
            this._linkService = linkService;
            this._regionalContentService = regionalContentService;
            this._userSettings = userSettings;
            this._taxSettings = taxSettings;
            this._commonSettings = commonSettings;
            this._forumSettings = forumSettings;
            this._localizationSettings = localizationSettings;
            this._securitySettings = securitySettings;

            this._themeSettings = themeSettings;
            this._pageAssetsBuilder = pageAssetsBuilder;
            this._pictureService = pictureService;
            this._services = services;
            this._mediaSettings = mediaSettings;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        #endregion

        #region Utilities
        [NonAction]
        protected LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _services.Cache.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, _services.SiteContext.CurrentSite.Id), () =>
            {
                var result = _languageService.Value
                    .GetAllLanguages(siteId: _services.SiteContext.CurrentSite.Id)
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NativeName = LocalizationHelper.GetLanguageNativeName(x.LanguageCulture) ?? x.Name,
                        ISOCode = x.LanguageCulture,
                        SeoCode = x.UniqueSeoCode,
                        FlagImageFileName = x.FlagImageFileName
                    })
                    .ToList();
                return result;
            });

            var workingLanguage = _services.WorkContext.WorkingLanguage;

            var model = new LanguageSelectorModel()
            {
                CurrentLanguageId = workingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            string defaultSeoCode = _languageService.Value.GetDefaultLanguageSeoCode();

            foreach (var lang in model.AvailableLanguages)
            {
                var helper = new LocalizedUrlHelper(HttpContext.Request, true);

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    if (lang.SeoCode == defaultSeoCode && (int)(_localizationSettings.DefaultLanguageRedirectBehaviour) > 0)
                    {
                        helper.StripSeoCode();
                    }
                    else
                    {
                        helper.PrependSeoCode(lang.SeoCode, true);
                    }
                }

                model.ReturnUrls[lang.SeoCode] = helper.GetAbsolutePath();
            }

            return model;
        }
        // TODO: (MC) zentral auslagern
        private string GetLanguageNativeName(string locale)
        {
            try
            {
                if (!string.IsNullOrEmpty(locale))
                {
                    var info = CultureInfo.GetCultureInfoByIetfLanguageTag(locale);
                    if (info == null)
                    {
                        return null;
                    }
                    return info.NativeName;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 未读的私人信息
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected int GetUnreadPrivateMessages()
        {
            var result = 0;
            var user = _services.WorkContext.CurrentUser;
            if (_forumSettings.AllowPrivateMessages && !user.IsGuest())
            {
                var privateMessages = _forumservice.Value.GetAllPrivateMessages(_services.SiteContext.CurrentSite.Id, 0, user.Id, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = privateMessages.TotalCount;
                }
            }

            return result;
        }
        protected string GetVisitReferrerType(string url)
        {
            url = url.Trim();
            if ("" == url)
            {
                return "0"; //没有来源
            }
            else if (url.IndexOf("fromsource=") > -1)
            {
                return "1"; //推广链接
            }
            else if (url.IndexOf("baidu.com") > -1)
            {
                return "2"; // 百度搜索引擎
            }
            else if (url.IndexOf("google.com") > -1)
            {
                return "3"; // Google搜索引擎
            }
            else if (url.IndexOf("sogou.com") > -1)
            {
                return "4"; // 搜狗搜索引擎
            }
            else if (url.IndexOf("soso.com") > -1)
            {
                return "5"; // 搜搜搜索引擎
            }
            else
            {
                return "6"; // 其他浏览
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// 头部信息部分页面
        /// <remarks>包括Logo图片 图片大小</remarks>
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Header()
        {
            var model = _services.Cache.Get(ModelCacheEventConsumer.SITEHEADER_MODEL_KEY.FormatWith(_services.SiteContext.CurrentSite.Id), () =>
            {
                var pictureService = _pictureService.Value;
                int logoPictureId = _services.SiteContext.CurrentSite.LogoPictureId;

                Picture picture = null;
                if (logoPictureId > 0)
                {
                    picture = pictureService.GetPictureById(logoPictureId);
                }

                string logoUrl = null;
                var logoSize = new Size();
                if (picture != null)
                {
                    logoUrl = pictureService.GetPictureUrl(picture);
                    logoSize = pictureService.GetPictureSize(picture);
                }

                return new SiteHeaderModel()
                {
                    LogoUploaded = picture != null,
                    LogoUrl = logoUrl,
                    LogoWidth = logoSize.Width,
                    LogoHeight = logoSize.Height,
                    LogoTitle = _services.SiteContext.CurrentSite.Name
                };
            });


            return PartialView(model);
        }
        /// <summary>
        /// 多语言选择部分页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = PrepareLanguageSelectorModel();

            if (model.AvailableLanguages.Count < 2)
                return Content("");

            // register all available languages as <link hreflang="..." ... />
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var host = _services.WebHelper.GetSiteLocation();
                foreach (var lang in model.AvailableLanguages)
                {
                    _pageAssetsBuilder.AddLinkPart("alternate", host + model.ReturnUrls[lang.SeoCode].TrimStart('/'), hreflang: lang.SeoCode);
                }
            }

            return PartialView(model);
        }
        /// <summary>
        /// 选择语言事件
        /// </summary>
        /// <param name="langid"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.Value.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _services.WorkContext.WorkingLanguage = language;
            }

            // url referrer
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = _services.WebHelper.GetUrlReferrer();
            }

            // home page
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.RouteUrl("HomePage");
            }

            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var helper = new LocalizedUrlHelper(HttpContext.Request.ApplicationPath, returnUrl, true);
                helper.PrependSeoCode(_services.WorkContext.WorkingLanguage.UniqueSeoCode, true);
                returnUrl = helper.GetAbsolutePath();
            }

            return Redirect(returnUrl);
        }
        /// <summary>
        /// 网站Barnner部分也面
        /// <remarks></remarks>
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult SiteBar()
        {
            var user = _services.WorkContext.CurrentUser;
            var pictureService = _pictureService.Value;
            var unreadMessageCount = GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = T("PrivateMessages.TotalUnread");

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !user.GetAttribute<bool>(SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, _services.SiteContext.CurrentSite.Id))
                {
                    _genericAttributeService.Value.SaveAttribute(user, SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, true, _services.SiteContext.CurrentSite.Id);
                    alertMessage = T("PrivateMessages.YouHaveUnreadPM", unreadMessageCount);
                }
            }

            var model = new SiteBarModel
            {
                IsAuthenticated = user.IsRegistered(),
                UserEmailUserName = user.IsRegistered() ? (_userSettings.UserNamesEnabled ? user.UserName : user.Email) : "",
                IsUserImpersonated = _services.WorkContext.OriginalUserIfImpersonated != null,
                DisplayAdminLink = _services.Permissions.Authorize(StandardPermissionProvider.AccessAdminPanel),
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
                UserAvatar = pictureService.GetPictureUrl(
               user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
               _mediaSettings.AvatarPictureSize,false)

            };

            return PartialView(model);
        }
        /// <summary>
        /// 会员中心下拉部分也面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult AccountDropdown()
        {
            var user = _services.WorkContext.CurrentUser;

            var unreadMessageCount = GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = unreadMessageCount.ToString();

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !user.GetAttribute<bool>(SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, _services.SiteContext.CurrentSite.Id))
                {
                    _genericAttributeService.Value.SaveAttribute(user, SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, true, _services.SiteContext.CurrentSite.Id);
                    alertMessage = T("PrivateMessages.YouHaveUnreadPM", unreadMessageCount);
                }
            }

            var model = new AccountDropdownModel
            {
                IsAuthenticated = user.IsRegistered(),
                DisplayAdminLink = _services.Permissions.Authorize(StandardPermissionProvider.AccessAdminPanel),
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage
            };

            return PartialView(model);
        }
        /// <summary>
        /// 信息块
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult InfoBlock()
        {
            var model = new InfoBlockModel
            {
                SitemapEnabled = _commonSettings.SitemapEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
            };

            return PartialView(model);
        }
        /// <summary>
        /// 友情链接
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult LinkBlock()
        {
            var links = _linkService.Value.GetAllLinks(_services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id, 0, 20);
            string cacheKey = ModelCacheEventConsumer.LINK_MODEL_KEY.FormatInvariant(_services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id);

            var result = _services.Cache.Get(cacheKey, () =>
            {
                var model = new LinkModel();

                foreach (var item in links)
                {
                    var linkBlck = new LinkBlockModel();
                    linkBlck.Name = item.Name;
                    linkBlck.Intro = item.Intro;
                    linkBlck.IsHome = item.IsHome;
                    linkBlck.SortId = item.SortId;
                    linkBlck.LogoUrl = item.LogoUrl;
                    linkBlck.LinkUrl = item.LinkUrl;
                    if (item.PictureId.HasValue)
                    {
                        linkBlck.DefaultPictureModel = new PictureModel()
                        {
                            PictureId = item.PictureId.GetValueOrDefault(),
                            FullSizeImageUrl = _pictureService.Value.GetPictureUrl(item.PictureId.GetValueOrDefault()),
                            ImageUrl = _pictureService.Value.GetPictureUrl(item.PictureId.GetValueOrDefault(), _mediaSettings.DetailsPictureSize),
                        };
                    }
                    model.LinkBlocks.Add(linkBlck);
                }
                return model;
            });

            return PartialView(result);

        }
        /// <summary>
        /// 图标图标
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByCustom = "Theme_Site")]
        public ActionResult Favicon()
        {
            var icons = new string[] 
            { 
                "favicon-{0}.ico".FormatInvariant(_services.SiteContext.CurrentSite.Id), 
                "favicon.ico" 
            };

            string virtualPath = null;

            foreach (var icon in icons)
            {
                virtualPath = Url.ThemeAwareContent(icon);
                if (virtualPath.HasValue())
                {
                    break;
                }
            }

            if (virtualPath.IsEmpty())
            {
                return Content("");
            }

            var model = new FaviconModel()
            {
                Uploaded = true,
                FaviconUrl = virtualPath
            };

            return PartialView(model);
        }
        /// <summary>
        /// 底部部分页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Footer()
        {
            string taxInfo = (_services.WorkContext.GetTaxDisplayTypeFor(_services.WorkContext.CurrentUser, _services.SiteContext.CurrentSite.Id) == TaxDisplayType.IncludingTax)
                ? T("Tax.InclVAT")
                : T("Tax.ExclVAT");

            string shippingInfoLink = Url.RouteUrl("Topic", new { SystemName = "shippinginfo" });
            var site = _services.SiteContext.CurrentSite;

            var availableSiteThemes = !_themeSettings.AllowUserToSelectTheme ? new List<SiteThemeModel>() : _themeRegistry.Value.GetThemeManifests()
                .Where(x => !x.MobileTheme)
                .Select(x =>
                {
                    return new SiteThemeModel()
                    {
                        Name = x.ThemeName,
                        Title = x.ThemeTitle
                    };
                })
                .ToList();

            var model = new FooterModel
            {
                SiteName = site.Name,
                LegalInfo = T("Tax.LegalInfoFooter", taxInfo, shippingInfoLink),
                ShowLegalInfo = _taxSettings.ShowLegalHintsInFooter,
                ShowThemeSelector = availableSiteThemes.Count > 1,
                ForumEnabled = _forumSettings.ForumsEnabled,
                HideNewsletterBlock = _userSettings.HideNewsletterBlock,
            };

            var hint = _services.Settings.GetSettingByKey<string>("Rnd_CafCopyrightHint", string.Empty, site.Id);
            if (hint.IsEmpty())
            {
                hint = s_hints[new Random().Next(s_hints.Length)];
                _services.Settings.SetSetting<string>("Rnd_CafCopyrightHint", hint, site.Id);
            }

            var topics = new string[] { "paymentinfo", "imprint", "disclaimer" };
            foreach (var t in topics)
            {
                //load by site
                var topic = _topicService.GetTopicBySystemName(t, site.Id);
                if (topic == null)
                    //not found. let's find topic assigned to all sites
                    topic = _topicService.GetTopicBySystemName(t, 0);

                if (topic != null)
                {
                    model.Topics.Add(t, topic.Title);
                }
            }

            var socialSettings = EngineContext.Current.Resolve<SocialSettings>();

            model.ShowSocialLinks = socialSettings.ShowSocialLinksInFooter;
            model.FacebookLink = socialSettings.FacebookLink;
            model.GooglePlusLink = socialSettings.GooglePlusLink;
            model.TwitterLink = socialSettings.TwitterLink;
            model.PinterestLink = socialSettings.PinterestLink;
            model.YoutubeLink = socialSettings.YoutubeLink;
            model.CafSiteHint = "<a href='http://www.fengkuangmayi.net/' class='sm-hint' target='_blank'><strong>{0}</strong></a> by CafSite AG &copy; {1}".FormatCurrent(hint, DateTime.Now.Year);

            return PartialView(model);
        }
        /// <summary>
        /// (桌面或移动版)部分页面
        /// </summary>
        /// <param name="dontUseMobileVersion">True - 使用桌面版; false - 使用版本为移动设备</param>
        /// <returns>Action result</returns>
        public ActionResult ChangeDevice(bool dontUseMobileVersion)
        {
            _genericAttributeService.Value.SaveAttribute(_services.WorkContext.CurrentUser,
                SystemUserAttributeNames.DontUseMobileVersion, dontUseMobileVersion, _services.SiteContext.CurrentSite.Id);

            string returnurl = _services.WebHelper.GetUrlReferrer();
            if (String.IsNullOrEmpty(returnurl))
                returnurl = Url.RouteUrl("HomePage");
            return Redirect(returnurl);
        }
        /// <summary>
        /// 页面脚本块
        /// </summary>
        /// <param name="systemName"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RegionalContentBlock(string systemName)
        {
            string cacheKey = ModelCacheEventConsumer.REGIONALCONTENT_MODEL_KEY.FormatInvariant(systemName, _services.WorkContext.WorkingLanguage.Id, _services.SiteContext.CurrentSite.Id);

            var result = _services.Cache.Get(cacheKey, () =>
            {
                var regionalContent = _regionalContentService.Value.GetRegionalContentBySystemName(systemName, _services.SiteContext.CurrentSite.Id, _services.WorkContext.WorkingLanguage.Id);
                return regionalContent;
            });

            if (result == null)
                return Content("");

            return Content(result.Body);
        }

        /// <summary>
        /// (桌面或移动版)切换事件
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ChangeDeviceBlock()
        {
            if (!_mobileDeviceHelper.Value.MobileDevicesSupported())
                //mobile devices support is disabled
                return Content("");

            if (!_mobileDeviceHelper.Value.IsMobileDevice())
                //request is made by a desktop computer
                return Content("");

            return View();
        }

        public ActionResult RobotsTextFile()
        {
            var disallowPaths = new List<string>()
            {
                "/bin/",
                "/Content/files/",
                "/Content/files/ExportImport/",
				"/Exchange/",
                "/Country/GetStatesByCountryId",
                "/Install",
                "/Article/SetReviewHelpfulness",
            };
            var localizableDisallowPaths = new List<string>()
            {
                
                "/Member/Avatar",
                "/Member/Activation",
                "/Member/Addresses",
                "/Member/BackInStockSubscriptions",
                "/Member/ChangePassword",
                "/Member/CheckUsernameAvailability",
                "/Member/DownloadableProducts",
                "/Member/ForumSubscriptions",
				"/Member/DeleteForumSubscriptions",
                "/Member/Info",
                "/Member/Orders",
                "/Member/ReturnRequests",
                "/Member/RewardPoints",
                "/PrivateMessages",
                "/PasswordRecovery",
                "/Poll/Vote",
                "/Topic/Authenticate",
                "/Article/AskQuestion",
                "/Article/EmailAFriend",
				"/Search",
				"/Config",
				"/Settings"
            };


            const string newLine = "\r\n"; //Environment.NewLine
            var sb = new StringBuilder();
            sb.Append("User-agent: *");
            sb.Append(newLine);
            sb.AppendFormat("Sitemap: {0}", Url.RouteUrl("SitemapSEO", (object)null, _securitySettings.Value.ForceSslForAllPages ? "https" : "http"));
            sb.AppendLine();

            var disallows = disallowPaths.Concat(localizableDisallowPaths);

            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                // URLs are localizable. Append SEO code
                foreach (var language in _languageService.Value.GetAllLanguages(siteId: _services.SiteContext.CurrentSite.Id))
                {
                    disallows = disallows.Concat(localizableDisallowPaths.Select(x => "/{0}{1}".FormatInvariant(language.UniqueSeoCode, x)));
                }
            }

            var seoSettings = EngineContext.Current.Resolve<SeoSettings>();

            // append extra disallows
            disallows = disallows.Concat(seoSettings.ExtraRobotsDisallows.Select(x => x.Trim()));

            // Append all lowercase variants (at least Google is case sensitive)
            disallows = disallows.Concat(GetLowerCaseVariants(disallows));

            foreach (var disallow in disallows)
            {
                sb.AppendFormat("Disallow: {0}", disallow);
                sb.Append(newLine);
            }

            Response.ContentType = "text/plain";
            Response.Write(sb.ToString());
            return null;
        }

        private IEnumerable<string> GetLowerCaseVariants(IEnumerable<string> disallows)
        {
            var other = new List<string>();
            foreach (var item in disallows)
            {
                var lower = item.ToLower();
                if (lower != item)
                {
                    other.Add(lower);
                }
            }

            return other;
        }

        /// <summary>
        /// JavaScript脚本开启警告
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }
        /// <summary>
        /// 错误页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GenericUrl()
        {
            // seems that no entity was found
            return HttpNotFound();
        }
        /// <summary>
        /// 网站访问统计
        /// </summary>
        /// <returns></returns>
        public ActionResult WebVisit()
        {
            VisitRecord visitModel = new VisitRecord();
            visitModel.VisitReffer = Request.QueryString["oldlink"].ToString();
            visitModel.VisitRefferType = int.Parse(GetVisitReferrerType(Request.QueryString["oldlink"].ToString()));
            visitModel.VisitResolution = Request.QueryString["s"].ToString();
            visitModel.VisitURL = Request.QueryString["id"].ToString();
            visitModel.VisitTimeIn = System.DateTime.Now;
            visitModel.VisitIP = _services.WebHelper.GetCurrentIpAddress();
            visitModel.VisitOS = Request.QueryString["sys"].ToString();
            visitModel.VisitTitle = Request.QueryString["title"].ToString();
            visitModel.VisitBrowerType = Request.QueryString["b"].ToString();
            visitModel.VisitRefferKeyWork = "";
            visitModel.VisitProvince = Request.QueryString["p"].ToString();
            visitModel.VisitCity = Request.QueryString["c"].ToString();


            // 获取source后面的内容]
            string url = Request.QueryString["id"].ToString();
            if (url.IndexOf("fromsource=") > 0)
            {
                int startindex = url.IndexOf("fromsource=");
                int endindex = url.Length - startindex - 11;
                string id = url.Substring(startindex + 11, endindex);
                visitModel.FromSource = id;
            }
            // 这里执行添加到数据库的操作并返回添加以后ID信息
            this._visitRecordService.Value.InsertVisitRecord(visitModel);
            int newID = visitModel.Id;

            return Json(newID.ToString(), JsonRequestBehavior.AllowGet);

        }

        #endregion
    }
}
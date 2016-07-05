using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Helpers;
using CAF.Infrastructure.Core.Themes;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Common;
using CAF.Infrastructure.Core.Settings;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Mvc.Admin.Models.Settings;
using CAF.Infrastructure.Core.Domain;
using CAF.Infrastructure.Core.Domain.Seo;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Exceptions;
//using CAF.WebSite.Application.Services.Tax;
//using CAF.WebSite.Application.Services.Orders;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class SettingController : AdminControllerBase
    {
        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEncryptionService _encryptionService;
        private readonly IThemeRegistry _themeRegistry;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;
        private readonly IFulltextService _fulltextService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly ISiteService _siteService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IDeliveryTimeService _deliveryTimesService;

        private SiteDependingSettingHelper _siteDependingSettings;

        #region Constructors
        public SettingController(ISettingService settingService,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IAddressService addressService, IPictureService pictureService,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IEncryptionService encryptionService,
            IThemeRegistry themeRegistry, IUserService userService,
            IUserActivityService userActivityService, IPermissionService permissionService,
            IWebHelper webHelper,
            IFulltextService fulltextService,
            IMaintenanceService maintenanceService,
            ISiteService siteService,
            IWorkContext workContext, IGenericAttributeService genericAttributeService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService,
            IDeliveryTimeService deliveryTimesService)
        {
            this._settingService = settingService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._encryptionService = encryptionService;
            this._themeRegistry = themeRegistry;
            this._userService = userService;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._webHelper = webHelper;
            this._fulltextService = fulltextService;
            this._maintenanceService = maintenanceService;
            this._siteService = siteService;
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._deliveryTimesService = deliveryTimesService;
        }

        #endregion


        private SiteDependingSettingHelper SiteDependingSettings
        {
            get
            {
                if (_siteDependingSettings == null)
                    _siteDependingSettings = new SiteDependingSettingHelper(this.ViewData);
                return _siteDependingSettings;
            }
        }

        #region Media

        public ActionResult Media()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen site scope
            var siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(siteScope);
            var model = mediaSettings.ToModel();

            SiteDependingSettings.GetOverrideKeys(mediaSettings, model, siteScope, _settingService);

            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;

            var resKey = "Admin.Configuration.Settings.Media.PictureZoomType.";

            #region AvailablePictureZoomTypes


            model.AvailablePictureZoomTypes.Add(new SelectListItem
            {
                Text = _localizationService.GetResource(resKey + "Window"),
                Value = "window",
                Selected = model.PictureZoomType.Equals("window")
            });
            model.AvailablePictureZoomTypes.Add(new SelectListItem
            {
                Text = _localizationService.GetResource(resKey + "Inner"),
                Value = "inner",
                Selected = model.PictureZoomType.Equals("inner")
            });
            model.AvailablePictureZoomTypes.Add(new SelectListItem
            {
                Text = _localizationService.GetResource(resKey + "Lens"),
                Value = "lens",
                Selected = model.PictureZoomType.Equals("lens")
            });
            #endregion

            #region AvailableWatermarkTypes
         //   model.AvailableWatermarkTypes.Add(new SelectListItem
         //{
         //    Text = "关闭水印",
         //    Value = "0",
         //    Selected = model.WatermarkType.Equals(0)
         //});
         //   model.AvailableWatermarkTypes.Add(new SelectListItem
         //   {
         //       Text = "文字水印",
         //       Value = "1",
         //       Selected = model.WatermarkType.Equals(1)
         //   });
         //   model.AvailableWatermarkTypes.Add(new SelectListItem
         //   {
         //       Text = "图片水印",
         //       Value = "2",
         //       Selected = model.WatermarkType.Equals(2)
         //   });
            #endregion

            #region AvailableWatermarkPositions
            //model.AvailableWatermarkPositions.Add(new SelectListItem
            //{
            //    Text = "左上",
            //    Value = "1",
            //    Selected = model.WatermarkPosition.Equals(0)
            //});
            //model.AvailableWatermarkTypes.Add(new SelectListItem
            //{
            //    Text = "中上",
            //    Value = "1",
            //    Selected = model.WatermarkType.Equals(1)
            //});
            //model.AvailableWatermarkTypes.Add(new SelectListItem
            //{
            //    Text = "图片水印",
            //    Value = "2",
            //    Selected = model.WatermarkType.Equals(2)
            //});
            #endregion

            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Media(MediaSettingsModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen site scope
            var siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(siteScope);
            mediaSettings = model.ToEntity(mediaSettings);

            SiteDependingSettings.UpdateSettings(mediaSettings, form, siteScope, _settingService);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }
        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public ActionResult ChangePictureStorage()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }


        [ChildActionOnly]
        public ActionResult SiteScopeConfiguration()
        {
            var allSites = _siteService.GetAllSites();
            if (allSites.Count < 2)
                return Content("");

            var model = new SiteScopeConfigurationModel()
            {
                SiteId = this.GetActiveSiteScopeConfiguration(_siteService, _workContext)
            };

            foreach (var site in allSites)
            {
                model.AllSites.Add(new SelectListItem()
                {
                    Text = site.Name,
                    Selected = (site.Id == model.SiteId),
                    Value = Url.Action("ChangeSiteScopeConfiguration", "Setting", new { siteid = site.Id, returnUrl = Request.RawUrl })
                });
            }

            model.AllSites.Insert(0, new SelectListItem()
            {
                Text = _localizationService.GetResource("Admin.Common.SitesAll"),
                Selected = (0 == model.SiteId),
                Value = Url.Action("ChangeSiteScopeConfiguration", "Setting", new { siteid = 0, returnUrl = Request.RawUrl })
            });

            return PartialView(model);
        }

        public ActionResult ChangeSiteScopeConfiguration(int siteid, string returnUrl = "")
        {
            var site = _siteService.GetSiteById(siteid);
            if (site != null || siteid == 0)
            {
                _genericAttributeService.SaveAttribute(_workContext.CurrentUser,
                    SystemUserAttributeNames.AdminAreaSiteScopeConfiguration, siteid);
            }

            //url referrer
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = _webHelper.GetUrlReferrer();

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = "Admin" });

            return Redirect(returnUrl);
        }
        #endregion


        public ActionResult GeneralUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            SiteDependingSettings.CreateViewDataObject(siteScope);

            var userSettings = _settingService.LoadSetting<UserSettings>(siteScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(siteScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(siteScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(siteScope);

            //merge settings
            var model = new GeneralUserSettingsModel();
            model.UserSettings = userSettings.ToModel();

            SiteDependingSettings.GetOverrideKeys(userSettings, model.UserSettings, siteScope, _settingService, false);

            model.AddressSettings = addressSettings.ToModel();

            SiteDependingSettings.GetOverrideKeys(addressSettings, model.AddressSettings, siteScope, _settingService, false);

            model.DateTimeSettings.AllowUsersToSetTimeZone = dateTimeSettings.AllowUsersToSetTimeZone;
            model.DateTimeSettings.DefaultSiteTimeZoneId = _dateTimeHelper.DefaultSiteTimeZone.Id;
            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem()
                {
                    Text = timeZone.DisplayName,
                    Value = timeZone.Id,
                    Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultSiteTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                });
            }

            SiteDependingSettings.GetOverrideKeys(dateTimeSettings, model.DateTimeSettings, siteScope, _settingService, false);

            model.ExternalAuthenticationSettings.AutoRegisterEnabled = externalAuthenticationSettings.AutoRegisterEnabled;

            SiteDependingSettings.GetOverrideKeys(externalAuthenticationSettings, model.ExternalAuthenticationSettings, siteScope, _settingService, false);

            return View(model);
        }
        [HttpPost]
        public ActionResult GeneralUser(GeneralUserSettingsModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);

            var userSettings = _settingService.LoadSetting<UserSettings>(siteScope);
            userSettings = model.UserSettings.ToEntity(userSettings);

            SiteDependingSettings.UpdateSettings(userSettings, form, siteScope, _settingService);

            _settingService.SaveSetting(userSettings, x => x.DefaultPasswordFormat, 0, false);

            var addressSettings = _settingService.LoadSetting<AddressSettings>(siteScope);
            addressSettings = model.AddressSettings.ToEntity(addressSettings);

            SiteDependingSettings.UpdateSettings(addressSettings, form, siteScope, _settingService);

            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(siteScope);
            dateTimeSettings.DefaultSiteTimeZoneId = model.DateTimeSettings.DefaultSiteTimeZoneId;
            dateTimeSettings.AllowUsersToSetTimeZone = model.DateTimeSettings.AllowUsersToSetTimeZone;

            SiteDependingSettings.UpdateSettings(dateTimeSettings, form, siteScope, _settingService);

            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(siteScope);
            externalAuthenticationSettings.AutoRegisterEnabled = model.ExternalAuthenticationSettings.AutoRegisterEnabled;

            SiteDependingSettings.UpdateSettings(externalAuthenticationSettings, form, siteScope, _settingService);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("GeneralUser");
        }


        public ActionResult Forum()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
            var model = forumSettings.ToModel();

            SiteDependingSettings.GetOverrideKeys(forumSettings, model, storeScope, _settingService);

            model.ForumEditorValues = forumSettings.ForumEditor.ToSelectList();

            return View(model);
        }
        [HttpPost]
        public ActionResult Forum(ForumSettingsModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
            forumSettings = model.ToEntity(forumSettings);

            SiteDependingSettings.UpdateSettings(forumSettings, form, storeScope, _settingService);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Forum");
        }
        #region Article
 
        //public ActionResult Article()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
        //        return AccessDeniedView();

        //    //load settings for a chosen store scope
        //    var storeScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
        //    var blogSettings = _settingService.LoadSetting<ArticleSettings>(storeScope);
        //    var model = blogSettings.ToModel();

        //    SiteDependingSettings.GetOverrideKeys(blogSettings, model, storeScope, _settingService);

        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult Article(ArticleSettingsModel model, FormCollection form)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
        //        return AccessDeniedView();

        //    //load settings for a chosen store scope
        //    var storeScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
        //    var blogSettings = _settingService.LoadSetting<ArticleSettings>(storeScope);
        //    blogSettings = model.ToEntity(blogSettings);

        //    SiteDependingSettings.UpdateSettings(blogSettings, form, storeScope, _settingService);

        //    //now clear settings cache
        //    _settingService.ClearCache();

        //    //activity log
        //    _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

        //    NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
        //    return RedirectToAction("Article");
        //}


        #endregion

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult GeneralCommon(GeneralCommonSettingsModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen site scope
            var siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);

            //site information
            var siteInformationSettings = _settingService.LoadSetting<SiteInformationSettings>(siteScope);
            siteInformationSettings.SiteClosed = model.SiteInformationSettings.SiteClosed;
            siteInformationSettings.SiteClosedAllowForAdmins = model.SiteInformationSettings.SiteClosedAllowForAdmins;
            siteInformationSettings.SiteContentShare = model.SiteInformationSettings.SiteContentShare; 
            SiteDependingSettings.UpdateSettings(siteInformationSettings, form, siteScope, _settingService);

            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(siteScope);
            seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            seoSettings.PageTitleSeoAdjustment = model.SeoSettings.PageTitleSeoAdjustment;
            seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
            seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
            seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
            seoSettings.CanonicalHostNameRule = model.SeoSettings.CanonicalHostNameRule;

            SiteDependingSettings.UpdateSettings(seoSettings, form, siteScope, _settingService);

            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(siteScope);
            if (securitySettings.AdminAreaAllowedIpAddresses == null)
                securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
            securitySettings.AdminAreaAllowedIpAddresses.Clear();
            if (model.SecuritySettings.AdminAreaAllowedIpAddresses.HasValue())
            {
                foreach (string s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!String.IsNullOrWhiteSpace(s))
                        securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
                }
            }
            securitySettings.HideAdminMenuItemsBasedOnPermissions = model.SecuritySettings.HideAdminMenuItemsBasedOnPermissions;
            _settingService.SaveSetting(securitySettings);

            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(siteScope);
            captchaSettings.Enabled = model.CaptchaSettings.Enabled;
            captchaSettings.ShowOnLoginPage = model.CaptchaSettings.ShowOnLoginPage;
            captchaSettings.ShowOnRegistrationPage = model.CaptchaSettings.ShowOnRegistrationPage;
            captchaSettings.ShowOnContactUsPage = model.CaptchaSettings.ShowOnContactUsPage;
            captchaSettings.ShowOnEmailWishlistToFriendPage = model.CaptchaSettings.ShowOnEmailWishlistToFriendPage;
            captchaSettings.ShowOnEmailProductToFriendPage = model.CaptchaSettings.ShowOnEmailProductToFriendPage;
            captchaSettings.ShowOnAskQuestionPage = model.CaptchaSettings.ShowOnAskQuestionPage;
            captchaSettings.ShowOnArticleCommentPage = model.CaptchaSettings.ShowOnArticleCommentPage;
            captchaSettings.ReCaptchaPublicKey = model.CaptchaSettings.ReCaptchaPublicKey;
            captchaSettings.ReCaptchaPrivateKey = model.CaptchaSettings.ReCaptchaPrivateKey;

            SiteDependingSettings.UpdateSettings(captchaSettings, form, siteScope, _settingService);

            //if (captchaSettings.Enabled && (String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
            //{
            //    NotifyError(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabledNoKeys"));
            //}

            //PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>(siteScope);
            pdfSettings.Enabled = model.PdfSettings.Enabled;
            pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
            pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;

            SiteDependingSettings.UpdateSettings(pdfSettings, form, siteScope, _settingService);

            //localization settings
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>(siteScope);
            localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
            localizationSettings.DefaultLanguageRedirectBehaviour = model.LocalizationSettings.DefaultLanguageRedirectBehaviour;
            localizationSettings.InvalidLanguageRedirectBehaviour = model.LocalizationSettings.InvalidLanguageRedirectBehaviour;
            localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
            localizationSettings.DetectBrowserUserLanguage = model.LocalizationSettings.DetectBrowserUserLanguage;

            SiteDependingSettings.UpdateSettings(localizationSettings, form, siteScope, _settingService);

            _settingService.SaveSetting(localizationSettings, x => x.LoadAllLocaleRecordsOnStartup, 0, false);
            _settingService.SaveSetting(localizationSettings, x => x.DefaultLanguageRedirectBehaviour, 0, false);
            _settingService.SaveSetting(localizationSettings, x => x.InvalidLanguageRedirectBehaviour, 0, false);

            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
                _settingService.SaveSetting(localizationSettings, x => x.SeoFriendlyUrlsForLanguagesEnabled, 0, false);

                System.Web.Routing.RouteTable.Routes.ClearSeoFriendlyUrlsCachedValueForRoutes();	// clear cached values of routes
            }

            //full-text
            var commonSettings = _settingService.LoadSetting<CommonSettings>(siteScope);
            commonSettings.FullTextMode = model.FullTextSettings.SearchMode;

            _settingService.SaveSetting(commonSettings);

            //company information
            var companySettings = _settingService.LoadSetting<CompanyInformationSettings>(siteScope);
            companySettings.CompanyName = model.CompanyInformationSettings.CompanyName;
            companySettings.Salutation = model.CompanyInformationSettings.Salutation;
            companySettings.Title = model.CompanyInformationSettings.Title;
            companySettings.Firstname = model.CompanyInformationSettings.Firstname;
            companySettings.Lastname = model.CompanyInformationSettings.Lastname;
            companySettings.CompanyManagementDescription = model.CompanyInformationSettings.CompanyManagementDescription;
            companySettings.CompanyManagement = model.CompanyInformationSettings.CompanyManagement;
            companySettings.Street = model.CompanyInformationSettings.Street;
            companySettings.Street2 = model.CompanyInformationSettings.Street2;
            companySettings.ZipCode = model.CompanyInformationSettings.ZipCode;
            companySettings.City = model.CompanyInformationSettings.City;
            companySettings.CountryId = model.CompanyInformationSettings.CountryId;
            companySettings.Region = model.CompanyInformationSettings.Region;
            if (model.CompanyInformationSettings.CountryId != 0)
            {
                companySettings.CountryName = _countryService.GetCountryById(model.CompanyInformationSettings.CountryId).Name;
            }
            companySettings.VatId = model.CompanyInformationSettings.VatId;
            companySettings.CommercialRegister = model.CompanyInformationSettings.CommercialRegister;
            companySettings.TaxNumber = model.CompanyInformationSettings.TaxNumber;

            SiteDependingSettings.UpdateSettings(companySettings, form, siteScope, _settingService);

            //contact data
            var contactDataSettings = _settingService.LoadSetting<ContactDataSettings>(siteScope);
            contactDataSettings.CompanyTelephoneNumber = model.ContactDataSettings.CompanyTelephoneNumber;
            contactDataSettings.HotlineTelephoneNumber = model.ContactDataSettings.HotlineTelephoneNumber;
            contactDataSettings.MobileTelephoneNumber = model.ContactDataSettings.MobileTelephoneNumber;
            contactDataSettings.CompanyFaxNumber = model.ContactDataSettings.CompanyFaxNumber;
            contactDataSettings.CompanyEmailAddress = model.ContactDataSettings.CompanyEmailAddress;
            contactDataSettings.WebmasterEmailAddress = model.ContactDataSettings.WebmasterEmailAddress;
            contactDataSettings.SupportEmailAddress = model.ContactDataSettings.SupportEmailAddress;
            contactDataSettings.ContactEmailAddress = model.ContactDataSettings.ContactEmailAddress;

            SiteDependingSettings.UpdateSettings(contactDataSettings, form, siteScope, _settingService);

            //bank connection
            var bankConnectionSettings = _settingService.LoadSetting<BankConnectionSettings>(siteScope);
            bankConnectionSettings.Bankname = model.BankConnectionSettings.Bankname;
            bankConnectionSettings.Bankcode = model.BankConnectionSettings.Bankcode;
            bankConnectionSettings.AccountNumber = model.BankConnectionSettings.AccountNumber;
            bankConnectionSettings.AccountHolder = model.BankConnectionSettings.AccountHolder;
            bankConnectionSettings.Iban = model.BankConnectionSettings.Iban;
            bankConnectionSettings.Bic = model.BankConnectionSettings.Bic;

            SiteDependingSettings.UpdateSettings(bankConnectionSettings, form, siteScope, _settingService);

            //social
            var socialSettings = _settingService.LoadSetting<SocialSettings>(siteScope);
            socialSettings.ShowSocialLinksInFooter = model.SocialSettings.ShowSocialLinksInFooter;
            socialSettings.FacebookLink = model.SocialSettings.FacebookLink;
            socialSettings.GooglePlusLink = model.SocialSettings.GooglePlusLink;
            socialSettings.TwitterLink = model.SocialSettings.TwitterLink;
            socialSettings.PinterestLink = model.SocialSettings.PinterestLink;
            socialSettings.YoutubeLink = model.SocialSettings.YoutubeLink;

            SiteDependingSettings.UpdateSettings(socialSettings, form, siteScope, _settingService);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("GeneralCommon");
        }


        public ActionResult GeneralCommon()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new GeneralCommonSettingsModel();
            var siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);

            SiteDependingSettings.CreateViewDataObject(siteScope);

            //site information
            var siteInformationSettings = _settingService.LoadSetting<SiteInformationSettings>(siteScope);
            model.SiteInformationSettings.SiteClosed = siteInformationSettings.SiteClosed;
            model.SiteInformationSettings.SiteClosedAllowForAdmins = siteInformationSettings.SiteClosedAllowForAdmins;
            model.SiteInformationSettings.SiteContentShare = siteInformationSettings.SiteContentShare;
            SiteDependingSettings.GetOverrideKeys(siteInformationSettings, model.SiteInformationSettings, siteScope, _settingService, false);

            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(siteScope);
            model.SeoSettings.PageTitleSeoAdjustment = seoSettings.PageTitleSeoAdjustment;
            model.SeoSettings.PageTitleSeparator = seoSettings.PageTitleSeparator;
            model.SeoSettings.DefaultTitle = seoSettings.DefaultTitle;
            model.SeoSettings.DefaultMetaKeywords = seoSettings.DefaultMetaKeywords;
            model.SeoSettings.DefaultMetaDescription = seoSettings.DefaultMetaDescription;
            model.SeoSettings.ConvertNonWesternChars = seoSettings.ConvertNonWesternChars;
            model.SeoSettings.CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled;
            model.SeoSettings.CanonicalHostNameRule = seoSettings.CanonicalHostNameRule;

            SiteDependingSettings.GetOverrideKeys(seoSettings, model.SeoSettings, siteScope, _settingService, false);

            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(siteScope);
            model.SecuritySettings.EncryptionKey = securitySettings.EncryptionKey;
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
            {
                for (int i = 0; i < securitySettings.AdminAreaAllowedIpAddresses.Count; i++)
                {
                    model.SecuritySettings.AdminAreaAllowedIpAddresses += securitySettings.AdminAreaAllowedIpAddresses[i];
                    if (i != securitySettings.AdminAreaAllowedIpAddresses.Count - 1)
                        model.SecuritySettings.AdminAreaAllowedIpAddresses += ",";
                }
            }
            model.SecuritySettings.HideAdminMenuItemsBasedOnPermissions = securitySettings.HideAdminMenuItemsBasedOnPermissions;

            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(siteScope);
            model.CaptchaSettings.Enabled = captchaSettings.Enabled;
            model.CaptchaSettings.ShowOnLoginPage = captchaSettings.ShowOnLoginPage;
            model.CaptchaSettings.ShowOnRegistrationPage = captchaSettings.ShowOnRegistrationPage;
            model.CaptchaSettings.ShowOnContactUsPage = captchaSettings.ShowOnContactUsPage;
            model.CaptchaSettings.ShowOnEmailWishlistToFriendPage = captchaSettings.ShowOnEmailWishlistToFriendPage;
            model.CaptchaSettings.ShowOnEmailProductToFriendPage = captchaSettings.ShowOnEmailProductToFriendPage;
            model.CaptchaSettings.ShowOnAskQuestionPage = captchaSettings.ShowOnAskQuestionPage;
            model.CaptchaSettings.ShowOnArticleCommentPage = captchaSettings.ShowOnArticleCommentPage;
            model.CaptchaSettings.ReCaptchaPublicKey = captchaSettings.ReCaptchaPublicKey;
            model.CaptchaSettings.ReCaptchaPrivateKey = captchaSettings.ReCaptchaPrivateKey;

            SiteDependingSettings.GetOverrideKeys(captchaSettings, model.CaptchaSettings, siteScope, _settingService, false);

            //PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>(siteScope);
            model.PdfSettings.Enabled = pdfSettings.Enabled;
            model.PdfSettings.LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled;
            model.PdfSettings.LogoPictureId = pdfSettings.LogoPictureId;

            SiteDependingSettings.GetOverrideKeys(pdfSettings, model.PdfSettings, siteScope, _settingService, false);

            //localization
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>(siteScope);
            model.LocalizationSettings.UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection;
            model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
            model.LocalizationSettings.LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup;
            model.LocalizationSettings.DefaultLanguageRedirectBehaviour = localizationSettings.DefaultLanguageRedirectBehaviour;
            model.LocalizationSettings.InvalidLanguageRedirectBehaviour = localizationSettings.InvalidLanguageRedirectBehaviour;
            model.LocalizationSettings.DetectBrowserUserLanguage = localizationSettings.DetectBrowserUserLanguage;

            SiteDependingSettings.GetOverrideKeys(localizationSettings, model.LocalizationSettings, siteScope, _settingService, false);

            //full-text support
            var commonSettings = _settingService.LoadSetting<CommonSettings>(siteScope);
            model.FullTextSettings.Supported = _fulltextService.IsFullTextSupported();
            model.FullTextSettings.Enabled = commonSettings.UseFullTextSearch;
            model.FullTextSettings.SearchMode = commonSettings.FullTextMode;
            model.FullTextSettings.SearchModeValues = commonSettings.FullTextMode.ToSelectList();

            //company information
            var companySettings = _settingService.LoadSetting<CompanyInformationSettings>(siteScope);
            model.CompanyInformationSettings.CompanyName = companySettings.CompanyName;
            model.CompanyInformationSettings.Salutation = companySettings.Salutation;
            model.CompanyInformationSettings.Title = companySettings.Title;
            model.CompanyInformationSettings.Firstname = companySettings.Firstname;
            model.CompanyInformationSettings.Lastname = companySettings.Lastname;
            model.CompanyInformationSettings.CompanyManagementDescription = companySettings.CompanyManagementDescription;
            model.CompanyInformationSettings.CompanyManagement = companySettings.CompanyManagement;
            model.CompanyInformationSettings.Street = companySettings.Street;
            model.CompanyInformationSettings.Street2 = companySettings.Street2;
            model.CompanyInformationSettings.ZipCode = companySettings.ZipCode;
            model.CompanyInformationSettings.City = companySettings.City;
            model.CompanyInformationSettings.CountryId = companySettings.CountryId;
            model.CompanyInformationSettings.Region = companySettings.Region;
            model.CompanyInformationSettings.VatId = companySettings.VatId;
            model.CompanyInformationSettings.CommercialRegister = companySettings.CommercialRegister;
            model.CompanyInformationSettings.TaxNumber = companySettings.TaxNumber;

            SiteDependingSettings.GetOverrideKeys(companySettings, model.CompanyInformationSettings, siteScope, _settingService, false);

            foreach (var c in _countryService.GetAllCountries(true))
            {
                model.CompanyInformationSettings.AvailableCountries.Add(
                    new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = (c.Id == model.CompanyInformationSettings.CountryId)
                    });
            }

            model.CompanyInformationSettings.Salutations.Add(ResToSelectListItem("Admin.Address.Salutation.Mr"));
            model.CompanyInformationSettings.Salutations.Add(ResToSelectListItem("Admin.Address.Salutation.Mrs"));

            model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Manager"));
            model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Shopkeeper"));
            model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Procurator"));
            model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Shareholder"));
            model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.AuthorizedPartner"));
            model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Director"));
            model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.ManagingPartner"));

            //contact data
            var contactDataSettings = _settingService.LoadSetting<ContactDataSettings>(siteScope);
            model.ContactDataSettings.CompanyTelephoneNumber = contactDataSettings.CompanyTelephoneNumber;
            model.ContactDataSettings.HotlineTelephoneNumber = contactDataSettings.HotlineTelephoneNumber;
            model.ContactDataSettings.MobileTelephoneNumber = contactDataSettings.MobileTelephoneNumber;
            model.ContactDataSettings.CompanyFaxNumber = contactDataSettings.CompanyFaxNumber;
            model.ContactDataSettings.CompanyEmailAddress = contactDataSettings.CompanyEmailAddress;
            model.ContactDataSettings.WebmasterEmailAddress = contactDataSettings.WebmasterEmailAddress;
            model.ContactDataSettings.SupportEmailAddress = contactDataSettings.SupportEmailAddress;
            model.ContactDataSettings.ContactEmailAddress = contactDataSettings.ContactEmailAddress;

            SiteDependingSettings.GetOverrideKeys(contactDataSettings, model.ContactDataSettings, siteScope, _settingService, false);

            //bank connection
            var bankConnectionSettings = _settingService.LoadSetting<BankConnectionSettings>(siteScope);
            model.BankConnectionSettings.Bankname = bankConnectionSettings.Bankname;
            model.BankConnectionSettings.Bankcode = bankConnectionSettings.Bankcode;
            model.BankConnectionSettings.AccountNumber = bankConnectionSettings.AccountNumber;
            model.BankConnectionSettings.AccountHolder = bankConnectionSettings.AccountHolder;
            model.BankConnectionSettings.Iban = bankConnectionSettings.Iban;
            model.BankConnectionSettings.Bic = bankConnectionSettings.Bic;

            SiteDependingSettings.GetOverrideKeys(bankConnectionSettings, model.BankConnectionSettings, siteScope, _settingService, false);

            //social
            var socialSettings = _settingService.LoadSetting<SocialSettings>(siteScope);
            model.SocialSettings.ShowSocialLinksInFooter = socialSettings.ShowSocialLinksInFooter;
            model.SocialSettings.FacebookLink = socialSettings.FacebookLink;
            model.SocialSettings.GooglePlusLink = socialSettings.GooglePlusLink;
            model.SocialSettings.TwitterLink = socialSettings.TwitterLink;
            model.SocialSettings.PinterestLink = socialSettings.PinterestLink;
            model.SocialSettings.YoutubeLink = socialSettings.YoutubeLink;

            SiteDependingSettings.GetOverrideKeys(socialSettings, model.SocialSettings, siteScope, _settingService, false);

            return View(model);
        }

        private SelectListItem ResToSelectListItem(string resourceKey)
        {
            string value = _localizationService.GetResource(resourceKey).EmptyNull();
            return new SelectListItem() { Text = value, Value = value };
        }


        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public ActionResult ChangeEnryptionKey(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(siteScope);

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = "";

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (String.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new WorkException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

                string oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new WorkException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));

                ////update encrypted order info
                //var orders = _orderService.LoadAllOrders();
                //foreach (var order in orders)
                //{
                //    // new credit card encryption
                //    string decryptedCardType = _encryptionService.DecryptText(order.CardType, oldEncryptionPrivateKey);
                //    string decryptedCardName = _encryptionService.DecryptText(order.CardName, oldEncryptionPrivateKey);
                //    string decryptedCardNumber = _encryptionService.DecryptText(order.CardNumber, oldEncryptionPrivateKey);
                //    string decryptedMaskedCreditCardNumber = _encryptionService.DecryptText(order.MaskedCreditCardNumber, oldEncryptionPrivateKey);
                //    string decryptedCardCvv2 = _encryptionService.DecryptText(order.CardCvv2, oldEncryptionPrivateKey);
                //    string decryptedCardExpirationMonth = _encryptionService.DecryptText(order.CardExpirationMonth, oldEncryptionPrivateKey);
                //    string decryptedCardExpirationYear = _encryptionService.DecryptText(order.CardExpirationYear, oldEncryptionPrivateKey);

                //    string encryptedCardType = _encryptionService.EncryptText(decryptedCardType, newEncryptionPrivateKey);
                //    string encryptedCardName = _encryptionService.EncryptText(decryptedCardName, newEncryptionPrivateKey);
                //    string encryptedCardNumber = _encryptionService.EncryptText(decryptedCardNumber, newEncryptionPrivateKey);
                //    string encryptedMaskedCreditCardNumber = _encryptionService.EncryptText(decryptedMaskedCreditCardNumber, newEncryptionPrivateKey);
                //    string encryptedCardCvv2 = _encryptionService.EncryptText(decryptedCardCvv2, newEncryptionPrivateKey);
                //    string encryptedCardExpirationMonth = _encryptionService.EncryptText(decryptedCardExpirationMonth, newEncryptionPrivateKey);
                //    string encryptedCardExpirationYear = _encryptionService.EncryptText(decryptedCardExpirationYear, newEncryptionPrivateKey);

                //    order.CardType = encryptedCardType;
                //    order.CardName = encryptedCardName;
                //    order.CardNumber = encryptedCardNumber;
                //    order.MaskedCreditCardNumber = encryptedMaskedCreditCardNumber;
                //    order.CardCvv2 = encryptedCardCvv2;
                //    order.CardExpirationMonth = encryptedCardExpirationMonth;
                //    order.CardExpirationYear = encryptedCardExpirationYear;

                //    // new direct debit encryption
                //    string decryptedAccountHolder = _encryptionService.DecryptText(order.DirectDebitAccountHolder, oldEncryptionPrivateKey);
                //    string decryptedAccountNumber = _encryptionService.DecryptText(order.DirectDebitAccountNumber, oldEncryptionPrivateKey);
                //    string decryptedBankCode = _encryptionService.DecryptText(order.DirectDebitBankCode, oldEncryptionPrivateKey);
                //    string decryptedBankName = _encryptionService.DecryptText(order.DirectDebitBankName, oldEncryptionPrivateKey);
                //    string decryptedBic = _encryptionService.DecryptText(order.DirectDebitBIC, oldEncryptionPrivateKey);
                //    string decryptedCountry = _encryptionService.DecryptText(order.DirectDebitCountry, oldEncryptionPrivateKey);
                //    string decryptedIban = _encryptionService.DecryptText(order.DirectDebitIban, oldEncryptionPrivateKey);

                //    string encryptedAccountHolder = _encryptionService.EncryptText(decryptedAccountHolder, newEncryptionPrivateKey);
                //    string encryptedAccountNumber = _encryptionService.EncryptText(decryptedAccountNumber, newEncryptionPrivateKey);
                //    string encryptedBankCode = _encryptionService.EncryptText(decryptedBankCode, newEncryptionPrivateKey);
                //    string encryptedBankName = _encryptionService.EncryptText(decryptedBankName, newEncryptionPrivateKey);
                //    string encryptedBic = _encryptionService.EncryptText(decryptedBic, newEncryptionPrivateKey);
                //    string encryptedCountry = _encryptionService.EncryptText(decryptedCountry, newEncryptionPrivateKey);
                //    string encryptedIban = _encryptionService.EncryptText(decryptedIban, newEncryptionPrivateKey);

                //    order.DirectDebitAccountHolder = encryptedAccountHolder;
                //    order.DirectDebitAccountNumber = encryptedAccountNumber;
                //    order.DirectDebitBankCode = encryptedBankCode;
                //    order.DirectDebitBankName = encryptedBankName;
                //    order.DirectDebitBIC = encryptedBic;
                //    order.DirectDebitCountry = encryptedCountry;
                //    order.DirectDebitIban = encryptedIban;

                //    _orderService.UpdateOrder(order);
                //}

                //update user information
                //optimization - load only users with PasswordFormat.Encrypted
                var users = _userService.GetAllUsersByPasswordFormat(PasswordFormat.Encrypted);
                foreach (var user in users)
                {
                    string decryptedPassword = _encryptionService.DecryptText(user.Password, oldEncryptionPrivateKey);
                    string encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    user.Password = encryptedPassword;
                    _userService.UpdateUser(user);
                }

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(securitySettings);
                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                NotifyError(exc);
            }
            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("togglefulltext")]
        public ActionResult ToggleFullText(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var siteScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(siteScope);

            try
            {
                if (!_fulltextService.IsFullTextSupported())
                    throw new WorkException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.NotSupported"));

                if (commonSettings.UseFullTextSearch)
                {
                    _fulltextService.DisableFullText();

                    commonSettings.UseFullTextSearch = false;
                    _settingService.SaveSetting(commonSettings, siteScope);

                    NotifySuccess(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disabled"));
                }
                else
                {
                    _fulltextService.EnableFullText();

                    commonSettings.UseFullTextSearch = true;
                    _settingService.SaveSetting(commonSettings, siteScope);

                    NotifySuccess(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enabled"));
                }
            }
            catch (Exception exc)
            {
                NotifyError(exc);
            }
            return RedirectToAction("GeneralCommon");
        }


        public ActionResult ArticleCatalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            var catalogSettings = _settingService.LoadSetting<ArticleCatalogSettings>(storeScope);
            var model = catalogSettings.ToModel();

            SiteDependingSettings.GetOverrideKeys(catalogSettings, model, storeScope, _settingService);

            //model.AvailableSubCategoryDisplayTypes = catalogSettings.SubCategoryDisplayType.ToSelectList();

            model.AvailableDefaultViewModes.Add(
                new SelectListItem { Value = "grid", Text = _localizationService.GetResource("Common.Grid"), Selected = model.DefaultViewMode.IsCaseInsensitiveEqual("grid") }
            );
            model.AvailableDefaultViewModes.Add(
                new SelectListItem { Value = "list", Text = _localizationService.GetResource("Common.List"), Selected = model.DefaultViewMode.IsCaseInsensitiveEqual("list") }
            );


            return View(model);
        }
        [HttpPost]
        public ActionResult ArticleCatalog(ArticleCatalogSettingsModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveSiteScopeConfiguration(_siteService, _workContext);
            var catalogSettings = _settingService.LoadSetting<ArticleCatalogSettings>(storeScope);
            catalogSettings = model.ToEntity(catalogSettings);

            SiteDependingSettings.UpdateSettings(catalogSettings, form, storeScope, _settingService);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("ArticleCatalog");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Fakes;
using CAF.Infrastructure.Core.Collections;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Common;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Application.Services.Authentication;
using CAF.Infrastructure.Core.Domain.Security;
using System.Web.Security;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.WebSite.Application.Services.Directory;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Domain.Tax;
//using CAF.WebSite.Application.Services.Tax;



namespace CAF.WebSite.Application.WebUI
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        private const string UserCookieName = "caf.user";
        private const string AppSystemCookieName = "caf.appsystem";
        private readonly IUserService _userService;
        private readonly ISiteContext _siteContext;
        private readonly HttpContextBase _httpContext;
        private readonly ILanguageService _languageService;
        private readonly ICacheManager _cacheManager;
        private readonly IGenericAttributeService _attrService;
        private readonly ISiteService _siteService;
        private readonly TimeSpan _expirationTimeSpan;
        private readonly SecuritySettings _securitySettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly CurrencySettings _currencySettings;
        private readonly ISettingService _settingService;
        private readonly TaxSettings _taxSettings;
        private readonly IUserAgent _userAgent;

        private TaxDisplayType? _cachedTaxDisplayType;
        private Language _cachedLanguage;
        private User _cachedUser;
        private User _originalUserIfImpersonated;
        public WebWorkContext(Func<string, ICacheManager> cacheManager,
            HttpContextBase httpContext,
            IUserService userService,
            LocalizationSettings localizationSettings,
            SecuritySettings securitySettings, CurrencySettings currencySettings,
            ISiteContext siteContext,
            ILanguageService languageService,
            ISiteService siteService,
            IAuthenticationService authenticationService,
            IGenericAttributeService attrService,
            ISettingService settingService,
              TaxSettings taxSettings,
            IUserAgent userAgent
            )
        {

            this._cacheManager = cacheManager("static");
            this._userService = userService;
            this._httpContext = httpContext;
            this._siteContext = siteContext;
            this._languageService = languageService;
            this._siteService = siteService;
            this._currencySettings = currencySettings;
            this._taxSettings = taxSettings;
            this._localizationSettings = localizationSettings;
            this._securitySettings = securitySettings;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
            this._authenticationService = authenticationService;
            this._attrService = attrService;
            this._settingService = settingService;
            this._userAgent = userAgent;
        }
        protected HttpCookie GetUserCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[UserCookieName];
        }

        protected void SetUserCookie(Guid userGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserCookieName);
                cookie.HttpOnly = true;
                cookie.Value = userGuid.ToString();
                if (userGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    //int cookieExpires = 24 * 365; //通过Forms配置
                    cookie.Expires = DateTime.Now.Add(_expirationTimeSpan);
                }

                _httpContext.Response.Cookies.Remove(UserCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }


        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public User CurrentUser
        {
            get
            {
                if (_cachedUser != null)
                    return _cachedUser;

                User user = null;

                // check whether request is made by a background task
                // in this case return built-in user record for background task
                if (_httpContext == null || _httpContext.IsFakeContext())
                {
                    user = _userService.GetUserByUserName(SystemUserNames.BackgroundTask);
                }

                // check whether request is made by a search engine
                // in this case return built-in user record for search engines 
                if (user == null || user.Deleted || !user.Active)
                {
                    if (_userAgent.IsBot)
                    {
                        user = _userService.GetUserByUserName(SystemUserNames.SearchEngine);
                    }
                }

                // check whether request is made by the PDF converter
                // in this case return built-in user record for the converter
                if (user == null || user.Deleted || !user.Active)
                {
                    if (_userAgent.IsPdfConverter)
                    {
                        user = _userService.GetUserByUserName(SystemUserNames.PdfConverter);
                    }
                }

                //registered user
                if (user == null || user.Deleted || !user.Active)
                {
                    user = _authenticationService.GetAuthenticatedUser();
                }

                // impersonate user if required (currently used for 'phone order' support)
                if (user != null && !user.Deleted && user.Active)
                {
                    int? impersonatedUserId = user.GetAttribute<int?>(SystemUserAttributeNames.ImpersonatedUserId);
                    if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                    {
                        var impersonatedUser = _userService.GetUserById(impersonatedUserId.Value);
                        if (impersonatedUser != null && !impersonatedUser.Deleted && impersonatedUser.Active)
                        {
                            //set impersonated user
                            _originalUserIfImpersonated = user;
                            user = impersonatedUser;
                        }
                    }
                }
                //load guest user
                if (user == null || user.Deleted || !user.Active)
                {
                    var userCookie = GetUserCookie();
                    if (userCookie != null && !String.IsNullOrEmpty(userCookie.Value))
                    {
                        Guid userGuid;
                        if (Guid.TryParse(userCookie.Value, out userGuid))
                        {
                            var userByCookie = _userService.GetUserByGuid(userGuid);
                            if (userByCookie != null &&
                                //this user (from cookie) should not be registered
                                !userByCookie.IsRegistered() &&
                                //it should not be a built-in 'search engine' user account
                                !userByCookie.IsSearchEngineAccount())
                                user = userByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (user == null || user.Deleted || !user.Active)
                {
                        user = _userService.InsertGuestUser();

                }


                //validation
                if (user != null && !user.Deleted && user.Active)
                {
                    SetUserCookie(user.UserGuid);
                    _cachedUser = user;
                }

                return _cachedUser;
            }
            set
            {
                if (!value.IsSystemAccount)
                {
                    SetUserCookie(value.UserGuid);
                }
                //SetUserCookie(value == null ? Guid.Empty : value.UserGuid);
                _cachedUser = value;
            }
        }

        /// <summary>
        /// Gets or sets the original user (in case the current one is impersonated)
        /// </summary>
        public User OriginalUserIfImpersonated
        {
            get
            {
                return _originalUserIfImpersonated;
            }
        }

        /// <summary>
        /// Get or set current tax display type
        /// </summary>
        public TaxDisplayType TaxDisplayType
        {
            get
            {
                return GetTaxDisplayTypeFor(this.CurrentUser, _siteContext.CurrentSite.Id);
            }
            set
            {
                if (!_taxSettings.AllowUsersToSelectTaxDisplayType)
                    return;

                _attrService.SaveAttribute(this.CurrentUser,
                     SystemUserAttributeNames.TaxDisplayTypeId,
                     (int)value, _siteContext.CurrentSite.Id);
            }
        }

        public TaxDisplayType GetTaxDisplayTypeFor(User user, int siteId)
        {
            if (_cachedTaxDisplayType.HasValue)
            {
                return _cachedTaxDisplayType.Value;
            }

            int? taxDisplayType = null;

            if (_taxSettings.AllowUsersToSelectTaxDisplayType && user != null)
            {
                taxDisplayType = user.GetAttribute<int?>(SystemUserAttributeNames.TaxDisplayTypeId, siteId);
            }

            //if (!taxDisplayType.HasValue && _taxSettings.EuVatEnabled)
            //{
            //    if (user != null && _taxService.Value.IsVatExempt(null, user))
            //    {
            //        taxDisplayType = (int)TaxDisplayType.ExcludingTax;
            //    }
            //}

            if (!taxDisplayType.HasValue)
            {
                var userRoles = user.UserRoles;
                string key = string.Format(ModelCacheEventConsumer.USERRROLES_TAX_DISPLAY_TYPES_KEY, String.Join(",", userRoles.Select(x => x.Id)), siteId);
                var cacheResult = _cacheManager.Get(key, () =>
                {
                    var roleTaxDisplayTypes = userRoles
                        .Where(x => x.TaxDisplayType.HasValue)
                        .OrderByDescending(x => x.TaxDisplayType.Value)
                        .Select(x => x.TaxDisplayType.Value);

                    if (roleTaxDisplayTypes.Any())
                    {
                        return (TaxDisplayType)roleTaxDisplayTypes.FirstOrDefault();
                    }

                    return _taxSettings.TaxDisplayType;
                });

                taxDisplayType = (int)cacheResult;
            }

            _cachedTaxDisplayType = (TaxDisplayType)taxDisplayType.Value;
            return _cachedTaxDisplayType.Value;
        }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        public bool IsAdmin { get; set; }


        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                int siteId = _siteContext.CurrentSite.Id;
                int userLangId = 0;

                if (this.CurrentUser != null)
                {
                    userLangId = this.CurrentUser.GetAttribute<int>(
                        SystemUserAttributeNames.LanguageId,
                        _attrService,
                        _siteContext.CurrentSite.Id);
                }


                #region Get language from URL (if possible)
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && _httpContext != null && _httpContext.Request != null)
                {
                    var helper = new LocalizedUrlHelper(_httpContext.Request, true);
                    string seoCode;
                    if (helper.IsLocalizedUrl(out seoCode))
                    {
                        if (this.IsPublishedLanguage(seoCode, siteId))
                        {
                            // the language is found. now we need to save it
                            var langBySeoCode = _languageService.GetLanguageBySeoCode(seoCode);
                            if (this.CurrentUser != null && userLangId != langBySeoCode.Id)
                            {
                                userLangId = langBySeoCode.Id;
                                this.SetUserLanguage(langBySeoCode.Id, siteId);
                            }
                            _cachedLanguage = langBySeoCode;
                            return langBySeoCode;
                        }
                    }
                }
                #endregion

                if (_localizationSettings.DetectBrowserUserLanguage && (userLangId == 0 || !this.IsPublishedLanguage(userLangId, siteId)))
                {
                    #region Get Browser UserLanguage

                    // Fallback to browser detected language
                    Language browserLanguage = null;

                    if (_httpContext != null && _httpContext.Request != null && _httpContext.Request.UserLanguages != null)
                    {
                        var userLangs = _httpContext.Request.UserLanguages.Select(x => x.Split(new[] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (userLangs.Any())
                        {
                            foreach (var culture in userLangs)
                            {
                                browserLanguage = _languageService.GetLanguageByCulture(culture);
                                if (browserLanguage != null && this.IsPublishedLanguage(browserLanguage.Id, siteId))
                                {
                                    // the language is found. now we need to save it
                                    if (this.CurrentUser != null && userLangId != browserLanguage.Id)
                                    {
                                        userLangId = browserLanguage.Id;
                                        SetUserLanguage(userLangId, siteId);
                                    }
                                    _cachedLanguage = browserLanguage;
                                    return browserLanguage;
                                }
                            }
                        }
                    }

                    #endregion
                }

                if (userLangId > 0 && this.IsPublishedLanguage(userLangId, siteId))
                {
                    _cachedLanguage = _languageService.GetLanguageById(userLangId);
                    return _cachedLanguage;
                }

                // Fallback
                userLangId = this.GetDefaultLanguageId(siteId);
                SetUserLanguage(userLangId, siteId);

                _cachedLanguage = _languageService.GetLanguageById(userLangId);
                return _cachedLanguage;
            }
            set
            {
                var languageId = value != null ? value.Id : 0;
                this.SetUserLanguage(languageId, _siteContext.CurrentSite.Id);
                _cachedLanguage = null;
            }
        }


        private void SetUserLanguage(int languageId, int siteId)
        {
            _attrService.SaveAttribute(
               this.CurrentUser,
               SystemUserAttributeNames.LanguageId,
               languageId,
               siteId);
        }

 

        public bool IsPublishedLanguage(string seoCode, int siteId = 0)
        {
            if (siteId <= 0)
                siteId = _siteContext.CurrentSite.Id;

            var map = this.GetSiteLanguageMap();
            if (map.ContainsKey(siteId))
            {
                return map[siteId].Any(x => x.Item2 == seoCode);
            }

            return false;
        }

        internal bool IsPublishedLanguage(int languageId, int siteId = 0)
        {
            if (languageId <= 0)
                return false;

            if (siteId <= 0)
                siteId = _siteContext.CurrentSite.Id;

            var map = this.GetSiteLanguageMap();
            if (map.ContainsKey(siteId))
            {
                return map[siteId].Any(x => x.Item1 == languageId);
            }

            return false;
        }
        public string GetDefaultLanguageSeoCode(int siteId = 0)
        {
            if (siteId <= 0)
                siteId = _siteContext.CurrentSite.Id;

            var map = this.GetSiteLanguageMap();
            if (map.ContainsKey(siteId))
            {
                return map[siteId].FirstOrDefault().Item2;
            }

            return null;
        }

        internal int GetDefaultLanguageId(int siteId = 0)
        {
            if (siteId <= 0)
                siteId = _siteContext.CurrentSite.Id;

            var map = this.GetSiteLanguageMap();
            if (map.ContainsKey(siteId))
            {
                return map[siteId].FirstOrDefault().Item1;
            }

            return 0;
        }

        /// <summary>
        /// Gets a map of active/published site languages
        /// </summary>
        /// <returns>A map of site languages where key is the site id and values are tuples of lnguage ids and seo codes</returns>
        protected virtual Multimap<int, Tuple<int, string>> GetSiteLanguageMap()
        {
            var result = _cacheManager.Get(ModelCacheEventConsumer.STORE_LANGUAGE_MAP_KEY, () =>
            {
                var map = new Multimap<int, Tuple<int, string>>();

                var allSites = _siteService.GetAllSites();
                foreach (var site in allSites)
                {
                    var languages = _languageService.GetAllLanguages(false, site.Id);
                    if (!languages.Any())
                    {
                        // language-less sites aren't allowed but could exist accidentally. Correct this.
                        var firstSiteLang = _languageService.GetAllLanguages(true, site.Id).FirstOrDefault();
                        if (firstSiteLang == null)
                        {
                            // absolute fallback
                            firstSiteLang = _languageService.GetAllLanguages(true).FirstOrDefault();
                        }
                        map.Add(site.Id, new Tuple<int, string>(firstSiteLang.Id, firstSiteLang.UniqueSeoCode));
                    }
                    else
                    {
                        foreach (var lang in languages)
                        {
                            map.Add(site.Id, new Tuple<int, string>(lang.Id, lang.UniqueSeoCode));
                        }
                    }
                }

                return map;
            }, 1440 /* 24 hrs */);

            return result;
        }
    }
}

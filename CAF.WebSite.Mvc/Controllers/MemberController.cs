using CAF.WebSite.Application.WebUI.Security;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Models.Users;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Authentication;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Directory;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.WebSite.Mvc.Models.Common;
using CAF.WebSite.Application.Services.Forums;
using CAF.WebSite.Application.Services.Seo;
using CAF.Infrastructure.Core.Utilities;
using CAF.Infrastructure.Core.Exceptions;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CaptchaMvc.HtmlHelpers;
namespace CAF.WebSite.Mvc.Controllers
{
    /// <summary>
    /// 会员
    /// </summary>
    public class MemberController : PublicControllerBase
    {
        #region Fields
        private readonly UserSettings _userSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IForumService _forumService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IWebHelper _webHelper;
        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        #endregion

        #region Ctor

        public MemberController(IAuthenticationService authenticationService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            DateTimeSettings dateTimeSettings,
            ForumSettings forumSettings, AddressSettings addressSettings,
            IWorkContext workContext, ISiteContext siteContext,
            IUserService userService, IAddressService addressService,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IGenericAttributeService genericAttributeService,
            IUserRegistrationService userRegistrationService,
            IUserActivityService userActivityService,
            UserSettings UserSettings, IWebHelper webHelper,
            IWorkflowMessageService workflowMessageService,
            IForumService forumService,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            INewsLetterSubscriptionService newsLetterSubscriptionService, LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings)
        {
            this._addressSettings = addressSettings;
            this._forumSettings = forumSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._dateTimeSettings = dateTimeSettings;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._captchaSettings = captchaSettings;
            this._userSettings = UserSettings;
            this._webHelper = webHelper;
            this._userService = userService;
            this._forumService = forumService;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._userActivityService = userActivityService;
            this._localizationService = localizationService;
            this._userRegistrationService = userRegistrationService;
            this._genericAttributeService = genericAttributeService;
            this._authenticationService = authenticationService;
            this._workflowMessageService = workflowMessageService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._localizationSettings = localizationSettings;

            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
        }
        #endregion

        #region Utilities
        [NonAction]
        protected bool IsCurrentUserRegistered()
        {
            return _workContext.CurrentUser.IsRegistered();
        }
        [NonAction]
        protected UserNavigationModel GetUserNavigationModel(User user)
        {
            var model = new UserNavigationModel();
            model.HideAvatar = !_userSettings.AllowUsersToUploadAvatars;
            // model.HideRewardPoints = !_rewardPointsSettings.Enabled;
            model.HideForumSubscriptions = !_forumSettings.ForumsEnabled || !_forumSettings.AllowUsersToManageSubscriptions;
            return model;
        }
        [NonAction]
        protected bool UserNameIsValid(string username)
        {
            var result = true;

            if (String.IsNullOrEmpty(username))
            {
                return false;
            }

            // other validation 

            return result;
        }
        [NonAction]
        protected void TryAssociateAccountWithExternalAccount(User user)
        {
            //var parameters = ExternalAuthorizerHelper.RetrieveParametersFromRoundTrip(true);
            //if (parameters == null)
            //    return;

            //if (_openAuthenticationService.AccountExists(parameters))
            //    return;

            //_openAuthenticationService.AssociateExternalAccountWithUser(user, parameters);
        }

        [NonAction]
        protected void PrepareUserInfoModel(UserInfoModel model, User user, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (user == null)
                throw new ArgumentNullException("user");

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            if (!excludeProperties)
            {
                model.VatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);
                model.FirstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
                model.LastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);
                model.Gender = user.GetAttribute<string>(SystemUserAttributeNames.Gender);
                var dateOfBirth = user.GetAttribute<DateTime?>(SystemUserAttributeNames.DateOfBirth);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
                model.Company = user.GetAttribute<string>(SystemUserAttributeNames.Company);
                model.StreetAddress = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
                model.StreetAddress2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
                model.ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);
                model.City = user.GetAttribute<string>(SystemUserAttributeNames.City);
                model.CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                model.StateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId);
                model.Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
                model.Fax = user.GetAttribute<string>(SystemUserAttributeNames.Fax);

                //newsletter
                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(user.Email, _siteContext.CurrentSite.Id);
                model.Newsletter = newsletter != null && newsletter.Active;

                model.Signature = user.GetAttribute<string>(SystemUserAttributeNames.Signature);

                model.Email = user.Email;
                model.UserName = user.UserName;
            }
            else
            {
                if (_userSettings.UserNamesEnabled && !_userSettings.AllowUsersToChangeUserNames)
                    model.UserName = user.UserName;
            }

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                            model.AvailableStates.Add(new SelectListItem() { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                    else
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

                }
            }
            //model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //model.VatNumberStatusNote = ((VatNumberStatus)user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId))
            //     .GetLocalizedEnum(_localizationService, _workContext);
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.AllowUsersToChangeUserNames = _userSettings.AllowUsersToChangeUserNames;
            model.CheckUserNameAvailabilityEnabled = _userSettings.CheckUserNameAvailabilityEnabled;
            model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

            ////external authentication
            //foreach (var ear in _openAuthenticationService.GetExternalIdentifiersFor(user))
            //{
            //    var authMethod = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(ear.ProviderSystemName);
            //    if (authMethod == null || !authMethod.IsMethodActive(_externalAuthenticationSettings))
            //        continue;

            //    model.AssociatedExternalAuthRecords.Add(new UserInfoModel.AssociatedExternalAuthModel()
            //    {
            //        Id = ear.Id,
            //        Email = ear.Email,
            //        ExternalIdentifier = ear.ExternalIdentifier,
            //        AuthMethodName = _pluginMediator.GetLocalizedFriendlyName(authMethod.Metadata, _workContext.WorkingLanguage.Id)
            //    });
            //}

            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Info;
        }

        [NonAction]
        //protected UserOrderListModel PrepareUserOrderListModel(User user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException("user");

        //    var model = new UserOrderListModel();
        //    model.NavigationModel = GetUserNavigationModel(user);
        //    model.NavigationModel.SelectedTab = UserNavigationEnum.Orders;
        //    var orders = _orderService.SearchOrders(_siteContext.CurrentSite.Id, user.Id,
        //        null, null, null, null, null, null, null, null, 0, int.MaxValue);
        //    foreach (var order in orders)
        //    {
        //        var orderModel = new UserOrderListModel.OrderDetailsModel()
        //        {
        //            Id = order.Id,
        //            OrderNumber = order.GetOrderNumber(),
        //            CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
        //            OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
        //            IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order)
        //        };
        //        var orderTotalInUserCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
        //        orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInUserCurrency, true, order.UserCurrencyCode, false, _workContext.WorkingLanguage);

        //        model.Orders.Add(orderModel);
        //    }

        //    var recurringPayments = _orderService.SearchRecurringPayments(_siteContext.CurrentSite.Id,
        //        user.Id, 0, null);
        //    foreach (var recurringPayment in recurringPayments)
        //    {
        //        var recurringPaymentModel = new UserOrderListModel.RecurringOrderModel()
        //        {
        //            Id = recurringPayment.Id,
        //            StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
        //            CycleInfo = string.Format("{0} {1}", recurringPayment.CycleLength, recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext)),
        //            NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
        //            TotalCycles = recurringPayment.TotalCycles,
        //            CyclesRemaining = recurringPayment.CyclesRemaining,
        //            InitialOrderId = recurringPayment.InitialOrder.Id,
        //            CanCancel = _orderProcessingService.CanCancelRecurringPayment(user, recurringPayment),
        //        };

        //        model.RecurringOrders.Add(recurringPaymentModel);
        //    }

        //    return model;
        //}
        #endregion

        public ActionResult Index()
        {
            return View();
        }

        #region Login / logout / register

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Login(bool? checkoutAsGuest)
        {
            var model = new LoginModel();
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.CheckoutAsGuest = checkoutAsGuest.HasValue ? checkoutAsGuest.Value : false;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;

            // codehint: sm-add
            if (_userSettings.PrefillLoginUserName.HasValue())
            {
                if (model.UserNamesEnabled)
                    model.UserName = _userSettings.PrefillLoginUserName;
                else
                    model.Email = _userSettings.PrefillLoginUserName;

            }
            if (_userSettings.PrefillLoginPwd.HasValue())
            {
                model.Password = _userSettings.PrefillLoginPwd;
            }

            return View(model);
        }

        [HttpPost]
       // [CaptchaMvc.Attributes.CaptchaVerify("验证码错误，请重新填写")]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !this.IsCaptchaValid("验证码错误，请重新填写"))
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }
            if (ModelState.IsValid)
            {
                if (_userSettings.UserNamesEnabled && model.UserName != null)
                {
                    model.UserName = model.UserName.Trim();
                }

                if (_userRegistrationService.ValidateUser(_userSettings.UserNamesEnabled ? model.UserName : model.Email, model.Password))
                {
                    var User = _userSettings.UserNamesEnabled ? _userService.GetUserByUserName(model.UserName) : _userService.GetUserByEmail(model.Email);


                    //sign in new User
                    _authenticationService.SignIn(User, model.RememberMe);

                    //activity log
                    _userActivityService.InsertActivity("PublicSite.Login", _localizationService.GetResource("ActivityLog.PublicSite.Login"), User);

                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return Redirect("/admin");
                }
                else
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                }
            }

            //If we got this far, something failed, redisplay form
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return View(model);
        }


        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Register()
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new RegisterModel();
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultSiteTimeZone.Id) });
            // model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.CheckUserNameAvailabilityEnabled = _userSettings.CheckUserNameAvailabilityEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem() { Text = c.GetLocalized(x => x.Name), Value = c.Id.ToString() });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                            model.AvailableStates.Add(new SelectListItem() { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                    else
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateMvcCaptcha]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl, bool captchaValid)
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_workContext.CurrentUser.IsRegistered())
            {
                //Already registered user. 
                _authenticationService.SignOut();

                //Save a new record
                _workContext.CurrentUser = _userService.InsertGuestUser();
            }
            var user = _workContext.CurrentUser;

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {
                if (_userSettings.UserNamesEnabled && model.UserName != null)
                {
                    model.UserName = model.UserName.Trim();
                }

                bool isApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new UserRegistrationRequest(user, model.Email,
                    _userSettings.UserNamesEnabled ? model.UserName : model.Email, model.Password, _userSettings.DefaultPasswordFormat, isApproved);
                var registrationResult = _userRegistrationService.RegisterUser(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    //VAT number
                    //if (_taxSettings.EuVatEnabled)
                    //{
                    //    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.VatNumber, model.VatNumber);

                    //    string vatName = "";
                    //    string vatAddress = "";
                    //    var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                    //    _genericAttributeService.SaveAttribute(user,
                    //        SystemUserAttributeNames.VatNumberStatusId,
                    //        (int)vatNumberStatus);
                    //    //send VAT number admin notification
                    //    if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                    //        _workflowMessageService.SendNewVatSubmittedSiteOwnerNotification(user, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    //}

                    //form fields
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = null;
                        try
                        {
                            dateOfBirth = new DateTime(model.DateOfBirthYear.Value, model.DateOfBirthMonth.Value, model.DateOfBirthDay.Value);
                        }
                        catch { }
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, dateOfBirth);
                    }
                    if (_userSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Company, model.Company);
                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress, model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    if (_userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.City, model.City);
                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CountryId, model.CountryId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);

                    //newsletter
                    if (_userSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(model.Email, _siteContext.CurrentSite.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription()
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    Active = true,
                                    CreatedOnUtc = DateTime.UtcNow,
                                    SiteId = _siteContext.CurrentSite.Id
                                });
                            }
                        }
                    }

                    //login user now
                    if (isApproved)
                        _authenticationService.SignIn(user, true);

                    //associated with external account (if possible)
                    TryAssociateAccountWithExternalAccount(user);

                    //insert default address (if possible)
                    var defaultAddress = new Address()
                    {
                        FirstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName),
                        LastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName),
                        Email = user.Email,
                        Company = user.GetAttribute<string>(SystemUserAttributeNames.Company),
                        CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId) > 0 ?
                            (int?)user.GetAttribute<int>(SystemUserAttributeNames.CountryId) : null,
                        StateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId) > 0 ?
                            (int?)user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId) : null,
                        City = user.GetAttribute<string>(SystemUserAttributeNames.City),
                        Address1 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress),
                        Address2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2),
                        ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode),
                        PhoneNumber = user.GetAttribute<string>(SystemUserAttributeNames.Phone),
                        FaxNumber = user.GetAttribute<string>(SystemUserAttributeNames.Fax),
                        CreatedOnUtc = user.CreatedOnUtc
                    };
                    if (this._addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        user.Addresses.Add(defaultAddress);
                        user.BillingAddress = defaultAddress;
                        user.ShippingAddress = defaultAddress;
                        _userService.UpdateUser(user);
                    }

                    //notifications
                    if (_userSettings.NotifyNewUserRegistration)
                        _workflowMessageService.SendUserRegisteredNotificationMessage(user, _localizationSettings.DefaultAdminLanguageId);

                    switch (_userSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                _workflowMessageService.SendUserEmailValidationMessage(user, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send user welcome message
                                _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

                                var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                if (!String.IsNullOrEmpty(returnUrl))
                                    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("HomePage");
                            }
                    }
                }
                else
                {
                    foreach (var error in registrationResult.Errors)
                        ModelState.AddModelError("", error);
                }
            }

            //If we got this far, something failed, redisplay form
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultSiteTimeZone.Id) });
            //model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.CheckUserNameAvailabilityEnabled = _userSettings.CheckUserNameAvailabilityEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem() { Text = c.GetLocalized(x => x.Name), Value = c.Id.ToString(), Selected = (c.Id == model.CountryId) });
                }


                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                            model.AvailableStates.Add(new SelectListItem() { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                    else
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

                }
            }

            return View(model);
        }

        public ActionResult RegisterResult(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel()
            {
                Result = resultText
            };
            return View(model);
        }
        /// <summary>
        /// 验证用户名是否可用
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CheckUserNameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = _localizationService.GetResource("Account.CheckUserNameAvailability.NotAvailable");

            if (_userSettings.UserNamesEnabled && username != null)
            {
                username = username.Trim();

                if (UserNameIsValid(username))
                {
                    if (_workContext.CurrentUser != null &&
                        _workContext.CurrentUser.UserName != null &&
                        _workContext.CurrentUser.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                    {
                        statusText = _localizationService.GetResource("Account.CheckUserNameAvailability.CurrentUserName");
                    }
                    else
                    {
                        var User = _userService.GetUserByUserName(username);
                        if (User == null)
                        {
                            statusText = _localizationService.GetResource("Account.CheckUserNameAvailability.Available");
                            usernameAvailable = true;
                        }
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        public ActionResult Logout()
        {
            //external authentication
            // ExternalAuthorizerHelper.RemoveParameters();

            if (_workContext.OriginalUserIfImpersonated != null)
            {
                //logout impersonated User
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalUserIfImpersonated,
                    SystemUserAttributeNames.ImpersonatedUserId, null);
                //redirect back to User details page (admin area)
                return this.RedirectToAction("Edit", "User", new { id = _workContext.CurrentUser.Id, area = "Admin" });

            }
            else
            {
                //standard logout 

                //activity log
                _userActivityService.InsertActivity("PublicSite.Logout", _localizationService.GetResource("ActivityLog.PublicSite.Logout"));

                _authenticationService.SignOut();
                return RedirectToRoute("HomePage");
            }

        }
        /// <summary>
        /// 用户邮箱验证
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult AccountActivation(string token, string email)
        {
            var User = _userService.GetUserByEmail(email);
            if (User == null)
                return RedirectToRoute("HomePage");

            var cToken = User.GetAttribute<string>(SystemUserAttributeNames.AccountActivationToken);
            if (String.IsNullOrEmpty(cToken))
                return RedirectToRoute("HomePage");

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            //activate user account
            User.Active = true;
            _userService.UpdateUser(User);
            _genericAttributeService.SaveAttribute(User, SystemUserAttributeNames.AccountActivationToken, "");
            //send welcome message
            _workflowMessageService.SendUserWelcomeMessage(User, _workContext.WorkingLanguage.Id);

            var model = new AccountActivationModel();
            model.Result = _localizationService.GetResource("Account.AccountActivation.Activated");
            return View(model);
        }

        #endregion

        #region My account

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult MyAccount()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = GetUserNavigationModel(user);
            return View(model);
        }

        #region Info
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Info()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = new UserInfoModel();
            PrepareUserInfoModel(model, user, false);

            return View(model);
        }
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info(UserInfoModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            //email
            if (String.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("", "Email is not provided.");
            //username 
            if (_userSettings.UserNamesEnabled &&
                this._userSettings.AllowUsersToChangeUserNames)
            {
                if (String.IsNullOrEmpty(model.UserName))
                    ModelState.AddModelError("", "UserName is not provided.");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_userSettings.UserNamesEnabled &&
                        this._userSettings.AllowUsersToChangeUserNames)
                    {
                        if (!user.UserName.Equals(model.UserName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _userRegistrationService.SetUserName(user, model.UserName.Trim());
                            //re-authenticate
                            _authenticationService.SignIn(user, true);
                        }
                    }
                    //email
                    if (!user.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        _userRegistrationService.SetEmail(user, model.Email.Trim());
                        //re-authenticate (if usernames are disabled)
                        if (!_userSettings.UserNamesEnabled)
                        {
                            _authenticationService.SignIn(user, true);
                        }
                    }

                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                   

                    //form fields
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = null;
                        try
                        {
                            dateOfBirth = new DateTime(model.DateOfBirthYear.Value, model.DateOfBirthMonth.Value, model.DateOfBirthDay.Value);
                        }
                        catch { }
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, dateOfBirth);
                    }
                    if (_userSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Company, model.Company);
                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress, model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    if (_userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.City, model.City);
                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CountryId, model.CountryId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);

                    //newsletter
                    if (_userSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(user.Email, _siteContext.CurrentSite.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            else
                            {
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            }
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription()
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = user.Email,
                                    Active = true,
                                    CreatedOnUtc = DateTime.UtcNow,
                                    SiteId = _siteContext.CurrentSite.Id
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Signature, model.Signature);

                    return RedirectToAction("Info");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }


            //If we got this far, something failed, redisplay form
            PrepareUserInfoModel(model, user, true);
            return View(model);
        }

        #endregion

        #region Addresses
        /// <summary>
        /// 用户地址本管理
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Addresses()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = new UserAddressListModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            foreach (var address in user.Addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModel(address, false, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries());
                model.Addresses.Add(addressModel);
            }
            return View(model);
        }

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult AddressDelete(int id)
        {
            if (id < 1)
                return HttpNotFound();

            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.Where(a => a.Id == id).FirstOrDefault();
            if (address != null)
            {
                user.RemoveAddress(address);
                _userService.UpdateUser(user);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            return RedirectToAction("Addresses");
        }

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult AddressAdd()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = new UserAddressEditModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            model.Address.PrepareModel(null, false, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries());

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressAdd(UserAddressEditModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;


            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                user.Addresses.Add(address);
                _userService.UpdateUser(user);

                return RedirectToAction("Addresses");
            }


            //If we got this far, something failed, redisplay form
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            model.Address.PrepareModel(null, true, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries());

            return View(model);
        }

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult AddressEdit(int id)
        {
            if (id < 1)
                return HttpNotFound();

            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;
            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.Where(a => a.Id == id).FirstOrDefault();
            if (address == null)
                //address is not found
                return RedirectToAction("Addresses");

            var model = new UserAddressEditModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            model.Address.PrepareModel(address, false, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries());

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(UserAddressEditModel model, int id)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;
            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.Where(a => a.Id == id).FirstOrDefault();
            if (address == null)
                //address is not found
                return RedirectToAction("Addresses");

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                _addressService.UpdateAddress(address);
                return RedirectToAction("Addresses");
            }

            //If we got this far, something failed, redisplay form
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            model.Address.PrepareModel(address, true, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries());
            return View(model);
        }

        #endregion

        #region Change password

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult ChangePassword()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = new ChangePasswordModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.ChangePassword;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.ChangePassword;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(user.Email,
                    true, _userSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _userRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model);
                }
                else
                {
                    foreach (var error in changePasswordResult.Errors)
                        ModelState.AddModelError("", error);
                }
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Avatar
        /// <summary>
        /// 用户头像
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Avatar()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToAction("Info");

            var user = _workContext.CurrentUser;

            var model = new UserAvatarModel();
            model.MaxFileSize = Prettifier.BytesToString(_userSettings.AvatarMaximumSizeBytes);
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Avatar;
            model.AvatarUrl = _pictureService.GetPictureUrl(
                user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("upload-avatar")]
        public ActionResult UploadAvatar(UserAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToAction("Info");

            var user = _workContext.CurrentUser;

            model.MaxFileSize = Prettifier.BytesToString(_userSettings.AvatarMaximumSizeBytes);
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Avatar;

            if (ModelState.IsValid)
            {
                try
                {
                    var userAvatar = _pictureService.GetPictureById(user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId));
                    if ((uploadedFile != null) && (!String.IsNullOrEmpty(uploadedFile.FileName)))
                    {
                        int avatarMaxSize = _userSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.ContentLength > avatarMaxSize)
                            throw new WorkException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), Prettifier.BytesToString(avatarMaxSize)));

                        byte[] userPictureBinary = uploadedFile.GetPictureBits();
                        if (userAvatar != null)
                            userAvatar = _pictureService.UpdatePicture(userAvatar.Id, userPictureBinary, uploadedFile.ContentType, null, true);
                        else
                            userAvatar = _pictureService.InsertPicture(userPictureBinary, uploadedFile.ContentType, null, true);
                    }

                    int userAvatarId = 0;
                    if (userAvatar != null)
                        userAvatarId = userAvatar.Id;

                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AvatarPictureId, userAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                        _mediaSettings.AvatarPictureSize,
                        false);
                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }


            //If we got this far, something failed, redisplay form
            model.AvatarUrl = _pictureService.GetPictureUrl(
                user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);

            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("remove-avatar")]
        public ActionResult RemoveAvatar(UserAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToAction("Info");

            var user = _workContext.CurrentUser;

            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Avatar;

            var userAvatar = _pictureService.GetPictureById(user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId));
            if (userAvatar != null)
                _pictureService.DeletePicture(userAvatar);
            _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AvatarPictureId, 0);

            return RedirectToAction("Avatar");
        }

        #endregion

        #endregion

        #region Password recovery
        /// <summary>
        /// 密码找回
        /// </summary>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            return View(model);
        }
        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        public ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null && user.Active && !user.Deleted)
                {
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                    _workflowMessageService.SendUserPasswordRecoveryMessage(user, _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        /// <summary>
        /// 邮箱验证
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            var cPrt = user.GetAttribute<string>(SystemUserAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return RedirectToRoute("HomePage");

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            var model = new PasswordRecoveryConfirmModel();
            return View(model);
        }
        /// <summary>
        /// 设置新密码
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        public ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            var cPrt = user.GetAttribute<string>(SystemUserAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return RedirectToRoute("HomePage");

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            if (ModelState.IsValid)
            {
                var response = _userRegistrationService.ChangePassword(new ChangePasswordRequest(email,
                    false, _userSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.PasswordRecoveryToken, "");

                    model.SuccessfullyChanged = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        #region Forum subscriptions

        public ActionResult ForumSubscriptions(int? page)
        {
            if (!_forumSettings.AllowUsersToManageSubscriptions)
            {
                return RedirectToAction("Info");
            }

            int pageIndex = 0;
            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var user = _workContext.CurrentUser;

            var pageSize = _forumSettings.ForumSubscriptionsPageSize;

            var list = _forumService.GetAllSubscriptions(user.Id, 0, 0, pageIndex, pageSize);

            var model = new UserForumSubscriptionsModel(list);
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.ForumSubscriptions;

            foreach (var forumSubscription in list)
            {
                var forumTopicId = forumSubscription.TopicId;
                var forumId = forumSubscription.ForumId;
                bool topicSubscription = false;
                var title = string.Empty;
                var slug = string.Empty;

                if (forumTopicId > 0)
                {
                    topicSubscription = true;
                    var forumTopic = _forumService.GetTopicById(forumTopicId);
                    if (forumTopic != null)
                    {
                        title = forumTopic.Subject;
                        slug = forumTopic.GetSeName();
                    }
                }
                else
                {
                    var forum = _forumService.GetForumById(forumId);
                    if (forum != null)
                    {
                        title = forum.Name;
                        slug = forum.GetSeName();
                    }
                }

                model.ForumSubscriptions.Add(new ForumSubscriptionModel()
                {
                    Id = forumSubscription.Id,
                    ForumTopicId = forumTopicId,
                    ForumId = forumSubscription.ForumId,
                    TopicSubscription = topicSubscription,
                    Title = title,
                    Slug = slug,
                });
            }

            return View(model);
        }

        [HttpPost, ActionName("ForumSubscriptions")]
        public ActionResult ForumSubscriptionsPOST(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("fs", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("fs", "").Trim();
                    int forumSubscriptionId = 0;

                    if (Int32.TryParse(id, out forumSubscriptionId))
                    {
                        var forumSubscription = _forumService.GetSubscriptionById(forumSubscriptionId);
                        if (forumSubscription != null && forumSubscription.UserId == _workContext.CurrentUser.Id)
                        {
                            _forumService.DeleteSubscription(forumSubscription);
                        }
                    }
                }
            }

            return RedirectToAction("ForumSubscriptions");
        }

        public ActionResult DeleteForumSubscription(int id)
        {
            if (id < 1)
                return HttpNotFound();

            var forumSubscription = _forumService.GetSubscriptionById(id);
            if (forumSubscription != null && forumSubscription.UserId == _workContext.CurrentUser.Id)
            {
                _forumService.DeleteSubscription(forumSubscription);
            }

            return RedirectToAction("ForumSubscriptions");
        }

        #endregion

    }
}
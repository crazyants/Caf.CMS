using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.ExportImport;
using CAF.WebSite.Application.Services.Forums;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Plugins;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.ModelBinding;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.Infrastructure.Core.Exceptions;
using CAF.WebSite.Mvc.Admin.Models.Common;
using CAF.Infrastructure.Core.Domain.Directory;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public partial class UserController : AdminControllerBase
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserReportService _userReportService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        // private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly UserSettings _userSettings;
        //  private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IExportManager _exportManager;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ForumSettings _forumSettings;
        private readonly IForumService _forumService;
        // private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly AddressSettings _addressSettings;
        private readonly ISiteService _siteService;
        private readonly IEventPublisher _eventPublisher;
        private readonly PluginMediator _pluginMediator;
        //private readonly IAffiliateService _affiliateService;

        #endregion

        #region Constructors

        public UserController(IUserService userService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IGenericAttributeService genericAttributeService,
            IUserRegistrationService userRegistrationService,
            IUserReportService userReportService, 
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            //RewardPointsSettings rewardPointsSettings,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IAddressService addressService,
            UserSettings userSettings,
            //ITaxService taxService,
            IWorkContext workContext, ISiteContext siteContext,
            IExportManager exportManager,
            IUserActivityService userActivityService,
            IPermissionService permissionService,
            AdminAreaSettings adminAreaSettings,
            IQueuedEmailService queuedEmailService, EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService, ForumSettings forumSettings,
            IForumService forumService,
            //IOpenAuthenticationService openAuthenticationService,
            AddressSettings addressSettings, ISiteService siteService,
            IEventPublisher eventPublisher,
            PluginMediator pluginMediator
            //IAffiliateService affiliateService
            )
        {
            this._userService = userService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._genericAttributeService = genericAttributeService;
            this._userRegistrationService = userRegistrationService;
            this._userReportService = userReportService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            //this._rewardPointsSettings = rewardPointsSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._userSettings = userSettings;
            //this._taxService = taxService;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._exportManager = exportManager;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            this._forumSettings = forumSettings;
            this._forumService = forumService;
            // this._openAuthenticationService = openAuthenticationService;
            this._addressSettings = addressSettings;
            this._siteService = siteService;
            this._eventPublisher = eventPublisher;
            this._pluginMediator = pluginMediator;
            //this._affiliateService = affiliateService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected string GetUserRolesNames(IList<UserRole> userRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < userRoles.Count; i++)
            {
                sb.Append(userRoles[i].Name);
                if (i != userRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        [NonAction]
        protected IList<RegisteredUserReportLineModel> GetReportRegisteredUsersModel()
        {
            var report = new List<RegisteredUserReportLineModel>();
            report.Add(new RegisteredUserReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.7days"),
                Users = _userReportService.GetRegisteredUsersReport(7)
            });

            report.Add(new RegisteredUserReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.14days"),
                Users = _userReportService.GetRegisteredUsersReport(14)
            });
            report.Add(new RegisteredUserReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.month"),
                Users = _userReportService.GetRegisteredUsersReport(30)
            });
            report.Add(new RegisteredUserReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.year"),
                Users = _userReportService.GetRegisteredUsersReport(365)
            });

            return report;
        }

        [NonAction]
        protected IList<UserModel.AssociatedExternalAuthModel> GetAssociatedExternalAuthRecords(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var result = new List<UserModel.AssociatedExternalAuthModel>();
            //foreach (var record in _openAuthenticationService.GetExternalIdentifiersFor(user))
            //{
            //    var method = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(record.ProviderSystemName);
            //    if (method == null)
            //        continue;

            //    result.Add(new UserModel.AssociatedExternalAuthModel()
            //    {
            //        Id = record.Id,
            //        Email = record.Email,
            //        ExternalIdentifier = record.ExternalIdentifier,
            //        AuthMethodName = _pluginMediator.GetLocalizedFriendlyName(method.Metadata, _workContext.WorkingLanguage.Id)
            //    });
            //}

            return result;
        }

        [NonAction]
        protected UserModel PrepareUserModelForList(User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                Email = !String.IsNullOrEmpty(user.Email) ? user.Email : (user.IsGuest() ? _localizationService.GetResource("Admin.Users.Guest") : "".NaIfEmpty()),
                UserName = user.UserName,
                FullName = user.GetFullName(),
                Company = user.GetAttribute<string>(SystemUserAttributeNames.Company),
                Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone),
                ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode),
                UserRoleNames = GetUserRolesNames(user.UserRoles.ToList()),
                Active = user.Active,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc),
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc),
            };
        }

        private void PrepareUserModelForCreate(UserModel model)
        {
            string timeZoneId = (model.TimeZoneId.HasValue() ? model.TimeZoneId : _dateTimeHelper.DefaultSiteTimeZone.Id);

            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.AllowUsersToChangeUserNames = _userSettings.AllowUsersToChangeUserNames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;

            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            {
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == timeZoneId) });
            }

            model.DisplayVatNumber = false;
            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();

            if (model.SelectedUserRoleIds == null)
            {
                model.SelectedUserRoleIds = new int[0];
            }

            model.AllowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;

            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });

                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.CountryId) });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
                    }
                }
            }
        }

        [NonAction]
        protected string ValidateUserRoles(IList<UserRole> userRoles)
        {
            if (userRoles == null)
                throw new ArgumentNullException("userRoles");

            //ensure a user is not added to both 'Guests' and 'Registered' user roles
            //ensure that a user is in at least one required role ('Guests' and 'Registered')
            bool isInGuestsRole = userRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Guests) != null;
            bool isInRegisteredRole = userRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Registered) != null;
            if (isInGuestsRole && isInRegisteredRole)
            {
                //return "The user cannot be in both 'Guests' and 'Registered' user roles";
                return String.Format(_localizationService.GetResource("Admin.Users.CanOnlyBeUserOrGuest"),
                    _userService.GetUserRoleBySystemName(SystemUserRoleNames.Guests).Name,
                    _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Name);
            }
            if (!isInGuestsRole && !isInRegisteredRole)
            {
                //return "Add the user to 'Guests' or 'Registered' user role";
                return String.Format(_localizationService.GetResource("Admin.Users.MustBeUserOrGuest"),
                    _userService.GetUserRoleBySystemName(SystemUserRoleNames.Guests).Name,
                    _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Name);
            }
            //no errors
            return "";
        }

        #endregion

        #region Users

        //ajax
        public ActionResult AllUserRoles(string label, int selectedId)
        {
            var userRoles = _userService.GetAllUserRoles(true);
            if (label.HasValue())
            {
                userRoles.Insert(0, new UserRole { Name = label, Id = 0 });
            }

            var list = from c in userRoles
                       select new
                       {
                           id = c.Id.ToString(),
                           text = c.Name,
                           selected = c.Id == selectedId
                       };

            return new JsonResult { Data = list.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //load registered users by default
            var defaultRoleIds = new[] { _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Id };
            var listModel = new UserListModel()
            {
                UserNamesEnabled = _userSettings.UserNamesEnabled,
                DateOfBirthEnabled = _userSettings.DateOfBirthEnabled,
                CompanyEnabled = _userSettings.CompanyEnabled,
                PhoneEnabled = _userSettings.PhoneEnabled,
                ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled,
                AvailableUserRoles = _userService.GetAllUserRoles(true).Select(cr => cr.ToModel()).ToList(),
                SearchUserRoleIds = defaultRoleIds,
            };
            //var users = _userService.GetAllUsers(null, null, defaultRoleIds, null,
            //    null, null, null, 0, 0, null, null, null,
            //    false, 0, _adminAreaSettings.GridPageSize);
            ////user list
            //listModel.Users = new GridModel<UserModel>
            //{
            //    Data = users.Select(PrepareUserModelForList),
            //    Total = users.TotalCount
            //};
            return View(listModel);
        }

        [HttpPost]
        public ActionResult UserList(DataTablesParam dataTableParam, UserListModel model, [ModelBinderAttribute(typeof(CommaSeparatedModelBinder))] int[] searchUserRoleIds)
        {
            //we use own own binder for searchUserRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var searchDayOfBirth = 0;
            int searchMonthOfBirth = 0;
            if (!String.IsNullOrWhiteSpace(model.SearchDayOfBirth))
                searchDayOfBirth = Convert.ToInt32(model.SearchDayOfBirth);
            if (!String.IsNullOrWhiteSpace(model.SearchMonthOfBirth))
                searchMonthOfBirth = Convert.ToInt32(model.SearchMonthOfBirth);

            var users = _userService.GetAllUsers(null, null,
                searchUserRoleIds, model.SearchEmail, model.SearchUserName,
                model.SearchFirstName, model.SearchLastName,
                searchDayOfBirth, searchMonthOfBirth,
                model.SearchCompany, model.SearchPhone, model.SearchZipPostalCode,
                false, dataTableParam.PageIndex, dataTableParam.PageSize);
            var total = users.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = users.Select(PrepareUserModelForList).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = new UserModel();

            PrepareUserModelForCreate(model);

            //default value
            model.Active = true;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Create(UserModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var cust2 = _userService.GetUserByEmail(model.Email);
                if (cust2 != null)
                    ModelState.AddModelError("", "Email is already registered");
            }
            if (!String.IsNullOrWhiteSpace(model.UserName) & _userSettings.UserNamesEnabled)
            {
                var cust2 = _userService.GetUserByEmail(model.UserName);
                if (cust2 != null)
                    ModelState.AddModelError("", "UserName is already registered");
            }

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();

            if (model.SelectedUserRoleIds != null)
            {
                foreach (var userRole in allUserRoles)
                {
                    if (model.SelectedUserRoleIds.Contains(userRole.Id))
                        newUserRoles.Add(userRole);
                }
            }

            var userRolesError = ValidateUserRoles(newUserRoles);

            if (userRolesError.HasValue())
                ModelState.AddModelError("", userRolesError);

            bool allowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);

            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserGuid = Guid.NewGuid(),
                    Email = model.Email,
                    UserName = model.UserName,
                    AdminComment = model.AdminComment,
                    IsTaxExempt = model.IsTaxExempt,
                    Active = model.Active,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                };
                  user.AddEntitySysParam();
                _userService.InsertUser(user);

                //form fields
                if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                if (_userSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, model.FirstName);
                _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, model.LastName);
                if (_userSettings.DateOfBirthEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, model.DateOfBirth);
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

                //password
                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _userSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            NotifyError(changePassError);
                    }
                }

                //user roles
                if (allowManagingUserRoles)
                {
                    foreach (var userRole in newUserRoles)
                        user.UserRoles.Add(userRole);
                    _userService.UpdateUser(user);
                }

                _eventPublisher.Publish(new ModelBoundEvent(model, user, form));

                //activity log
                _userActivityService.InsertActivity("AddNewUser", _localizationService.GetResource("ActivityLog.AddNewUser"), user.Id);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = user.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareUserModelForCreate(model);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null || user.Deleted)
                //No user found with the specified id
                return RedirectToAction("List");

            var model = new UserModel();
            model.Id = user.Id;
            model.Email = user.Email;
            model.UserName = user.UserName;
            model.AdminComment = user.AdminComment;
            model.IsTaxExempt = user.IsTaxExempt;
            model.Active = user.Active;
            model.TimeZoneId = user.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId);
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.AllowUsersToChangeUserNames = _userSettings.AllowUsersToChangeUserNames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);
            model.VatNumberStatusNote = ((VatNumberStatus)user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId))
                .GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
            model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
            model.LastIpAddress = user.LastIpAddress;
            model.LastVisitedPage = user.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);
            model.AffiliateId = user.AffiliateId;

            if (user.AffiliateId != 0)
            {
                //var affiliate = _affiliateService.GetAffiliateById(user.AffiliateId);
                //if (affiliate != null && affiliate.Address != null)
                //    model.AffiliateFullName = affiliate.Address.GetFullName();
            }

            //form fields
            model.FirstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
            model.LastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);
            model.Gender = user.GetAttribute<string>(SystemUserAttributeNames.Gender);
            model.DateOfBirth = user.GetAttribute<DateTime?>(SystemUserAttributeNames.DateOfBirth);
            model.Company = user.GetAttribute<string>(SystemUserAttributeNames.Company);
            model.StreetAddress = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
            model.StreetAddress2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
            model.ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);
            model.City = user.GetAttribute<string>(SystemUserAttributeNames.City);
            model.CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
            model.StateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId);
            model.Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
            model.Fax = user.GetAttribute<string>(SystemUserAttributeNames.Fax);

            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.Name,
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
                            model.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                    else
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

                }
            }

            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            model.SelectedUserRoleIds = user.UserRoles.Select(cr => cr.Id).ToArray();
            model.AllowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);
            //reward points gistory
            // model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            model.AddRewardPointsValue = 0;
            model.AddRewardPointsMessage = "Some comment here...";
            //external authentication records
            model.AssociatedExternalAuthRecords = GetAssociatedExternalAuthRecords(user);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Edit(UserModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null || user.Deleted)
                //No user found with the specified id
                return RedirectToAction("List");

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);
            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!String.IsNullOrEmpty(userRolesError))
                ModelState.AddModelError("", userRolesError);
            bool allowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);

            if (ModelState.IsValid)
            {
                try
                {
                    user.AdminComment = model.AdminComment;
                    user.IsTaxExempt = model.IsTaxExempt;
                    user.Active = model.Active;
                    //email
                    if (!String.IsNullOrWhiteSpace(model.Email))
                    {
                        _userRegistrationService.SetEmail(user, model.Email);
                    }
                    else
                    {
                        user.Email = model.Email;
                    }

                    //username
                    if (_userSettings.UserNamesEnabled && _userSettings.AllowUsersToChangeUserNames)
                    {
                        if (!String.IsNullOrWhiteSpace(model.UserName))
                        {
                            _userRegistrationService.SetUserName(user, model.UserName);
                        }
                        else
                        {
                            user.UserName = model.UserName;
                        }
                    }

                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        string prevVatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);

                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.VatNumber, model.VatNumber);
                        //set VAT number status
                        if (!String.IsNullOrEmpty(model.VatNumber))
                        {
                            if (!model.VatNumber.Equals(prevVatNumber, StringComparison.InvariantCultureIgnoreCase))
                            {
                                //_genericAttributeService.SaveAttribute(user,
                                //    SystemUserAttributeNames.VatNumberStatusId,
                                //    (int)_taxService.GetVatNumberStatus(model.VatNumber));
                            }
                        }
                        else
                        {
                            _genericAttributeService.SaveAttribute(user,
                                SystemUserAttributeNames.VatNumberStatusId,
                                (int)VatNumberStatus.Empty);
                        }
                    }
                       user.AddEntitySysParam(false, true);
                    _userService.UpdateUser(user);

                    //form fields
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, model.DateOfBirth);
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


                    //user roles
                    if (allowManagingUserRoles)
                    {
                        foreach (var userRole in allUserRoles)
                        {
                            if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                            {
                                //new role
                                if (user.UserRoles.Where(cr => cr.Id == userRole.Id).Count() == 0)
                                    user.UserRoles.Add(userRole);
                            }
                            else
                            {
                                //removed role
                                if (user.UserRoles.Where(cr => cr.Id == userRole.Id).Count() > 0)
                                    user.UserRoles.Remove(userRole);
                            }
                        }
                        _userService.UpdateUser(user);
                    }

                    _eventPublisher.Publish(new ModelBoundEvent(model, user, form));

                    //activity log
                    _userActivityService.InsertActivity("EditUser", _localizationService.GetResource("ActivityLog.EditUser"), user.Id);

                    NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Updated"));
                    return continueEditing ? RedirectToAction("Edit", user.Id) : RedirectToAction("List");
                }
                catch (Exception exc)
                {
                    NotifyError(exc.Message, false);
                }
            }


            //If we got this far, something failed, redisplay form
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.AllowUsersToChangeUserNames = _userSettings.AllowUsersToChangeUserNames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = ((VatNumberStatus)user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId))
                 .GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
            model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
            model.LastIpAddress = model.LastIpAddress;
            model.LastVisitedPage = user.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;
            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.Name,
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
                            model.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                    else
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

                }
            }
            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            model.AllowManagingUserRoles = allowManagingUserRoles;
            //reward points gistory
            //  model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            model.AddRewardPointsValue = 0;
            model.AddRewardPointsMessage = "Some comment here...";
            //external authentication records
            model.AssociatedExternalAuthRecords = GetAssociatedExternalAuthRecords(user);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var changePassRequest = new ChangePasswordRequest(model.Email,
                    false, _userSettings.DefaultPasswordFormat, model.Password);
                var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                if (changePassResult.Success)
                    NotifySuccess(_localizationService.GetResource("Admin.Users.Users.PasswordChanged"));
                else
                    foreach (var error in changePassResult.Errors)
                        NotifyError(error);
            }

            return RedirectToAction("Edit", user.Id);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsValid")]
        public ActionResult MarkVatNumberAsValid(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            _genericAttributeService.SaveAttribute(user,
                SystemUserAttributeNames.VatNumberStatusId,
                (int)VatNumberStatus.Valid);

            return RedirectToAction("Edit", user.Id);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsInvalid")]
        public ActionResult MarkVatNumberAsInvalid(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            _genericAttributeService.SaveAttribute(user,
                SystemUserAttributeNames.VatNumberStatusId,
                (int)VatNumberStatus.Invalid);

            return RedirectToAction("Edit", user.Id);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                _userService.DeleteUser(user);

                //remove newsletter subscriptions (if exists)
                var subscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(user.Email, 0, int.MaxValue, true);

                foreach (var subscription in subscriptions)
                {
                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
                }

                //activity log
                _userActivityService.InsertActivity("DeleteUser", _localizationService.GetResource("ActivityLog.DeleteUser"), user.Id);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
                return RedirectToAction("Edit", new { id = user.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public ActionResult Impersonate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowUserImpersonation))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            //ensure that a non-admin user cannot impersonate as an administrator
            //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
            if (!_workContext.CurrentUser.IsAdmin() && user.IsAdmin())
            {
                NotifyError("A non-admin user cannot impersonate as an administrator");
                return RedirectToAction("Edit", user.Id);
            }

            _genericAttributeService.SaveAttribute<int?>(_workContext.CurrentUser,
                SystemUserAttributeNames.ImpersonatedUserId, user.Id);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public ActionResult SendEmail(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(user.Email))
                    throw new WorkException("User email is empty");
                if (!user.Email.IsEmail())
                    throw new WorkException("User email is not valid");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new WorkException("Email subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new WorkException("Email body is empty");

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new WorkException("Email account can't be loaded");

                var email = new QueuedEmail()
                {
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = user.GetFullName(),
                    To = user.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                _queuedEmailService.InsertQueuedEmail(email);
                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }

        public ActionResult SendPm(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                if (!_forumSettings.AllowPrivateMessages)
                    throw new WorkException("Private messages are disabled");
                if (user.IsGuest())
                    throw new WorkException("User should be registered");
                if (String.IsNullOrWhiteSpace(model.SendPm.Subject))
                    throw new WorkException("PM subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendPm.Message))
                    throw new WorkException("PM message is empty");


                var privateMessage = new PrivateMessage
                {
                    SiteId = _siteContext.CurrentSite.Id,
                    ToUserId = user.Id,
                    FromUserId = _workContext.CurrentUser.Id,
                    Subject = model.SendPm.Subject,
                    Text = model.SendPm.Message,
                    IsDeletedByAuthor = false,
                    IsDeletedByRecipient = false,
                    IsRead = false,
                    CreatedOnUtc = DateTime.UtcNow
                };

                _forumService.InsertPrivateMessage(privateMessage);
                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.SendPM.Sent"));
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }

        #endregion

        #region Reward points history

        //public ActionResult RewardPointsHistorySelect(int userId)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
        //        return AccessDeniedView();

        //    var user = _userService.GetUserById(userId);
        //    if (user == null)
        //        throw new ArgumentException("No user found with the specified id");

        //    var model = new List<UserModel.RewardPointsHistoryModel>();
        //    foreach (var rph in user.RewardPointsHistory.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id))
        //    {
        //        model.Add(new UserModel.RewardPointsHistoryModel()
        //            {
        //                Points = rph.Points,
        //                PointsBalance = rph.PointsBalance,
        //                Message = rph.Message,
        //                CreatedOn = _dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc)
        //            });
        //    }
        //    var gridModel = new GridModel<UserModel.RewardPointsHistoryModel>
        //    {
        //        Data = model,
        //        Total = model.Count
        //    };
        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}

        //[ValidateInput(false)]
        //public ActionResult RewardPointsHistoryAdd(int userId, int addRewardPointsValue, string addRewardPointsMessage)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
        //        return AccessDeniedView();

        //    var user = _userService.GetUserById(userId);
        //    if (user == null)
        //        return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

        //    user.AddRewardPointsHistoryEntry(addRewardPointsValue, addRewardPointsMessage);
        //    _userService.UpdateUser(user);

        //    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region Addresses

        public ActionResult AddressesSelect(int userId, DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var addresses = user.Addresses.OrderByDescending(a => a.CreatedOnUtc).ThenByDescending(a => a.Id).ToList();
            var total = addresses.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = addresses.Select(x =>
                {
                    var model = x.ToModel();
                    var addressHtmlSb = new StringBuilder("<div>");
                    if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(model.Company))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Company));
                    if (_addressSettings.StreetAddressEnabled && !String.IsNullOrEmpty(model.Address1))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address1));
                    if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(model.Address2))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address2));
                    if (_addressSettings.CityEnabled && !String.IsNullOrEmpty(model.City))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.City));
                    if (_addressSettings.StateProvinceEnabled && !String.IsNullOrEmpty(model.StateProvinceName))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.StateProvinceName));
                    if (_addressSettings.ZipPostalCodeEnabled && !String.IsNullOrEmpty(model.ZipPostalCode))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.ZipPostalCode));
                    if (_addressSettings.CountryEnabled && !String.IsNullOrEmpty(model.CountryName))
                        addressHtmlSb.AppendFormat("{0}", Server.HtmlEncode(model.CountryName));
                    addressHtmlSb.Append("</div>");
                    model.AddressHtml = addressHtmlSb.ToString();
                    return model;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };

        }

        public ActionResult AddressDelete(int userId, int addressId, DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var address = user.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            user.RemoveAddress(address);
            _userService.UpdateUser(user);
            //now delete the address record
            _addressService.DeleteAddress(address);
            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Address.AddressDelete") }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddressCreate(int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var model = new UserAddressModel();
            model.Address = new AddressModel();
            model.UserId = userId;
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.ValidateEmailAddress = _addressSettings.ValidateEmailAddress;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressCreate(UserAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

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

                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Addresses.Added"));
                return RedirectToAction("AddressEdit", new { addressId = address.Id, userId = model.UserId });
            }

            //If we got this far, something failed, redisplay form
            model.UserId = user.Id;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
            {
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            }
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                {
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
                }
            }
            else
            {
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            }
            return View(model);
        }

        public ActionResult AddressEdit(int addressId, int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = user.Id });

            var model = new UserAddressModel();
            model.UserId = userId;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.ValidateEmailAddress = _addressSettings.ValidateEmailAddress;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(UserAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = user.Id });

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                _addressService.UpdateAddress(address);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Addresses.Updated"));
                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, userId = model.UserId });
            }

            //If we got this far, something failed, redisplay form
            model.UserId = user.Id;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        #endregion

        //#region Orders

        //[HttpPost, GridAction(EnableCustomBinding = true)]
        //public ActionResult OrderList(int userId, GridCommand command)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
        //        return AccessDeniedView();

        //    var orders = _orderService.SearchOrders(0, userId,
        //        null, null, null, null, null, null, null, null, 0, int.MaxValue);

        //    var model = new GridModel<UserModel.OrderModel>
        //    {
        //        Data = orders.PagedForCommand(command)
        //            .Select(order =>
        //            {
        //                var site = _siteService.GetSiteById(order.SiteId);
        //                var orderModel = new UserModel.OrderModel()
        //                {
        //                    Id = order.Id,
        //                    OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
        //                    PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
        //                    ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
        //                    OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false),
        //                    SiteName = site != null ? site.Name : "Unknown",
        //                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
        //                };
        //                return orderModel;
        //            }),
        //        Total = orders.Count
        //    };

        //    return new JsonResult
        //    {
        //        Data = model
        //    };
        //}

        //#endregion

        //#region Reports

        //public ActionResult Reports()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
        //        return AccessDeniedView();

        //    var model = new UserReportsModel();
        //    //users by number of orders
        //    model.BestUsersByNumberOfOrders = new BestUsersReportModel();
        //    model.BestUsersByNumberOfOrders.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
        //    model.BestUsersByNumberOfOrders.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
        //    model.BestUsersByNumberOfOrders.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
        //    model.BestUsersByNumberOfOrders.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
        //    model.BestUsersByNumberOfOrders.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
        //    model.BestUsersByNumberOfOrders.AvailableShippingStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

        //    //users by order total
        //    model.BestUsersByOrderTotal = new BestUsersReportModel();
        //    model.BestUsersByOrderTotal.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
        //    model.BestUsersByOrderTotal.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
        //    model.BestUsersByOrderTotal.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
        //    model.BestUsersByOrderTotal.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
        //    model.BestUsersByOrderTotal.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
        //    model.BestUsersByOrderTotal.AvailableShippingStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

        //    return View(model);
        //}

        //[GridAction(EnableCustomBinding = true)]
        //public ActionResult ReportBestUsersByOrderTotalList(GridCommand command, BestUsersReportModel model)
        //{
        //    DateTime? startDateValue = (model.StartDate == null) ? null
        //                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

        //    DateTime? endDateValue = (model.EndDate == null) ? null
        //                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

        //    OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
        //    PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
        //    ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;


        //    var items = _userReportService.GetBestUsersReport(startDateValue, endDateValue,
        //        orderStatus, paymentStatus, shippingStatus, 1);
        //    var gridModel = new GridModel<BestUserReportLineModel>
        //    {
        //        Data = items.Select(x =>
        //        {
        //            var m = new BestUserReportLineModel()
        //            {
        //                UserId = x.UserId,
        //                OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
        //                OrderCount = x.OrderCount,
        //            };
        //            var user = _userService.GetUserById(x.UserId);
        //            if (user != null)
        //            {
        //                m.UserName = user.IsGuest()
        //                                     ? _localizationService.GetResource("Admin.Users.Guest")
        //                                     : user.Email;
        //            }
        //            return m;
        //        }),
        //        Total = items.Count
        //    };
        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}
        //[GridAction(EnableCustomBinding = true)]
        //public ActionResult ReportBestUsersByNumberOfOrdersList(GridCommand command, BestUsersReportModel model)
        //{
        //    DateTime? startDateValue = (model.StartDate == null) ? null
        //                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

        //    DateTime? endDateValue = (model.EndDate == null) ? null
        //                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

        //    OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
        //    PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
        //    ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;


        //    var items = _userReportService.GetBestUsersReport(startDateValue, endDateValue,
        //        orderStatus, paymentStatus, shippingStatus, 2);
        //    var gridModel = new GridModel<BestUserReportLineModel>
        //    {
        //        Data = items.Select(x =>
        //        {
        //            var m = new BestUserReportLineModel()
        //            {
        //                UserId = x.UserId,
        //                OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
        //                OrderCount = x.OrderCount,
        //            };
        //            var user = _userService.GetUserById(x.UserId);
        //            if (user != null)
        //            {
        //                m.UserName = user.IsGuest()
        //                                     ? _localizationService.GetResource("Admin.Users.Guest")
        //                                     : user.Email;
        //            }
        //            return m;
        //        }),
        //        Total = items.Count
        //    };
        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}

        [ChildActionOnly]
        public ActionResult ReportRegisteredUsers()
        {
            var model = GetReportRegisteredUsersModel();
            return PartialView(model);
        }
        //[GridAction(EnableCustomBinding = true)]
        //public ActionResult ReportRegisteredUsersList(GridCommand command)
        //{
        //    var model = GetReportRegisteredUsersModel();
        //    var gridModel = new GridModel<RegisteredUserReportLineModel>
        //    {
        //        Data = model,
        //        Total = model.Count
        //    };
        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}

        //#endregion

        //#region Current shopping cart/ wishlist

        //[GridAction(EnableCustomBinding = true)]
        //public ActionResult GetCartList(int userId, int cartTypeId)
        //{
        //    var user = _userService.GetUserById(userId);
        //    var cart = user.GetCartItems((ShoppingCartType)cartTypeId);

        //    var gridModel = new GridModel<ShoppingCartItemModel>()
        //    {
        //        Data = cart.Select(sci =>
        //        {
        //            decimal taxRate;
        //            var site = _siteService.GetSiteById(sci.Item.SiteId);
        //            var sciModel = new ShoppingCartItemModel()
        //            {
        //                Id = sci.Item.Id,
        //                Site = site != null ? site.Name : "Unknown",
        //                ProductId = sci.Item.ProductId,
        //                Quantity = sci.Item.Quantity,
        //                ProductName = sci.Item.Product.Name,
        //                ProductTypeName = sci.Item.Product.GetProductTypeLabel(_localizationService),
        //                ProductTypeLabelHint = sci.Item.Product.ProductTypeLabelHint,
        //                UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Item.Product, _priceCalculationService.GetUnitPrice(sci, true), out taxRate)),
        //                Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Item.Product, _priceCalculationService.GetSubTotal(sci, true), out taxRate)),
        //                UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.Item.UpdatedOnUtc, DateTimeKind.Utc)
        //            };
        //            return sciModel;
        //        }),
        //        Total = cart.Count
        //    };
        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}

        //#endregion

        #region Activity log

        [HttpPost]
        public JsonResult ListActivityLog(DataTablesParam dataTableParam, int userId)
        {
            var activityLog = _userActivityService.GetAllActivities(null, null, userId, 0, dataTableParam.PageIndex, dataTableParam.PageSize);
            var total = activityLog.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = activityLog.Select(x =>
                {
                    var m = new UserModel.ActivityLogModel()
                    {
                        Id = x.Id,
                        ActivityLogTypeName = x.ActivityLogType.Name,
                        Comment = x.Comment,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                    return m;

                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };

        }

        #endregion

        #region Export / Import

        public ActionResult ExportExcelAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            try
            {
                var users = _userService.GetAllUsers(null, null, null, null,
                    null, null, null, 0, 0, null, null, null,
                    false, 0, int.MaxValue);

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportUsersToXlsx(stream, users);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "users.xlsx");
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var users = new List<User>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                users.AddRange(_userService.GetUsersByIds(ids));
            }

            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                _exportManager.ExportUsersToXlsx(stream, users);
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "users.xlsx");
        }

        public ActionResult ExportXmlAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            try
            {
                var users = _userService.GetAllUsers(null, null, null, null,
                    null, null, null, 0, 0, null, null, null,
                    false, 0, int.MaxValue);

                var xml = _exportManager.ExportUsersToXml(users);
                return new XmlDownloadResult(xml, "users.xml");
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var users = new List<User>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                users.AddRange(_userService.GetUsersByIds(ids));
            }

            var xml = _exportManager.ExportUsersToXml(users);
            return new XmlDownloadResult(xml, "users.xml");
        }

        #endregion
    }
}

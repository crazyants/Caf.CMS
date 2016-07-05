//Contributor:  Nicholas Mayne

using System;

using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Utilities;

namespace CAF.WebSite.Application.Services.Authentication.External
{
    public partial class ExternalAuthorizer : IExternalAuthorizer
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly UserSettings _userSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        #endregion

        #region Ctor

        public ExternalAuthorizer(IAuthenticationService authenticationService,
            IOpenAuthenticationService openAuthenticationService,
            IGenericAttributeService genericAttributeService,
            IUserRegistrationService userRegistrationService,
            IUserActivityService userActivityService, ILocalizationService localizationService,
            IWorkContext workContext, UserSettings userSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IWorkflowMessageService workflowMessageService, LocalizationSettings localizationSettings)
        {
            this._authenticationService = authenticationService;
            this._openAuthenticationService = openAuthenticationService;
            this._genericAttributeService = genericAttributeService;
            this._userRegistrationService = userRegistrationService;
            this._userActivityService = userActivityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._userSettings = userSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        private bool RegistrationIsEnabled()
        {
            return _userSettings.UserRegistrationType != UserRegistrationType.Disabled && !_externalAuthenticationSettings.AutoRegisterEnabled;
        }

        private bool AutoRegistrationIsEnabled()
        {
            return _userSettings.UserRegistrationType != UserRegistrationType.Disabled && _externalAuthenticationSettings.AutoRegisterEnabled;
        }

        private bool AccountDoesNotExistAndUserIsNotLoggedOn(User userFound, User userLoggedIn)
        {
            return userFound == null && userLoggedIn == null;
        }

        private bool AccountIsAssignedToLoggedOnAccount(User userFound, User userLoggedIn)
        {
            return userFound.Id.Equals(userLoggedIn.Id);
        }

        private bool AccountAlreadyExists(User userFound, User userLoggedIn)
        {
            return userFound != null && userLoggedIn != null;
        }

        #endregion

        #region Methods

        public virtual AuthorizationResult Authorize(OpenAuthenticationParameters parameters)
        {
            var userFound = _openAuthenticationService.GetUser(parameters);

            var userLoggedIn = _workContext.CurrentUser.IsRegistered() ? _workContext.CurrentUser : null;

            if (AccountAlreadyExists(userFound, userLoggedIn))
            {
                if (AccountIsAssignedToLoggedOnAccount(userFound, userLoggedIn))
                {
                    // The person is trying to log in as himself.. bit weird
                    return new AuthorizationResult(OpenAuthenticationStatus.Authenticated);
                }

                var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                result.AddError("Account is already assigned");
                return result;
            }
            if (AccountDoesNotExistAndUserIsNotLoggedOn(userFound, userLoggedIn))
            {
                ExternalAuthorizerHelper.StoreParametersForRoundTrip(parameters);

                if (AutoRegistrationIsEnabled())
                {
                    #region Register user

                    var currentUser = _workContext.CurrentUser;
                    var details = new RegistrationDetails(parameters);
                    var randomPassword = CommonHelper.GenerateRandomDigitCode(20);


                    bool isApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard;
                    var registrationRequest = new UserRegistrationRequest(currentUser, details.EmailAddress,
                        _userSettings.UserNamesEnabled ? details.UserName : details.EmailAddress, randomPassword, PasswordFormat.Clear, isApproved);
                    var registrationResult = _userRegistrationService.RegisterUser(registrationRequest);
                    if (registrationResult.Success)
                    {
                        //store other parameters (form fields)
                        if (!String.IsNullOrEmpty(details.FirstName))
                            _genericAttributeService.SaveAttribute(currentUser, SystemUserAttributeNames.FirstName, details.FirstName);
                        if (!String.IsNullOrEmpty(details.LastName))
                            _genericAttributeService.SaveAttribute(currentUser, SystemUserAttributeNames.LastName, details.LastName);


                        userFound = currentUser;
                        _openAuthenticationService.AssociateExternalAccountWithUser(currentUser, parameters);
                        ExternalAuthorizerHelper.RemoveParameters();

                        //code below is copied from UserController.Register method

                        //authenticate
                        if (isApproved)
                            _authenticationService.SignIn(userFound ?? userLoggedIn, false);

                        //notifications
                        if (_userSettings.NotifyNewUserRegistration)
                            _workflowMessageService.SendUserRegisteredNotificationMessage(currentUser, _localizationSettings.DefaultAdminLanguageId);

                        switch (_userSettings.UserRegistrationType)
                        {
                            case UserRegistrationType.EmailValidation:
                                {
                                    //email validation message
                                    _genericAttributeService.SaveAttribute(currentUser, SystemUserAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                    _workflowMessageService.SendUserEmailValidationMessage(currentUser, _workContext.WorkingLanguage.Id);

                                    //result
                                    return new AuthorizationResult(OpenAuthenticationStatus.AutoRegisteredEmailValidation);
                                }
                            case UserRegistrationType.AdminApproval:
                                {
                                    //result
                                    return new AuthorizationResult(OpenAuthenticationStatus.AutoRegisteredAdminApproval);
                                }
                            case UserRegistrationType.Standard:
                                {
                                    //send user welcome message
                                    _workflowMessageService.SendUserWelcomeMessage(currentUser, _workContext.WorkingLanguage.Id);

                                    //result
                                    return new AuthorizationResult(OpenAuthenticationStatus.AutoRegisteredStandard);
                                }
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ExternalAuthorizerHelper.RemoveParameters();

                        var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                        foreach (var error in registrationResult.Errors)
                            result.AddError(string.Format(error));
                        return result;
                    }

                    #endregion
                }
                else if (RegistrationIsEnabled())
                {
                    return new AuthorizationResult(OpenAuthenticationStatus.AssociateOnLogon);
                }
                else
                {
                    ExternalAuthorizerHelper.RemoveParameters();

                    var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                    result.AddError("Registration is disabled");
                    return result;
                }
            }
            if (userFound == null)
            {
                _openAuthenticationService.AssociateExternalAccountWithUser(userLoggedIn, parameters);
            }


            //authenticate
            _authenticationService.SignIn(userFound ?? userLoggedIn, false);
            //activity log
            _userActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"),
                userFound ?? userLoggedIn);

            return new AuthorizationResult(OpenAuthenticationStatus.Authenticated);
        }

        #endregion
    }
}

using CAF.WebSite.Application.Services.Common;

using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Exceptions;
using System;
using System.Linq;


namespace CAF.WebSite.Application.Services.Users
{
    /// <summary>
    /// User registration service
    /// </summary>
    public partial class UserRegistrationService : IUserRegistrationService
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ILocalizationService _localizationService;
        private readonly UserSettings _userSettings;
        private readonly IEncryptionService _encryptionService;
        private readonly ISerialRuleService _serialRuleService;
        private readonly IDbContext _dbContext;	 // codehint: sm-add
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="UserService">User service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="newsLetterSubscriptionService">Newsletter subscription service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="UserSettings">User settings</param>
        public UserRegistrationService(IUserService userService,
            IEncryptionService encryptionService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ISerialRuleService serialRuleService,
            ILocalizationService localizationService, UserSettings userSettings,
            IDbContext dbContext
          )
        {
            this._userService = userService;
            this._encryptionService = encryptionService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._localizationService = localizationService;
            this._userSettings = userSettings;
            this._serialRuleService = serialRuleService;
            this._dbContext = dbContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="usernameOrEmail">UserName or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        public virtual bool ValidateUser(string usernameOrEmail, string password)
        {
            User User = null;
            if (_userSettings.UserNamesEnabled)
                User = _userService.GetUserByUserName(usernameOrEmail);
            else
                User = _userService.GetUserByEmail(usernameOrEmail);

            if (User == null || User.Deleted || !User.Active)
                return false;

            //only registered can login
            if (!User.IsRegistered())
                return false;

            string pwd = "";
            switch (User.PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    pwd = _encryptionService.EncryptText(password);
                    break;
                case PasswordFormat.Hashed:
                    pwd = _encryptionService.CreatePasswordHash(password, User.PasswordSalt, _userSettings.HashedPasswordFormat);
                    break;
                default:
                    pwd = password;
                    break;
            }

            bool isValid = pwd == User.Password;

            //save last login date
            if (isValid)
            {
                User.LastLoginDateUtc = DateTime.UtcNow;
                _userService.UpdateUser(User);
            }
            //else
            //{
            //    User.FailedPasswordAttemptCount++;
            //    UpdateUser(User);
            //}

            return isValid;
        }
         /// <summary>
        /// Register User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual UserRegistrationResult RegisterUser(UserRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.User == null)
                throw new ArgumentException("Can't load current User");

            var result = new UserRegistrationResult();
            if (request.User.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }
            if (request.User.IsBackgroundTaskAccount())
            {
                result.AddError("Background task account can't be registered");
                return result;
            }
            if (request.User.IsRegistered())
            {
                result.AddError("Current User is already registered");
                return result;
            }
            if (String.IsNullOrEmpty(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }
            if (!request.Email.IsEmail())
            {
                result.AddError(_localizationService.GetResource("Common.WrongEmail"));
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.PasswordIsNotProvided"));
                return result;
            }
            if (_userSettings.UserNamesEnabled)
            {
                if (String.IsNullOrEmpty(request.UserName))
                {
                    result.AddError(_localizationService.GetResource("Account.Register.Errors.UserNameIsNotProvided"));
                    return result;
                }
            }

            //validate unique user
            if (_userService.GetUserByEmail(request.Email) != null)
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }
            if (_userSettings.UserNamesEnabled)
            {
                if (_userService.GetUserByUserName(request.UserName) != null)
                {
                    result.AddError(_localizationService.GetResource("Account.Register.Errors.UserNameAlreadyExists"));
                    return result;
                }
            }

            //at this point request is valid
            request.User.UserName = request.UserName;
            request.User.Email = request.Email;
            request.User.PasswordFormat = request.PasswordFormat;

            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    {
                        request.User.Password = request.Password;
                    }
                    break;
                case PasswordFormat.Encrypted:
                    {
                        request.User.Password = _encryptionService.EncryptText(request.Password);
                    }
                    break;
                case PasswordFormat.Hashed:
                    {
                        string saltKey = _encryptionService.CreateSaltKey(5);
                        request.User.PasswordSalt = saltKey;
                        request.User.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _userSettings.HashedPasswordFormat);
                    }
                    break;
                default:
                    break;
            }

            request.User.Active = request.IsApproved;

            // add to 'Registered' role
            var registeredRole = _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered);
            if (registeredRole == null)
                throw new WorkException("'Registered' role could not be loaded");
            request.User.UserRoles.Add(registeredRole);
            //remove from 'Guests' role
            var guestRole = request.User.UserRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Guests);
            if (guestRole != null)
                request.User.UserRoles.Remove(guestRole);

          
            _userService.UpdateUser(request.User);
            return result;
        }

      
        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual PasswordChangeResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new PasswordChangeResult();
            if (String.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var User = _userService.GetUserByEmail(request.Email);
            if (User == null)
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailNotFound"));
                return result;
            }


            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                string oldPwd = "";
                switch (User.PasswordFormat)
                {
                    case PasswordFormat.Encrypted:
                        oldPwd = _encryptionService.EncryptText(request.OldPassword);
                        break;
                    case PasswordFormat.Hashed:
                        oldPwd = _encryptionService.CreatePasswordHash(request.OldPassword, User.PasswordSalt, _userSettings.HashedPasswordFormat);
                        break;
                    default:
                        oldPwd = request.OldPassword;
                        break;
                }

                bool oldPasswordIsValid = oldPwd == User.Password;
                if (!oldPasswordIsValid)
                    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));

                if (oldPasswordIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                switch (request.NewPasswordFormat)
                {
                    case PasswordFormat.Clear:
                        {
                            User.Password = request.NewPassword;
                        }
                        break;
                    case PasswordFormat.Encrypted:
                        {
                            User.Password = _encryptionService.EncryptText(request.NewPassword);
                        }
                        break;
                    case PasswordFormat.Hashed:
                        {
                            string saltKey = _encryptionService.CreateSaltKey(5);
                            User.PasswordSalt = saltKey;
                            User.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey, _userSettings.HashedPasswordFormat);
                        }
                        break;
                    default:
                        break;
                }
                User.PasswordFormat = request.NewPasswordFormat;
                User.ModifiedOnUtc = DateTime.Now;
                //  User.ModifiedOn = _workContext.CurrentUser.UserCode;
                _userService.UpdateUser(User);
            }

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="newEmail">New email</param>
        public virtual void SetEmail(User User, string newEmail)
        {
            if (User == null)
                throw new ArgumentNullException("User");

            newEmail = newEmail.Trim();
            string oldEmail = User.Email;

            if (!newEmail.IsEmail())
                throw new WorkException(_localizationService.GetResource("Account.EmailUserNameErrors.NewEmailIsNotValid"));

            if (newEmail.Length > 100)
                throw new WorkException(_localizationService.GetResource("Account.EmailUserNameErrors.EmailTooLong"));

            var User2 = _userService.GetUserByEmail(newEmail);
            if (User2 != null && User.Id != User2.Id)
                throw new WorkException(_localizationService.GetResource("Account.EmailUserNameErrors.EmailAlreadyExists"));

            User.Email = newEmail;
            _userService.UpdateUser(User);

            //update newsletter subscription (if required)
            if (!String.IsNullOrEmpty(oldEmail) && !oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                var subscriptionOld = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(oldEmail);
                if (subscriptionOld != null)
                {
                    subscriptionOld.Email = newEmail;
                    _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscriptionOld);
                }
            }
        }

        /// <summary>
        /// Sets a User username
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="newUserName">New UserName</param>
        public virtual void SetUserName(User User, string newUserName)
        {
            if (User == null)
                throw new ArgumentNullException("User");

            if (!_userSettings.UserNamesEnabled)
                throw new WorkException("UserNames are disabled");

            if (!_userSettings.AllowUsersToChangeUserNames)
                throw new WorkException("Changing usernames is not allowed");

            newUserName = newUserName.Trim();

            if (newUserName.Length > 100)
                throw new WorkException(_localizationService.GetResource("Account.EmailUserNameErrors.UserNameTooLong"));

            var user2 = _userService.GetUserByUserName(newUserName);
            if (user2 != null && User.Id != user2.Id)
                throw new WorkException(_localizationService.GetResource("Account.EmailUserNameErrors.UserNameAlreadyExists"));

            User.UserName = newUserName;
            _userService.UpdateUser(User);
        }

        #endregion
    }
}
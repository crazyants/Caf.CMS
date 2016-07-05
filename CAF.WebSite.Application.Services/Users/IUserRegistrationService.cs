

using CAF.Infrastructure.Core.Domain.Users;
namespace CAF.WebSite.Application.Services.Users
{
    /// <summary>
    /// User registration interface
    /// </summary>
    public partial interface IUserRegistrationService
    {
        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="usernameOrEmail">UserName or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        bool ValidateUser(string usernameOrEmail, string password);


        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        UserRegistrationResult RegisterUser(UserRegistrationRequest request);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        PasswordChangeResult ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="newEmail">New email</param>
        void SetEmail(User User, string newEmail);

        /// <summary>
        /// Sets a User username
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="newUserName">New UserName</param>
        void SetUserName(User User, string newUserName);
    }
}
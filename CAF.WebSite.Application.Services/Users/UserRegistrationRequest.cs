using CAF.Infrastructure.Core.Domain.Users;
namespace CAF.WebSite.Application.Services.Users
{
    public class UserRegistrationRequest
    {

        public User User { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public PasswordFormat PasswordFormat { get; set; }
        public bool IsApproved { get; set; }

        public UserRegistrationRequest(User User, string email, string username,
            string password, 
            PasswordFormat passwordFormat,
            bool isApproved = true)
        {
            this.User = User;
            this.Email = email;
            this.UserName = username;
            this.Password = password;
            this.PasswordFormat = passwordFormat;
            this.IsApproved = isApproved;
        }

        //public bool IsValid  
        //{
        //    get 
        //    {
        //        return (!CommonHelper.AreNullOrEmpty(
        //                    this.Email,
        //                    this.Password
        //                    ));
        //    }
        //}
    }
}

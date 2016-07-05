
namespace CAF.Infrastructure.Core.Domain.Users
{
    public class UserRegisterEvent
    {
        private readonly User _user;

        public UserRegisterEvent(User user)
        {
            this._user = user;
        }

        public User User
        {
            get { return _user; }
        }
    }

}
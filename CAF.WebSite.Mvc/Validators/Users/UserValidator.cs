using CAF.WebSite.Mvc.Models.Users;
using FluentValidation;


namespace CAF.WebSite.Mvc.Validators.Users
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator()
        {

            RuleFor(x => x.UserName).NotEmpty().WithMessage("用户名必须填写");

        }
    }
}
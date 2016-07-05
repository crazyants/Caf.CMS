using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Users;
using FluentValidation;

namespace CAF.WebSite.Mvc.Admin.Validators.Users
{
	public partial class UserRoleValidator : AbstractValidator<UserRoleModel>
    {
        public UserRoleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Users.UserRoles.Fields.Name.Required"));
        }
    }
}
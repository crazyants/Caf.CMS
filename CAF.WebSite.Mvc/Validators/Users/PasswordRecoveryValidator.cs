using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Models.Users;
using FluentValidation;
namespace CAF.WebSite.Mvc.Validators.Users
{
    public class PasswordRecoveryValidator : AbstractValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.PasswordRecovery.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }}
}
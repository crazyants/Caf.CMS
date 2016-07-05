using FluentValidation;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Messages;

namespace CAF.WebSite.Mvc.Admin.Validators.Messages
{
	public partial class EmailAccountValidator : AbstractValidator<EmailAccountModel>
    {
        public EmailAccountValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            
            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}
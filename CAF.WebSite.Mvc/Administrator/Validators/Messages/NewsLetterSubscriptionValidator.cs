using FluentValidation;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Messages;


namespace CAF.WebSite.Mvc.Admin.Validators.Messages
{
	public partial class NewsLetterSubscriptionValidator : AbstractValidator<NewsLetterSubscriptionModel>
    {
        public NewsLetterSubscriptionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotNull().WithMessage(localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
        }
    }
}
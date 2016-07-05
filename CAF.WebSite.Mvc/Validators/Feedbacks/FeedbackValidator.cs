using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Models.Common;
using CAF.WebSite.Mvc.Models.Feedbacks;
using FluentValidation;


namespace CAF.WebSite.Mvc.Validators.Feedbacks
{
    public class FeedbackValidator : AbstractValidator<FeedbackModel>
    {
        public FeedbackValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.UserEmail).NotEmpty().WithMessage(localizationService.GetResource("Feedbacks.UserEmail.Required"));
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Feedbacks.Title.Required"));
            RuleFor(x => x.Content).NotEmpty().WithMessage(localizationService.GetResource("Feedbacks.Content.Required"));
            RuleFor(x => x.UserName).NotEmpty().WithMessage(localizationService.GetResource("Feedbacks.UserName.Required"));
        }
    }
}
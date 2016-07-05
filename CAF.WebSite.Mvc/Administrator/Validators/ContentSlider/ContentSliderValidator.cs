using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Settings;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.ContentSlider
{
	public partial class ContentSliderSlideValidator : AbstractValidator<ContentSliderSlideModel>
    {
        public ContentSliderSlideValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentSlider.Slide.Title.Required"));
        }
    }

	public partial class ContentSliderButtonValidator : AbstractValidator<ContentSliderButtonModel>
    {
        public ContentSliderButtonValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Url)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentSlider.Slide.ButtonUrl.Required"))
                .When(x => x.Published);
            RuleFor(x => x.Text)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentSlider.Slide.ButtonText.Required"))
                .When(x => x.Published);
        }
    }
}
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Topics;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.Topics
{
	public partial class TopicValidator : AbstractValidator<TopicModel>
    {
        public TopicValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SystemName).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Topics.Fields.SystemName.Required"));
            RuleFor(x => x.TopicTemplateId).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Topics.Fields.TopicTemplateId.Required"));
        }
    }
}
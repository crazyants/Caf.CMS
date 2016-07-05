using FluentValidation;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Messages;

namespace CAF.WebSite.Mvc.Admin.Validators.Messages
{
	public partial class MessageTemplateValidator : AbstractValidator<MessageTemplateModel>
    {
        public MessageTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Body.Required"));
        }
    }
}
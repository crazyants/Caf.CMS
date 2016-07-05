using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Models.Boards;
using FluentValidation;


namespace CAF.WebSite.Mvc.Validators.Boards
{
    public class EditForumTopicValidator : AbstractValidator<EditForumTopicModel>
    {
        public EditForumTopicValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Forum.TopicSubjectCannotBeEmpty"));
            RuleFor(x => x.Text).NotEmpty().WithMessage(localizationService.GetResource("Forum.TextCannotBeEmpty"));
        }
    }
}
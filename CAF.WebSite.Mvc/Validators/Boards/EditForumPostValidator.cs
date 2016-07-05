using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Models.Boards;
using FluentValidation;


namespace CAF.WebSite.Mvc.Validators.Boards
{
    public class EditForumPostValidator : AbstractValidator<EditForumPostModel>
    {
        public EditForumPostValidator(ILocalizationService localizationService)
        {            
            RuleFor(x => x.Text).NotEmpty().WithMessage(localizationService.GetResource("Forum.TextCannotBeEmpty"));
        }
    }
}
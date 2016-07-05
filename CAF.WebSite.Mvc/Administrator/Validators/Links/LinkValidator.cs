using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Links;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.Users
{
	public partial class LinkValidator : AbstractValidator<LinkModel>
    {
        public LinkValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Links.Fields.SystemName.Required"));
        }
    }
}
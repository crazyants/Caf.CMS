using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Domain.Cms.RegionalContents;
using CAF.WebSite.Mvc.Admin.Models.Links;
using CAF.WebSite.Mvc.Admin.Models.RegionalContents;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.Users
{
    public partial class RegionalContentValidator : AbstractValidator<RegionalContentModel>
    {
        public RegionalContentValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SystemName).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.RegionalContents.Fields.SystemName.Required"));

        }
    }
}
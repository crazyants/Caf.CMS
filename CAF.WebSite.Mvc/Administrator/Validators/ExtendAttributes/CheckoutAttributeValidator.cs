using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.ExtendedAttributes;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.ExtendAttributes
{
	public partial class ExtendedAttributeValidator : AbstractValidator<ExtendedAttributeModel>
    {
        public ExtendedAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ExtendedAttributes.Fields.Name.Required"));
            RuleFor(x => x.Title).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ExtendedAttributes.Fields.Title.Required"));
        }
    }
}
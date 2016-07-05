using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.ExtendedAttributes;
using FluentValidation;

namespace CAF.WebSite.Mvc.Admin.Validators.ExtendAttributes
{
	public partial class ExtendedAttributeValueValidator : AbstractValidator<ExtendedAttributeValueModel>
    {
        public ExtendedAttributeValueValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ExtendedAttributes.Values.Fields.Name.Required"));
        }
    }
}
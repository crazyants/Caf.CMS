using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Common;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.Localization
{
	public partial class GenericAttributeValidator : AbstractValidator<GenericAttributeModel>
    {
        public GenericAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Key).NotEmpty().WithMessage(localizationService.GetResource("Admin.Common.GenericAttributes.Fields.Name.Required"));
        }
    }
}
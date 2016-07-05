using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.ModelTemplates;
using CAF.WebSite.Mvc.Admin.Models.Topics;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.ModelTemplate
{
    public partial class ModelTemplateValidator : AbstractValidator<ModelTemplateModel>
    {
        public ModelTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.ModelTemplates.Fields.Name.Required"));
            RuleFor(x => x.ViewPath).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.ModelTemplates.Fields.ViewPath.Required"));
        }
    }
}
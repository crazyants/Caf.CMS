using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using FluentValidation;
 

namespace CAF.WebSite.Mvc.Admin.Validators.Articles
{
    public partial class ArticleTagValidator : AbstractValidator<ArticleTagModel>
    {
        public ArticleTagValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));
        }
    }
}
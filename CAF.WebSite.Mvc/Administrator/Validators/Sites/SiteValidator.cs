using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using FluentValidation;




namespace CAF.WebSite.Mvc.Admin.Validators.Sites
{
	public partial class SiteValidator : AbstractValidator<SiteModel>
	{
        public SiteValidator(ILocalizationService localizationService)
		{
			RuleFor(x => x.Name)
				.NotNull()
				.WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Name.Required"));
			RuleFor(x => x.Url)
				.NotNull()
				.WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Url.Required"));

			RuleFor(x => x.HtmlBodyId).Matches(@"^([A-Za-z])(\w|\-)*$")
				.WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.HtmlBodyId.Validation"));
		}
	}
}
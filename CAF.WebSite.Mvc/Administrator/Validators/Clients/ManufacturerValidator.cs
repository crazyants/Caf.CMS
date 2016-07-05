using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Admin.Models.Clients;
using FluentValidation;
 

namespace CAF.WebSite.Mvc.Admin.Validators.Clients
{
    public partial class ClientValidator : AbstractValidator<ClientModel>
    {
        public ClientValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Clients.Fields.Name.Required"));
        }
    }
}
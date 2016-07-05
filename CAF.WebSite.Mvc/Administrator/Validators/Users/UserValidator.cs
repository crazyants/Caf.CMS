
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Admin.Models.Users;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.Users
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator(ILocalizationService localizationService, UserSettings userSettings)
        {
            //form fields
            if (userSettings.CompanyRequired && userSettings.CompanyEnabled)
                RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.Company.Required"));
            if (userSettings.StreetAddressRequired && userSettings.StreetAddressEnabled)
                RuleFor(x => x.StreetAddress).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.StreetAddress.Required"));
            if (userSettings.StreetAddress2Required && userSettings.StreetAddress2Enabled)
                RuleFor(x => x.StreetAddress2).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.StreetAddress2.Required"));
            if (userSettings.ZipPostalCodeRequired && userSettings.ZipPostalCodeEnabled)
                RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.ZipPostalCode.Required"));
            if (userSettings.CityRequired && userSettings.CityEnabled)
                RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.City.Required"));
            if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
                RuleFor(x => x.Phone).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.Phone.Required"));
            if (userSettings.FaxRequired && userSettings.FaxEnabled)
                RuleFor(x => x.Fax).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.Fax.Required"));
        }
    }
}
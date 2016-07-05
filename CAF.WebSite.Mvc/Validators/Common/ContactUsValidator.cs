using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Models.Common;
using FluentValidation;
 

namespace CAF.WebSite.Mvc.Validators.Common
{
    public class ContactUsValidator : AbstractValidator<ContactUsModel>
    {
        public ContactUsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("ContactUs.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.FullName).NotEmpty().WithMessage(localizationService.GetResource("ContactUs.FullName.Required"));
            RuleFor(x => x.Enquiry).NotEmpty().WithMessage(localizationService.GetResource("ContactUs.Enquiry.Required"));
        }}
}
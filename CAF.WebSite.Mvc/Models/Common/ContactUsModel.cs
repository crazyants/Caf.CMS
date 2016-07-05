using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Validators.Common;
 

namespace CAF.WebSite.Mvc.Models.Common
{
    [Validator(typeof(ContactUsValidator))]
    public partial class ContactUsModel : ModelBase
    {
        [AllowHtml]
        [LangResourceDisplayName("ContactUs.Email")]
        public string Email { get; set; }

        [AllowHtml]
        [LangResourceDisplayName("ContactUs.Enquiry")]
        public string Enquiry { get; set; }

        [AllowHtml]
        [LangResourceDisplayName("ContactUs.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
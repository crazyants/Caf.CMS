
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Users
{
    public partial class LoginModel : ModelBase
    {
        public bool CheckoutAsGuest { get; set; }

        [LangResourceDisplayName("Account.Login.Fields.SiteName")]
        [AllowHtml]
        public string SiteName { get; set; }


        [LangResourceDisplayName("Account.Login.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [LangResourceDisplayName("Account.Login.Fields.Captcha")]
        [AllowHtml]
        public string Captcha { get; set; }

        public bool UserNameEnabled { get; set; }
        [LangResourceDisplayName("Account.Login.Fields.UserName")]
        [AllowHtml]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [LangResourceDisplayName("Account.Login.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [LangResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }

    }
}
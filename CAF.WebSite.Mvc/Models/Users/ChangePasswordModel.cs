using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Validators.Users;
using CAF.WebSite.Application.WebUI;
 

namespace CAF.WebSite.Mvc.Models.Users
{
    [Validator(typeof(ChangePasswordValidator))]
    public partial class ChangePasswordModel : ModelBase
    {
        [AllowHtml]
        [DataType(DataType.Password)]
        [LangResourceDisplayName("Account.ChangePassword.Fields.OldPassword")]
        public string OldPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [LangResourceDisplayName("Account.ChangePassword.Fields.NewPassword")]
        public string NewPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [LangResourceDisplayName("Account.ChangePassword.Fields.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        public string Result { get; set; }

        public UserNavigationModel NavigationModel { get; set; }

    }
}
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Users;
 

namespace CAF.WebSite.Mvc.Admin.Models.Users
{
    [Validator(typeof(UserRoleValidator))]
    public class UserRoleModel : EntityModelBase
    {
        public UserRoleModel()
        {
            TaxDisplayTypes = new List<SelectListItem>();
        }

        [LangResourceDisplayName("Admin.Users.UserRoles.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Users.UserRoles.Fields.FreeShipping")]
        [AllowHtml]
        public bool FreeShipping { get; set; }

        [LangResourceDisplayName("Admin.Users.UserRoles.Fields.TaxExempt")]
        public bool TaxExempt { get; set; }

        [LangResourceDisplayName("Admin.Users.UserRoles.Fields.TaxDisplayType")]
        public int? TaxDisplayType { get; set; }
        public List<SelectListItem> TaxDisplayTypes { get; set; }

        [LangResourceDisplayName("Admin.Users.UserRoles.Fields.Active")]
        public bool Active { get; set; }

        [LangResourceDisplayName("Admin.Users.UserRoles.Fields.IsSystemRole")]
        public bool IsSystemRole { get; set; }

        [LangResourceDisplayName("Admin.Users.UserRoles.Fields.SystemName")]
        public string SystemName { get; set; }
    }
}
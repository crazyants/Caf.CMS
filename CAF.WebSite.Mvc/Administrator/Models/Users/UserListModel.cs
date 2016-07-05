using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Users
{
    public class UserListModel : ModelBase
    {
        public IEnumerable<UserModel> Users { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        [AllowHtml]
        public List<UserRoleModel> AvailableUserRoles { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        public int[] SearchUserRoleIds { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.List.SearchUserName")]
        [AllowHtml]
        public string SearchUserName { get; set; }
        public bool UserNamesEnabled { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.List.SearchFirstName")]
        [AllowHtml]
        public string SearchFirstName { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.List.SearchLastName")]
        [AllowHtml]
        public string SearchLastName { get; set; }


        [LangResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchDayOfBirth { get; set; }
        [LangResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchMonthOfBirth { get; set; }
        public bool DateOfBirthEnabled { get; set; }



        [LangResourceDisplayName("Admin.Users.Users.List.SearchCompany")]
        [AllowHtml]
        public string SearchCompany { get; set; }
        public bool CompanyEnabled { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.List.SearchPhone")]
        [AllowHtml]
        public string SearchPhone { get; set; }
        public bool PhoneEnabled { get; set; }

        [LangResourceDisplayName("Admin.Users.Users.List.SearchZipCode")]
        [AllowHtml]
        public string SearchZipPostalCode { get; set; }
        public bool ZipPostalCodeEnabled { get; set; }
    }
}
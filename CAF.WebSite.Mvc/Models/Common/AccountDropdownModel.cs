using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Models.Common
{
    public partial class AccountDropdownModel : EntityModelBase
    {
        public string Name { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool DisplayAdminLink { get; set; }
        public bool ShoppingCartEnabled { get; set; }
        public int ShoppingCartItems { get; set; }

        public bool WishlistEnabled { get; set; }
        public int WishlistItems { get; set; }

        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }

        [LangResourceDisplayName("Account.Login.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public bool UserNamesEnabled { get; set; }
        [LangResourceDisplayName("Account.Login.Fields.UserName")]
        [AllowHtml]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [LangResourceDisplayName("Account.Login.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [LangResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }
    }
}
using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.Common
{
    public partial class SiteBarModel : ModelBase
    {
        public bool IsAuthenticated { get; set; }
        public string UserEmailUserName { get; set; }
        public bool IsUserImpersonated { get; set; }


        public bool DisplayAdminLink { get; set; }

        public bool ShoppingCartEnabled { get; set; }
        public int ShoppingCartItems { get; set; }
        public string ShoppingCartAmount { get; set; }

        public bool WishlistEnabled { get; set; }
        public int WishlistItems { get; set; }

        public bool CompareProductsEnabled { get; set; }
        public int CompareItems { get; set; }

        //TODO: werden nicht benötigt raus damit 
        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
        public string UserAvatar { get; set; }
    }
}
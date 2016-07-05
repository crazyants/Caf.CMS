using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Models.Common;

namespace CAF.WebSite.Mvc.Admin.Models.Users
{
    public class UserAddressModel : ModelBase
    {
        public int UserId { get; set; }

        public AddressModel Address { get; set; }
    }
}
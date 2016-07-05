using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Common;
using System.Collections.Generic;
 

namespace CAF.WebSite.Mvc.Models.Users
{
    public partial class UserAddressListModel : ModelBase
    {
        public UserAddressListModel()
        {
            Addresses = new List<AddressModel>();
        }

        public IList<AddressModel> Addresses { get; set; }
        public UserNavigationModel NavigationModel { get; set; }
    }
}
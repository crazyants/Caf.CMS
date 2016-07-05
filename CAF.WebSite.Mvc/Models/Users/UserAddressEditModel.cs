using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Common;
 
 

namespace CAF.WebSite.Mvc.Models.Users
{
    public partial class UserAddressEditModel : ModelBase
    {
        public UserAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        public AddressModel Address { get; set; }
        public UserNavigationModel NavigationModel { get; set; }
    }
}
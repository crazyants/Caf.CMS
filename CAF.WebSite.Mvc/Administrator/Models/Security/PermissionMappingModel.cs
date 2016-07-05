using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Models.Users;
using System.Collections.Generic;
 


namespace CAF.WebSite.Mvc.Admin.Security
{
    public class PermissionMappingModel : ModelBase
    {
        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecordModel>();
            AvailableUserRoles = new List<UserRoleModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }
        public IList<PermissionRecordModel> AvailablePermissions { get; set; }
        public IList<UserRoleModel> AvailableUserRoles { get; set; }

        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }
    }
}
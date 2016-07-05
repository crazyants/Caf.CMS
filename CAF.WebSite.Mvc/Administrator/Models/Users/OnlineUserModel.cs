using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;

namespace CAF.WebSite.Mvc.Admin.Models.Users
{
    public class OnlineUserModel : EntityModelBase
    {
        [LangResourceDisplayName("Admin.Users.OnlineUsers.Fields.UserInfo")]
        public string UserInfo { get; set; }

        [LangResourceDisplayName("Admin.Users.OnlineUsers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [LangResourceDisplayName("Admin.Users.OnlineUsers.Fields.Location")]
        public string Location { get; set; }

        [LangResourceDisplayName("Admin.Users.OnlineUsers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }
        
        [LangResourceDisplayName("Admin.Users.OnlineUsers.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }
    }
}
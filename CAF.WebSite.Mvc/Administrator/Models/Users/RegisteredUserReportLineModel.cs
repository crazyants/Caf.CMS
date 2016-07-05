

using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Admin.Models.Users
{
    public class RegisteredUserReportLineModel : ModelBase
    {
        [LangResourceDisplayName("Admin.Users.Reports.RegisteredUsers.Fields.Period")]
        public string Period { get; set; }

        [LangResourceDisplayName("Admin.Users.Reports.RegisteredUsers.Fields.Users")]
        public int Users { get; set; }
    }
}
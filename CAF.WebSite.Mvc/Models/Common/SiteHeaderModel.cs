using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.Common
{
    public partial class SiteHeaderModel : ModelBase
    {
        public bool LogoUploaded { get; set; }
        public string LogoUrl { get; set; }
        public int LogoWidth { get; set; }
        public int LogoHeight { get; set; }
        public string LogoTitle { get; set; }
    }
}
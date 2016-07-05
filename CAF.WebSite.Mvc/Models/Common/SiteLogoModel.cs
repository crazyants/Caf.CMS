

using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.Common
{
    public partial class FaviconModel : ModelBase
    {
        public bool Uploaded { get; set; }
        public string FaviconUrl { get; set; }
    }
}
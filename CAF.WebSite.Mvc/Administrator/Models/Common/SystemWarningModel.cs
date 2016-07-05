using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Admin.Models.Common
{
    public class SystemWarningModel : ModelBase
    {
        public SystemWarningLevel Level { get; set; }

        public string Text { get; set; }
    }

    public enum SystemWarningLevel
    {
        Pass,
        Warning,
        Fail
    }
}
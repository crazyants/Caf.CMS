

using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.Common
{
    public partial class LanguageModel : EntityModelBase
    {
        public string Name { get; set; }
        public string ISOCode { get; set; }
        public string SeoCode { get; set; }
        public string NativeName { get; set; }
        public string FlagImageFileName { get; set; }

    }
}


using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.QQAuth.Models
{
    public class ConfigurationModel : ModelBase
    {
        [LangResourceDisplayName("Plugins.ExternalAuth.QQ.ClientKeyIdentifier")]
        public string ClientKeyIdentifier { get; set; }

        [LangResourceDisplayName("Plugins.ExternalAuth.QQ.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}
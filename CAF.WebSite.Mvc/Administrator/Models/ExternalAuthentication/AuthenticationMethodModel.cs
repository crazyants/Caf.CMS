using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Web.Mvc;
using System.Web.Routing;
 

namespace CAF.WebSite.Mvc.Admin.Models.ExternalAuthentication
{
	public class AuthenticationMethodModel : ProviderModel, IActivatable
    {
        [LangResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.IsActive")]
        public bool IsActive { get; set; }
    }
}
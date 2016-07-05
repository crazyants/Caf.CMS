

using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Security;
namespace CAF.WebSite.Mvc.Controllers
{

  //  [UserLastActivity]
   // [SiteIpAddress]
    //[SiteLastVisitedPage]
    //[SiteClosedAttribute]
    [LanguageSeoCodeAttribute]
    [RequireHttpsByConfigAttribute(SslRequirement.Retain)]
    //压缩 HTML
    [WhitespaceFilterAttribute]
    public abstract partial class PublicControllerBase : BaseController
    {
    }
}

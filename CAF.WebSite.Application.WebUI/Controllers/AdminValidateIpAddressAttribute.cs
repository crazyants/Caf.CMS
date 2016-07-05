using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Security;
using System;
using System.Web;
using System.Web.Mvc;


namespace CAF.WebSite.Application.WebUI.Controllers
{
    public class AdminValidateIpAddressAttribute : ActionFilterAttribute
    {

        public Lazy<IWebHelper> WebHelper { get; set; }
        public Lazy<SecuritySettings> SecuritySettings { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            bool ok = false;
            var ipAddresses = SecuritySettings.Value.AdminAreaAllowedIpAddresses;
            if (ipAddresses != null && ipAddresses.Count > 0)
            {
                var webHelper = WebHelper.Value;
                foreach (string ip in ipAddresses)
                    if (ip.Equals(webHelper.GetCurrentIpAddress(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        ok = true;
                        break;
                    }
            }
            else
            {
                //no restrictions
                ok = true;
            }

            if (!ok)
            {
                //ensure that it's not 'Access denied' page
                var webHelper = WebHelper.Value;
                var thisPageUrl = webHelper.GetThisPageUrl(false);
                if (!thisPageUrl.StartsWith(string.Format("{0}admin/security/accessdenied", webHelper.GetSiteLocation()), StringComparison.InvariantCultureIgnoreCase))
                {
                    //redirect to 'Access denied' page
                    filterContext.Result = new RedirectResult(webHelper.GetSiteLocation() + "admin/security/accessdenied");
                }
            }
        }
    }
}

using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Security;
using System;
using System.Web.Mvc;


namespace CAF.WebSite.Application.WebUI.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsByConfigAttribute : FilterAttribute, IAuthorizationFilter
    {
        public RequireHttpsByConfigAttribute(SslRequirement sslRequirement)
        {
            this.SslRequirement = sslRequirement;
        }

        public Lazy<SecuritySettings> SecuritySettings { get; set; }
        public Lazy<ISiteContext> SiteContext { get; set; }
        public Lazy<IWebHelper> WebHelper { get; set; }
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            // don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            // only redirect for GET requests, 
            // otherwise the browser might not propagate the verb and request body correctly.
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            if (!DataSettings.DatabaseIsInstalled())
                return;

            var currentConnectionSecured = filterContext.HttpContext.Request.IsSecureConnection();

            var securitySettings = SecuritySettings.Value;
            if (securitySettings.ForceSslForAllPages)
            {
                // all pages are forced to be SSL no matter of the specified value
                this.SslRequirement = SslRequirement.Yes;
            }

            switch (this.SslRequirement)
            {
                case SslRequirement.Yes:
                    {
                        if (!currentConnectionSecured)
                        {
                            var storeContext = SiteContext.Value;
                            var currentSite = storeContext.CurrentSite;

                            if (currentSite != null && currentSite.GetSecurityMode() > HttpSecurityMode.Unsecured)
                            {
                                // redirect to HTTPS version of page
                                // string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                                var webHelper = WebHelper.Value;
                                string url = webHelper.GetThisPageUrl(true, true);
                                filterContext.Result = new RedirectResult(url, true);
                            }
                        }
                    }
                    break;
                case SslRequirement.No:
                    {
                        if (currentConnectionSecured)
                        {
                            var webHelper = WebHelper.Value;

                            // redirect to HTTP version of page
                            // string url = "http://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                            string url = webHelper.GetThisPageUrl(true, false);
                            filterContext.Result = new RedirectResult(url, true);
                        }
                    }
                    break;
                case SslRequirement.Retain:
                    {
                        //do nothing
                    }
                    break;
                default:
                    throw new WorkException("Unsupported SslRequirement parameter");
            }
        }

        public SslRequirement SslRequirement { get; set; }
    }
}

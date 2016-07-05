using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Seo;
using System;
using System.Web.Mvc;
 

namespace CAF.WebSite.Application.WebUI.Seo
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CanonicalHostNameAttribute : FilterAttribute, IAuthorizationFilter
    {
		public CanonicalHostNameAttribute()
        {
        }

		public Lazy<SeoSettings> SeoSettings { get; set; }
		public Lazy<IWebHelper> WebHelper { get; set; }
		public Lazy<ISiteContext> SiteContext { get; set; }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (filterContext.IsChildAction)
                return;
            
            // only redirect for GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

			if (!DataSettings.DatabaseIsInstalled())
                return;

			var rule = SeoSettings.Value.CanonicalHostNameRule;
			if (rule == CanonicalHostNameRule.NoRule)
				return;

			var uri = filterContext.HttpContext.Request.Url;

			if (uri.IsLoopback)
			{
				// allows testing of "localtest.me"
				return;
			}

			var url = uri.ToString();
			var protocol = uri.Scheme.ToLower();
			var isHttps = protocol.IsCaseInsensitiveEqual("https");
			var startsWith = "{0}://www.".FormatInvariant(protocol);
			var hasPrefix = url.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase);

			if (isHttps)
			{
				var securityMode = SiteContext.Value.CurrentSite.GetSecurityMode();
				if (securityMode == HttpSecurityMode.SharedSsl)
				{
					// Don't attempt to redirect when shared SSL is being used and current request is secured.
					// Redirecting from https://ssl.bla.com to https://www.ssl.bla.com will most probably fail.
					return;
				}
			}

			if (hasPrefix && rule == CanonicalHostNameRule.OmitWww)
			{
				url = url.Replace("://www.", "://");
				filterContext.Result = new RedirectResult(url, true);
			}

			if (!hasPrefix && rule == CanonicalHostNameRule.RequireWww)
			{
				url = url.Replace("{0}://".FormatInvariant(protocol), startsWith);
				filterContext.Result = new RedirectResult(url, true);
			}

        }

    }
}

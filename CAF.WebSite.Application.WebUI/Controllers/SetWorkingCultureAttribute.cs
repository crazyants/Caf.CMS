
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace CAF.WebSite.Application.WebUI.Controllers
{
    /// <summary>
    /// Attribute which determines and sets the working culture
    /// </summary>
    public class SetWorkingCultureAttribute : FilterAttribute, IAuthorizationFilter
    {

		public Lazy<IWorkContext> WorkContext { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

			// don't apply filter to child methods
			if (filterContext.IsChildAction)
				return;

			if (!DataSettings.DatabaseIsInstalled())
                return;

            var workContext = WorkContext.Value;
            var workingLanguage = workContext.WorkingLanguage;

            CultureInfo culture;
            if (workContext.CurrentUser != null && workingLanguage != null)
            {
                culture = new CultureInfo(workingLanguage.LanguageCulture);
            }
            else
            {
                culture = new CultureInfo("en-US");
            }
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

    }
}

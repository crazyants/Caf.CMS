using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI.Security;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Dynamic;
using CAF.WebSite.Application.WebUI.Theming;

namespace CAF.WebSite.Application.WebUI.Controllers
{
    [AdminAuthorize]
    [AdminThemed("Administrator")]
    [RequireHttpsByConfig(SslRequirement.Yes)]
    [AdminValidateIpAddress]
    [UserLastActivity]
   // [SiteIpAddress]
    public abstract class AdminControllerBase : BaseController
    {
        #region Constants
        private const string PERMISSIONS_INFO = "caf.permission.info-{0}-{1}";
        private const string APPSYSTEM_CURRENTPROJECTNAME = "caf.currentappsystemname.info";
        private const string APPSYSTEM_CURRENTPROJECTCODE = "caf.currentappsystemcode.info";

        #endregion

        #region Fields


        #endregion

        #region Ctor
        protected AdminControllerBase()
        {

        }
        #endregion

        #region Property


        #endregion

        #region Methods
        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="requestContext">Request context</param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            var routeData = requestContext.RouteData;
            if (routeData != null && !routeData.DataTokens.ContainsKey("ParentActionViewContext"))
            {
                EngineContext.Current.Resolve<IWorkContext>().IsAdmin = true;
            }
            base.Initialize(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //设置授权属性，然后赋值给ViewBag保存
            // ConvertAuthorizedInfo(filterContext);
            // ViewBag.AuthorizeKey = AuthorizeKey;

            // ViewBag.PerBtns = PermissionButtonList.ToModel();

            ViewBag.Vision = Convert.ToInt64((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
        }



        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        protected virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService, IList<TLocalizedModelLocal> locales) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            AddLocales(languageService, locales, null);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="configure">Configure action</param>
        protected virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService, IList<TLocalizedModelLocal> locales, Action<TLocalizedModelLocal, int> configure) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            foreach (var language in languageService.GetAllLanguages(true))
            {
                var locale = Activator.CreateInstance<TLocalizedModelLocal>();
                locale.LanguageId = language.Id;
                if (configure != null)
                {
                    configure.Invoke(locale, locale.LanguageId);
                }
                locales.Add(locale);
            }
        }

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected ActionResult AccessDeniedView()
        {
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
        }

        /// <summary>
        /// Renders default access denied view as a partial
        /// </summary>
        protected ActionResult AccessDeniedPartialView()
        {
            return PartialView("~/Administration/Views/Security/AccessDenied.cshtml");
        }


        #endregion
    }
}

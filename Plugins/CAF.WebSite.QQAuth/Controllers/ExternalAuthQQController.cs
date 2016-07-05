using System;
using System.Web.Mvc;
using CAF.WebSite.QQAuth.Models;
using CAF.WebSite.QQAuth;
using CAF.WebSite.QQAuth.Core;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.Services.Authentication.External;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Settings;
using CAF.Infrastructure.Core.Exceptions;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.QQAuth.Controllers
{
    public class ExternalAuthQQController : PluginControllerBase
    {
        private readonly IOAuthProviderQQAuthorizer _oAuthProviderQQAuthorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
		private readonly ICommonServices _services;

        public ExternalAuthQQController(
            IOAuthProviderQQAuthorizer oAuthProviderQQAuthorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
			ICommonServices services)
        {
            this._oAuthProviderQQAuthorizer = oAuthProviderQQAuthorizer;
            this._openAuthenticationService = openAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
			this._services = services;
        }

		private bool HasPermission(bool notify = true)
		{
			bool hasPermission = _services.Permissions.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods);

			if (notify && !hasPermission)
				NotifyError(_services.Localization.GetResource("Admin.AccessDenied.Description"));

			return hasPermission;
		}
        
		[AdminAuthorize, ChildActionOnly]
        public ActionResult Configure()
        {
			if (!HasPermission(false))
				return AccessDeniedPartialView();

            var model = new ConfigurationModel();
			int storeScope = this.GetActiveSiteScopeConfiguration(_services.SiteService, _services.WorkContext);
			var settings = _services.Settings.LoadSetting<QQExternalAuthSettings>(storeScope);

            model.ClientKeyIdentifier = settings.ClientKeyIdentifier;
            model.ClientSecret = settings.ClientSecret;

			var storeDependingSettingHelper = new SiteDependingSettingHelper(ViewData);
			storeDependingSettingHelper.GetOverrideKeys(settings, model, storeScope, _services.Settings);
            
            return View(model);
        }

		[HttpPost, AdminAuthorize, ChildActionOnly]
		public ActionResult Configure(ConfigurationModel model, FormCollection form)
        {
			if (!HasPermission(false))
				return Configure();

            if (!ModelState.IsValid)
                return Configure();

			var storeDependingSettingHelper = new SiteDependingSettingHelper(ViewData);
			int storeScope = this.GetActiveSiteScopeConfiguration(_services.SiteService, _services.WorkContext);
			var settings = _services.Settings.LoadSetting<QQExternalAuthSettings>(storeScope);

            settings.ClientKeyIdentifier = model.ClientKeyIdentifier;
            settings.ClientSecret = model.ClientSecret;

			storeDependingSettingHelper.UpdateSettings(settings, form, storeScope, _services.Settings);
			_services.Settings.ClearCache();

			NotifySuccess(_services.Localization.GetResource("Admin.Common.DataSuccessfullySaved"));

			return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo()
        {
            return View();
        }

		[NonAction]
		private ActionResult LoginInternal(string returnUrl, bool verifyResponse)
        {
			var processor = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(Provider.SystemName, _services.SiteContext.CurrentSite.Id);
			if (processor == null || !processor.IsMethodActive(_externalAuthenticationSettings))
			{
				throw new WorkException("QQ module cannot be loaded");
			}

            var viewModel = new LoginModel();
            TryUpdateModel(viewModel);

			var result = _oAuthProviderQQAuthorizer.Authorize(returnUrl, verifyResponse);
            switch (result.AuthenticationStatus)
            {
                case OpenAuthenticationStatus.Error:
                    {
                        if (!result.Success)
                            foreach (var error in result.Errors)
								NotifyError(error);

                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AssociateOnLogon:
                    {
                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AutoRegisteredEmailValidation:
                    {
                        //result
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                    }
                case OpenAuthenticationStatus.AutoRegisteredAdminApproval:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                    }
                case OpenAuthenticationStatus.AutoRegisteredStandard:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                    }
                default:
                    break;
            }

            if (result.Result != null)
				return result.Result;

            return HttpContext.Request.IsAuthenticated ?
				new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/") :
				new RedirectResult(Url.LogOn(returnUrl));
        
        }

		public ActionResult Login(string returnUrl)
		{
			return LoginInternal(returnUrl, false);
		}

		public ActionResult LoginCallback(string returnUrl)
		{
			return LoginInternal(returnUrl, true);
		}
	}
}
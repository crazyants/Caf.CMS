using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Email;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.WebSite.Mvc.Admin.Models.Messages;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
	public class EmailAccountController : AdminControllerBase
	{
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
		private readonly ISiteContext _siteContext;
        private readonly IEmailSender _emailSender;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IPermissionService _permissionService;

		public EmailAccountController(IEmailAccountService emailAccountService,
            ILocalizationService localizationService, ISettingService settingService,
			IEmailSender emailSender, ISiteContext siteContext,
            EmailAccountSettings emailAccountSettings, IPermissionService permissionService)
		{
            this._emailAccountService = emailAccountService;
            this._localizationService = localizationService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailSender = emailSender;
            this._settingService = settingService;
			this._siteContext = siteContext;
            this._permissionService = permissionService;
		}

		public ActionResult List(string id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

			//mark as default email account (if selected)
			if (!String.IsNullOrEmpty(id))
			{
				int defaultEmailAccountId = Convert.ToInt32(id);
				var defaultEmailAccount = _emailAccountService.GetEmailAccountById(defaultEmailAccountId);
				if (defaultEmailAccount != null)
				{
					_emailAccountSettings.DefaultEmailAccountId = defaultEmailAccountId;
					_settingService.SaveSetting(_emailAccountSettings);
				}
			}

			return View();
		}

		[HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccountModels = _emailAccountService.GetAllEmailAccounts()
                                    .Select(x => x.ToModel())
                                    .ToList();
            foreach (var eam in emailAccountModels)
                eam.IsDefaultEmailAccount = eam.Id == _emailAccountSettings.DefaultEmailAccountId;

            var total = emailAccountModels.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = emailAccountModels.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
          
		}

		public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var model = new EmailAccountModel();
            //default values
            model.Port = 25;
			return View(model);
		}
        
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
		public ActionResult Create(EmailAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var emailAccount = model.ToEntity();
                _emailAccountService.InsertEmailAccount(emailAccount);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.EmailAccounts.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

			var emailAccount = _emailAccountService.GetEmailAccountById(id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

			return View(emailAccount.ToModel());
		}
        
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(EmailAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(model.Id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                emailAccount = model.ToEntity(emailAccount);
                _emailAccountService.UpdateEmailAccount(emailAccount);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.EmailAccounts.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = emailAccount.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("sendtestemail")]
        public ActionResult SendTestEmail(EmailAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(model.Id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(model.SendTestEmailTo))
                    throw new WorkException("Enter test email address");


				var to = new EmailAddress(model.SendTestEmailTo);
				var from = new EmailAddress(emailAccount.Email, emailAccount.DisplayName);
				string subject = _siteContext.CurrentSite.Name + ". Testing email functionality.";
                string body = "Email works fine.";

				_emailSender.SendEmail(new SmtpContext(emailAccount), new EmailMessage(to, subject, body, from));

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.EmailAccounts.SendTestEmail.Success"), false);
            }
            catch (Exception exc)
            {
				NotifyError(exc.Message, false);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.EmailAccounts.Deleted"));
            _emailAccountService.DeleteEmailAccount(emailAccount);
            return RedirectToAction("List");
        }
	}
}

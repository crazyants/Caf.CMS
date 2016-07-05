using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.WebSite.Mvc.Admin.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
 

namespace CAF.WebSite.Mvc.Admin.Controllers
{
 
	public class QueuedEmailController : AdminControllerBase
	{
		private readonly IQueuedEmailService _queuedEmailService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

		public QueuedEmailController(IQueuedEmailService queuedEmailService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IPermissionService permissionService)
		{
            this._queuedEmailService = queuedEmailService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
		}

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var model = new QueuedEmailListModel();
            return View(model);
		}

		 
		public ActionResult QueuedEmailList(DataTablesParam dataTableParam, QueuedEmailListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            DateTime? startDateValue = (model.SearchStartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.SearchEndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //var queuedEmails = _queuedEmailService.SearchEmails(model.SearchFromEmail, model.SearchToEmail, 
            //    startDateValue, endDateValue, 
            //    model.SearchLoadNotSent, model.SearchMaxSentTries, true,
            //    dataTableParam.PageIndex, dataTableParam.PageSize);

            var q = new SearchEmailsQuery
            {
                EndTime = endDateValue,
                From = model.SearchFromEmail,
                MaxSendTries = model.SearchMaxSentTries,
                OrderByLatest = true,
                PageIndex = dataTableParam.PageIndex,
                PageSize = dataTableParam.PageSize,
                SendManually = model.SearchSendManually,
                StartTime = startDateValue,
                To = model.SearchToEmail,
                UnsentOnly = model.SearchLoadNotSent
            };
            var queuedEmails = _queuedEmailService.SearchEmails(q);
            var total = queuedEmails.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                sEcho = dataTableParam.sEcho,
                iTotalDisplayRecords = total,
                aaData = queuedEmails.Select(x =>
                {
                    var m = x.ToModel();
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    if (x.SentOnUtc.HasValue)
                        m.SentOn = _dateTimeHelper.ConvertToUserTime(x.SentOnUtc.Value, DateTimeKind.Utc);
                    return m;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
            
		}

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-email-by-number")]
        public ActionResult GoToEmailByNumber(QueuedEmailListModel model)
        {
            var queuedEmail = _queuedEmailService.GetQueuedEmailById(model.GoDirectlyToNumber);
            if (queuedEmail != null)
                return RedirectToAction("Edit", "QueuedEmail", new { id = queuedEmail.Id });
            else
                return List();
        }

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

			var email = _queuedEmailService.GetQueuedEmailById(id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("List");

            var model = email.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(email.CreatedOnUtc, DateTimeKind.Utc);
            if (email.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(email.SentOnUtc.Value, DateTimeKind.Utc);
            return View(model);
		}

        [HttpPost, ActionName("Edit")]
        [FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(QueuedEmailModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var email = _queuedEmailService.GetQueuedEmailById(model.Id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                email = model.ToEntity(email);
                _queuedEmailService.UpdateQueuedEmail(email);

                NotifySuccess(_localizationService.GetResource("Admin.System.QueuedEmails.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = email.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(email.CreatedOnUtc, DateTimeKind.Utc);
            if (email.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(email.SentOnUtc.Value, DateTimeKind.Utc);
            return View(model);
		}

        [HttpPost, ActionName("Edit"), FormValueRequired("requeue")]
        public ActionResult Requeue(QueuedEmailModel queuedEmailModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var queuedEmail = _queuedEmailService.GetQueuedEmailById(queuedEmailModel.Id);
            if (queuedEmail == null)
                //No email found with the specified id
                return RedirectToAction("List");

            var requeuedEmail = new QueuedEmail()
            {
                Priority = queuedEmail.Priority,
                From = queuedEmail.From,
                FromName = queuedEmail.FromName,
                To = queuedEmail.To,
                ToName = queuedEmail.ToName,
                CC = queuedEmail.CC,
                Bcc = queuedEmail.Bcc,
                Subject = queuedEmail.Subject,
                Body = queuedEmail.Body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = queuedEmail.EmailAccountId
            };
            _queuedEmailService.InsertQueuedEmail(requeuedEmail);

            NotifySuccess(_localizationService.GetResource("Admin.System.QueuedEmails.Requeued"));
            return RedirectToAction("Edit", requeuedEmail.Id);
        }

	    [HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

			var email = _queuedEmailService.GetQueuedEmailById(id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("List");

            _queuedEmailService.DeleteQueuedEmail(email);

            NotifySuccess(_localizationService.GetResource("Admin.System.QueuedEmails.Deleted"));
			return RedirectToAction("List");
		}

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var queuedEmails = _queuedEmailService.GetQueuedEmailsByIds(selectedIds.ToArray());
                foreach (var queuedEmail in queuedEmails)
                    _queuedEmailService.DeleteQueuedEmail(queuedEmail);
            }

            return Json(new { Result = true });
        }
	}
}

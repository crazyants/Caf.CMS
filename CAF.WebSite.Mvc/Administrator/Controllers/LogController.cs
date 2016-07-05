using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Admin.Models.Links;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Helpers;
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.WebSite.Mvc.Admin.Models.Plugins;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Mvc.Admin.Models.Logging;
namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class LogController : AdminControllerBase
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;

        private static readonly Dictionary<LogLevel, string> s_logLevelHintMap = new Dictionary<LogLevel, string> 
        { 
            { LogLevel.Fatal, "inverse" },
            { LogLevel.Error, "important" },
            { LogLevel.Warning, "warning" },
            { LogLevel.Information, "info" },
            { LogLevel.Debug, "default" }
        };

        public LogController(IWorkContext workContext,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService)
        {
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var model = new LogListModel();
            model.AvailableLogLevels = LogLevel.Debug.ToSelectList(false).ToList();
            model.AvailableLogLevels.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }


        public ActionResult LogList(DataTablesParam dataTableParam, LogListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            DateTime? createdOnFromValue = (model.CreatedOnFrom == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? createdToFromValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            LogLevel? logLevel = model.LogLevelId > 0 ? (LogLevel?)(model.LogLevelId) : null;


            var logItems = Logger.GetAllLogs(createdOnFromValue, createdToFromValue, model.Message,
                logLevel, dataTableParam.iDisplayStart / dataTableParam.iDisplayLength, dataTableParam.iDisplayLength, model.MinFrequency);

            var result = new DataTablesData
            {
                iTotalRecords = logItems.TotalCount,
                iTotalDisplayRecords = logItems.TotalCount,
                sEcho = dataTableParam.sEcho,
                aaData = logItems.Select(x =>
                {
                    var logModel = new LogModel()
                    {
                        Id = x.Id,
                        LogLevelHint = s_logLevelHintMap[x.LogLevel],
                        LogLevel = x.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                        ShortMessage = x.ShortMessage,
                        FullMessage = x.FullMessage,
                        IpAddress = x.IpAddress,
                        UserId = x.UserId,
                        UserEmail = x.User != null ? x.User.Email : null,
                        PageUrl = x.PageUrl,
                        ReferrerUrl = x.ReferrerUrl,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        Frequency = x.Frequency,
                        ContentHash = x.ContentHash
                    };

                    if (x.UpdatedOnUtc.HasValue)
                        logModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc.Value, DateTimeKind.Utc);

                    return logModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public ActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            Logger.ClearLog();

            NotifySuccess(_localizationService.GetResource("Admin.System.Log.Cleared"));
            return RedirectToAction("List");
        }

        public ActionResult View(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = Logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            var model = new LogModel()
            {
                Id = log.Id,
                LogLevelHint = s_logLevelHintMap[log.LogLevel],
                LogLevel = log.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                ShortMessage = log.ShortMessage,
                FullMessage = log.FullMessage,
                IpAddress = log.IpAddress,
                UserId = log.UserId,
                UserEmail = log.User != null ? log.User.Email : null,
                PageUrl = log.PageUrl,
                ReferrerUrl = log.ReferrerUrl,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc),
                Frequency = log.Frequency,
                ContentHash = log.ContentHash
            };

            if (log.UpdatedOnUtc.HasValue)
                model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(log.UpdatedOnUtc.Value, DateTimeKind.Utc);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = Logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            Logger.DeleteLog(log);


            NotifySuccess(_localizationService.GetResource("Admin.System.Log.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var logItems = Logger.GetLogByIds(selectedIds.ToArray());
                foreach (var logItem in logItems)
                    Logger.DeleteLog(logItem);
            }

            return Json(new { Result = true });
        }

    }
}

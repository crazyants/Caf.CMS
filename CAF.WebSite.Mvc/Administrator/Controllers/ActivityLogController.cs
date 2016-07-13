using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Logging;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public partial class ActivityLogController : AdminControllerBase
    {
        #region Fields

        private readonly IUserActivityService _userActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public ActivityLogController(IUserActivityService userActivityService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._userActivityService = userActivityService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Activity log types

        public ActionResult ListTypes()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //var activityLogTypeModel = _userActivityService.GetAllActivityTypes().Select(x => x.ToModel());
            //var gridModel = new GridModel<ActivityLogTypeModel>
            //{
            //    Data = activityLogTypeModel,
            //    Total = activityLogTypeModel.Count()
            //};
            return View();
        }

        [HttpPost]
        public ActionResult ListTypes(DataTablesParam dataTableParam)
        {
            var activityLogTypeModel = _userActivityService.GetAllActivityTypes().Select(x => x.ToModel());
            var total = activityLogTypeModel.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = activityLogTypeModel.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        [HttpPost]
        public ActionResult SaveTypes(FormCollection formCollection)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var keys = formCollection.AllKeys.Where(c => c.StartsWith("checkBox_")).Select(c => c.Substring(9));
            foreach (var key in keys)
            {
                int id;
                if (Int32.TryParse(key, out id))
                {
                    var activityType = _userActivityService.GetActivityTypeById(id);
                    activityType.Enabled = formCollection["checkBox_" + key].Equals("false") ? false : true;
                    _userActivityService.UpdateActivityType(activityType);
                }

            }
            NotifySuccess(_localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLogType.Updated"));
            return RedirectToAction("ListTypes");
        }

        #endregion

        #region Activity log

        public ActionResult ListLogs()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var activityLogSearchModel = new ActivityLogSearchModel();
            activityLogSearchModel.ActivityLogType.Add(new SelectListItem()
            {
                Value = "0",
                Text = "All"
            });


            foreach (var at in _userActivityService.GetAllActivityTypes()
                .OrderBy(x => x.Name)
                .Select(x =>
                {
                    return new SelectListItem()
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    };
                }))
                activityLogSearchModel.ActivityLogType.Add(at);
            return View(activityLogSearchModel);
        }

        [HttpPost]
        public JsonResult ListLogs(DataTablesParam dataTableParam, ActivityLogSearchModel model)
        {
            DateTime? startDateValue = (model.CreatedOnFrom == null) ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var activityLog = _userActivityService.GetAllActivities(startDateValue, endDateValue, null, model.ActivityLogTypeId, dataTableParam.iDisplayStart / dataTableParam.iDisplayLength, dataTableParam.iDisplayLength);
            var result = new DataTablesData
            {
                iTotalRecords = activityLog.TotalCount,
                sEcho = dataTableParam.sEcho,
                iTotalDisplayRecords = activityLog.TotalCount,
                aaData = activityLog.Select(x =>
                {
                    var m = x.ToModel();
                    m.UserEmail = (x.User != null) ? x.User.UserName : "";
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return m;

                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult AcivityLogDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var activityLog = _userActivityService.GetActivityById(id);
            if (activityLog == null)
                throw new ArgumentException("No activity log found with the specified id");

            _userActivityService.DeleteActivity(activityLog);

            //TODO pass and return current ActivityLogSearchModel
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            _userActivityService.ClearAllActivities();
            return RedirectToAction("ListLogs");
        }

        #endregion

    }
}

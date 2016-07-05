using CAF.Infrastructure.Core;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.Services.Feedbacks;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class FeedbackController : AdminControllerBase
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IFeedbackService _feedbackService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor

        public FeedbackController(
            IWorkContext workContext,
            IFeedbackService feedbackService,
            IPermissionService permissionService,
             ILocalizationService localizationService
             )
        {
            this._workContext = workContext;
            this._feedbackService = feedbackService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;

        }
        #endregion

        #region Comments

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFeedbacks))
                return AccessDeniedView();

            return View();
        }
        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFeedbacks))
                return AccessDeniedView();
            var feedbacks = _feedbackService.GetAllFeedbacks();
            var total = feedbacks.Count;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = feedbacks.PagedForCommand(dataTableParam.PageIndex, dataTableParam.PageSize).Select(x => x.ToModel()).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult Delete(int? filterByArticleId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFeedbacks))
                return AccessDeniedView();
        
            var topic = _feedbackService.GetFeedbackById(id);
            if (topic == null)
                //No topic found with the specified id
                return Json(new { Result = false });

            _feedbackService.DeleteFeedback(topic);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Feedbacks.Deleted"));
            

            return Json(new { Result = true });
        }


        #endregion
    }
}
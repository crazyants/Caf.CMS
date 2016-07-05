
using CAF.Infrastructure.Core;
using CAF.Mvc.JQuery.Datatables;
using CAF.WebSite.Application.Services.Authentication;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using CAF.WebSite.Mvc.Admin.Models.Users;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Controllers;
namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class EditController : AdminControllerBase
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly UserSettings _userSettings;
        private readonly IWebHelper _webHelper;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserActivityService _userActivityService;
        private readonly CaptchaSettings _captchaSettings;
        #endregion

        #region Ctor

        public EditController(
            IAuthenticationService authenticationService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            UserSettings userSettings,
            IWebHelper webHelper,
            IUserRegistrationService userRegistrationService, IUserActivityService userActivityService,
             CaptchaSettings captchaSettings
             )
        {
            this._authenticationService = authenticationService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._userSettings = userSettings;
            this._webHelper = webHelper;
            this._userRegistrationService = userRegistrationService;
            this._userActivityService = userActivityService;
            this._captchaSettings = captchaSettings;
        }
        #endregion

        // GET: Edit
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Index(UserModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserGuid = Guid.NewGuid(),
                    Email = model.Email,
                    UserName = model.UserName,


                    LastActivityDateUtc = DateTime.UtcNow,
                };

                NotifySuccess("添加用户成功！");
                return continueEditing ? RedirectToAction("Edit", new { id = user.Id }) : RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        //public DataTablesResult<ChannelCategoryModel> GetChannelCategorys(DataTablesParam dataTableParam)
        //{
        //    var activityLog = _channelCategoryService.GetAllChannelCategoryQ();
        //    return DataTablesResult.Create(activityLog.Select(a =>
        //   new ChannelCategoryModel()
        //   {
        //       Id = a.Id,
        //       Title = a.Title,
        //       Domain = a.Domain,
        //       BuildPath = a.BuildPath,
        //       SortId = a.SortId,
        //       IsDefault = a.IsDefault
        //   }), dataTableParam,
        //     uv => new
        //     {
        //         Title = "<b>" + uv.Title + "</b>"
        //     }
        //    );
        //}

        //public ActionResult ListPager(int id = 1)
        //{
        //   // var query = _channelCategoryService.GetAllChannelCategoryQ().OrderByDescending(a => a.CreatedOnUtc).ToPagedList(id, 8);
        //   // var result = query.Select(a =>
        //   //new ChannelCategoryModel()
        //   //{
        //   //    Id = a.Id,
        //   //    Title = a.Title,
        //   //    Domain = a.Domain,
        //   //    BuildPath = a.BuildPath,
        //   //    SortId = a.SortId,
        //   //    IsDefault = a.IsDefault
        //   //});
        //   // var model = query.ToPagedList<ChannelCategoryModel, ChannelCategory>(result);
        //   // if (Request.IsAjaxRequest())
        //   //     return PartialView("_AjaxSearchPost", model);
        //    return View(model);
        //}


        //[HttpPost]
        //public ActionResult ListPager(string title, int id = 1)
        //{
        //    return ajaxSearchPostResult(title, id);
        //}

        //private ActionResult ajaxSearchPostResult(string title, int id = 1)
        //{
        //    var qry = _channelCategoryService.GetAllChannelCategoryQ();


        //    if (!string.IsNullOrWhiteSpace(title))
        //        qry = qry.Where(a => a.Title.Contains(title));

        //    var query = qry.OrderByDescending(a => a.CreatedOnUtc).ToPagedList(id, 8);
        //    var result = query.Select(a =>
        //   new ChannelCategoryModel()
        //   {
        //       Id = a.Id,
        //       Title = a.Title,
        //       Domain = a.Domain,
        //       BuildPath = a.BuildPath,
        //       SortId = a.SortId,
        //       IsDefault = a.IsDefault
        //   });
        //    var model = query.ToPagedList<ChannelCategoryModel, ChannelCategory>(result);
        //    if (Request.IsAjaxRequest())
        //        return PartialView("_AjaxSearchPost", model);
        //    return View(model);

        //}
    }
}
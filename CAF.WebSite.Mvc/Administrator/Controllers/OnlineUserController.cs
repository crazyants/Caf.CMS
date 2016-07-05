using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Admin.Models.Users;
using System;
using System.Linq;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class OnlineUserController : AdminControllerBase
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IGeoCountryLookup _geoCountryLookup;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly UserSettings _userSettings;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public OnlineUserController(IUserService userService,
            IGeoCountryLookup geoCountryLookup, IDateTimeHelper dateTimeHelper,
            UserSettings userSettings, AdminAreaSettings adminAreaSettings,
            IPermissionService permissionService, ILocalizationService localizationService)
        {
            this._userService = userService;
            this._geoCountryLookup = geoCountryLookup;
            this._dateTimeHelper = dateTimeHelper;
            this._userSettings = userSettings;
            this._adminAreaSettings = adminAreaSettings;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Online users

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var users = _userService.GetOnlineUsers(DateTime.UtcNow.AddMinutes(-_userSettings.OnlineUserMinutes),
                null, dataTableParam.PageIndex, dataTableParam.PageSize);

            var total = users.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = users.Select(x =>
                {
                    return new OnlineUserModel()
                    {
                        Id = x.Id,
                        UserInfo = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Users.Guest"),
                        LastIpAddress = x.LastIpAddress,
                        Location = _geoCountryLookup.LookupCountryName(x.LastIpAddress),
                        LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc),
                        LastVisitedPage = x.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage)
                    };
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        #endregion
    }
}

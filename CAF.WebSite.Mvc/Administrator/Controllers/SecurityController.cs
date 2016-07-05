using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Users;
using CAF.WebSite.Mvc.Admin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class SecurityController : AdminControllerBase
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public SecurityController(IWorkContext workContext,
            IPermissionService permissionService,
            IUserService userService, ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._userService = userService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        public ActionResult AccessDenied(string pageUrl)
        {
            var currentUser = _workContext.CurrentUser;
            if (currentUser == null || currentUser.IsGuest())
            {
                Logger.Information(string.Format("Access denied to anonymous request on {0}", pageUrl));
                return View();
            }

            Logger.Information(string.Format("Access denied to user #{0} '{1}' on {2}", currentUser.Email, currentUser.Email, pageUrl));


            return View();
        }

        public ActionResult Permissions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            var model = new PermissionMappingModel();

            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var userRoles = _userService.GetAllUserRoles(true);
            foreach (var pr in permissionRecords)
            {
                model.AvailablePermissions.Add(new PermissionRecordModel()
                {
                    Name = pr.Name,
                    SystemName = pr.SystemName
                });
            }
            foreach (var cr in userRoles)
            {
                model.AvailableUserRoles.Add(new UserRoleModel()
                {
                    Id = cr.Id,
                    Name = cr.Name
                });
            }
            foreach (var pr in permissionRecords)
                foreach (var cr in userRoles)
                {
                    bool allowed = pr.UserRoles.Where(x => x.Id == cr.Id).ToList().Count() > 0;
                    if (!model.Allowed.ContainsKey(pr.SystemName))
                        model.Allowed[pr.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[pr.SystemName][cr.Id] = allowed;
                }

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        [FormValueRequired("save-permissions")]
        public ActionResult PermissionsSave(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var userRoles = _userService.GetAllUserRoles(true);


            foreach (var cr in userRoles)
            {
                string formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = form[formKey] != null ? form[formKey].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                foreach (var pr in permissionRecords)
                {

                    bool allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);
                    if (allow)
                    {
                        if (pr.UserRoles.Where(x => x.Id == cr.Id).FirstOrDefault() == null)
                        {
                            pr.UserRoles.Add(cr);
                            _permissionService.UpdatePermissionRecord(pr);
                        }
                    }
                    else
                    {
                        if (pr.UserRoles.Where(x => x.Id == cr.Id).FirstOrDefault() != null)
                        {
                            pr.UserRoles.Remove(cr);
                            _permissionService.UpdatePermissionRecord(pr);
                        }
                    }
                }
            }

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.ACL.Updated"));
            return RedirectToAction("Permissions");
        }

        [HttpPost, ActionName("Permissions")]
        [FormValueRequired("update-permissions")]
        public ActionResult PermissionsUpdate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();
            dynamic provider = Activator.CreateInstance(typeof(StandardPermissionProvider));
            _permissionService.InstallPermissions(provider);
            return RedirectToAction("Permissions");
        }
        #endregion
    }
}

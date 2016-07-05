using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using CAF.Infrastructure.Core;
using CAF.WebSite.Mvc.Admin.Controllers;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Mvc.Admin.Models.Users;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.WebSite.Mvc.Admin.Infrastructure;
using CAF.WebSite.Application.WebUI.Controllers;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class UserRoleController : AdminControllerBase
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        //private readonly TaxSettings _taxSettings;

        #endregion

        #region Constructors

        public UserRoleController(IUserService userService,
            ILocalizationService localizationService, IUserActivityService userActivityService,
            IPermissionService permissionService
            //  TaxSettings taxSettings
            )
        {
            this._userService = userService;
            this._localizationService = localizationService;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            //codehint: sm-add
            // this._taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected List<SelectListItem> GetTaxDisplayTypesList(UserRoleModel model)
        {
            var list = new List<SelectListItem>();

            if (model.TaxDisplayType.HasValue)
            {
                list.Insert(0, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Enums.CafSite.Core.Domain.Tax.TaxDisplayType.IncludingTax"),
                    Value = "0",
                    Selected = (TaxDisplayType)model.TaxDisplayType.Value == TaxDisplayType.IncludingTax
                });
                list.Insert(1, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Enums.CafSite.Core.Domain.Tax.TaxDisplayType.ExcludingTax"),
                    Value = "10",
                    Selected = (TaxDisplayType)model.TaxDisplayType.Value == TaxDisplayType.ExcludingTax
                });
            }
            else
            {
                list.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Enums.CafSite.Core.Domain.Tax.TaxDisplayType.IncludingTax"), Value = "0" });
                list.Insert(1, new SelectListItem() { Text = _localizationService.GetResource("Enums.CafSite.Core.Domain.Tax.TaxDisplayType.ExcludingTax"), Value = "10" });
            }

            return list;
        }

        #endregion

        #region User roles

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUserRoles))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
               if (!_permissionService.Authorize(StandardPermissionProvider.ManageUserRoles))
                   return AccessDeniedView();

            var userRoles = _userService.GetAllUserRoles(true).AsQueryable();

            return DataTablesResult.Create(userRoles.Select(a =>
              new UserRoleModel()
              {
                  Id = a.Id,
                  Name = a.Name,
                  FreeShipping = a.FreeShipping,
                  TaxExempt = a.TaxExempt,
                  TaxDisplayType = a.TaxDisplayType,
                  Active = a.Active,
                  IsSystemRole = a.IsSystemRole,
                  SystemName = a.SystemName
              }), dataTableParam
           );

        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUserRoles))
             return AccessDeniedView();

            var model = new UserRoleModel();
            model.TaxDisplayTypes = GetTaxDisplayTypesList(model);
            //default values
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(UserRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUserRoles))
               return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var userRole = model.ToEntity();
                _userService.InsertUserRole(userRole);

                //activity log
                _userActivityService.InsertActivity("AddNewUserRole", _localizationService.GetResource("ActivityLog.AddNewUserRole"), userRole.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Users.UserRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUserRoles))
               return AccessDeniedView();

            var userRole = _userService.GetUserRoleById(id);
            if (userRole == null)
                //No user role found with the specified id
                return RedirectToAction("List");

            var model = userRole.ToModel();
            model.TaxDisplayTypes = GetTaxDisplayTypesList(model);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(UserRoleModel model, bool continueEditing)
        {
             if (!_permissionService.Authorize(StandardPermissionProvider.ManageUserRoles))
              return AccessDeniedView();

            var userRole = _userService.GetUserRoleById(model.Id);
            if (userRole == null)
                //No user role found with the specified id
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (userRole.IsSystemRole && !model.Active)
                        throw new WorkException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.Active.CantEditSystem"));

                    if (userRole.IsSystemRole && !userRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new WorkException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.SystemName.CantEditSystem"));


                    userRole = model.ToEntity(userRole);
                    _userService.UpdateUserRole(userRole);

                    //activity log
                    _userActivityService.InsertActivity("EditUserRole", _localizationService.GetResource("ActivityLog.EditUserRole"), userRole.Name);

                    NotifySuccess(_localizationService.GetResource("Admin.Users.UserRoles.Updated"));
                    return continueEditing ? RedirectToAction("Edit", userRole.Id) : RedirectToAction("List");
                }

                //If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("Edit", new { id = userRole.Id });
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
              if (!_permissionService.Authorize(StandardPermissionProvider.ManageUserRoles))
               return AccessDeniedView();

            var userRole = _userService.GetUserRoleById(id);
            if (userRole == null)
                //No user role found with the specified id
                return RedirectToAction("List");

            try
            {
                _userService.DeleteUserRole(userRole);

                //activity log
                _userActivityService.InsertActivity("DeleteUserRole", _localizationService.GetResource("ActivityLog.DeleteUserRole"), userRole.Name);

                NotifySuccess(_localizationService.GetResource("Admin.Users.UserRoles.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
                return RedirectToAction("Edit", new { id = userRole.Id });
            }

        }

        #endregion
    }
}

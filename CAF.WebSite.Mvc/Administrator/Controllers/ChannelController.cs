using CAF.Infrastructure.Core;
using CAF.Mvc.JQuery.Datatables;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Authentication;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Captcha;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.MvcCaptcha;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using CAF.WebSite.Mvc.Admin.Models.Users;
using CAF.WebSite.Application.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Security;
namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class ChannelController : AdminControllerBase
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly UserSettings _userSettings;
        private readonly IExtendedAttributeService _extendedAttributeService;
        private readonly IChannelService _channelService;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        #endregion

        #region Ctor

        public ChannelController(
            IModelTemplateService modelTemplateService,
            IAuthenticationService authenticationService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            UserSettings userSettings,
            IChannelService channelService,
            IUserActivityService userActivityService,
            IExtendedAttributeService extendedAttributeService,
              IPermissionService permissionService
             )
        {
            this._modelTemplateService = modelTemplateService;
            this._authenticationService = authenticationService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._userSettings = userSettings;
            this._channelService = channelService;
            this._userActivityService = userActivityService;
            this._extendedAttributeService = extendedAttributeService;
            this._permissionService = permissionService;
        }
        #endregion


        #region Utilities

        private void PrepareChannelModel(ChannelModel model, Channel cannel, bool excludeProperties)
        {

            var extendeds = new List<ExtendedAttribute>();
            if (cannel != null)
            {
                var result = new StringBuilder();
                extendeds = cannel.ExtendedAttributes.ToList();
                for (int i = 0; i < cannel.ExtendedAttributes.Count; i++)
                {
                    var pt = extendeds[i];
                    result.Append(pt.Id);
                    if (i != cannel.ExtendedAttributes.Count - 1)
                        result.Append(", ");
                }
                model.ExtendedAttributes = result.ToString();
            }

            var allExtended = _extendedAttributeService.GetAllExtendedAttributes();
            foreach (var extended in allExtended)
            {
                model.AvailableExtendedAttributes.Add(new SelectListItem() { Text = extended.Title, Value = extended.Id.ToString(), Selected = extendeds.Where(e => e.Id == extended.Id).Any() });
            }
        }

        [NonAction]
        protected void UpdateExtendedAttributes(Channel cannel, string rawExtendedAttributes)
        {
            if (cannel == null)
                throw new ArgumentNullException("cannel");

            var extendedAttributes = new List<string>();

            foreach (string str in rawExtendedAttributes.SplitSafe(","))
            {
                string ext = str.TrimSafe();
                if (ext.HasValue())
                    extendedAttributes.Add(ext);
            }

            var existingExtendedAttributes = cannel.ExtendedAttributes.ToList();
            var extendedAttributeToRemove = new List<ExtendedAttribute>();

            foreach (var existingExtendedAttribute in existingExtendedAttributes)
            {
                bool found = false;
                foreach (string newExtendedAttributes in extendedAttributes)
                {
                    if (existingExtendedAttribute.Id.ToString().Equals(newExtendedAttributes, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    extendedAttributeToRemove.Add(existingExtendedAttribute);
                }
            }

            foreach (var extendedAttribute in extendedAttributeToRemove)
            {
                cannel.ExtendedAttributes.Remove(extendedAttribute);
                _channelService.UpdateChannel(cannel);
            }

            foreach (string extendedAttributeName in extendedAttributes)
            {
                ExtendedAttribute extendedAttribute = null;
                var extendedAttribute2 = _extendedAttributeService.GetExtendedAttributeById(extendedAttributeName.ToInt());

                if (extendedAttribute2 == null)
                {

                }
                else
                {
                    extendedAttribute = extendedAttribute2;
                    if (!cannel.ChannelExtendedAttributeExists(extendedAttribute.Id))
                    {
                        cannel.ExtendedAttributes.Add(extendedAttribute);
                        _channelService.UpdateChannel(cannel);
                    }
                }


            }
        }

        [NonAction]
        protected void PrepareTemplatesModel(ChannelModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var templates = _modelTemplateService.GetAllModelTemplates();
            var listTemplate = templates.Where(p => p.TemplageTypeId == (int)TemplateTypeFormat.List).ToList();
            foreach (var template in listTemplate)
            {
                model.AvailableModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
            var detailTemplate = templates.Where(p => p.TemplageTypeId == (int)TemplateTypeFormat.Detail).ToList();
            foreach (var template in detailTemplate)
            {
                model.AvailableDetailModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
        }
        #endregion Utilities

        #region List
        // GET: Edit
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        public ActionResult List()
        {

            return View();
        }
        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            var channel = _channelService.GetAllChannelQ();

            return DataTablesResult.Create(channel.Select(a =>
               new ChannelModel()
               {
                   Id = a.Id,
                   Name = a.Name,
                   Title = a.Title,
                   DisplayOrder = a.DisplayOrder,


               }), dataTableParam,
             uv => new
             {
                 Title = "<b>" + uv.Title + "</b>"
             }
            );
        }

        #endregion


        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var model = new ChannelModel();
            PrepareChannelModel(model, null, true);
            //templates
            PrepareTemplatesModel(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(ChannelModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                model.ExtendedAttributes = form["ExtendedAttributes"];
                var channel = model.ToEntity();
                channel.AddEntitySysParam();
                _channelService.InsertChannel(channel);
                UpdateExtendedAttributes(channel, model.ExtendedAttributes);
                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = channel.Id }) : RedirectToAction("List");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var channel = _channelService.GetChannelById(id);
            if (channel == null)
                //No channel found with the specified id
                return RedirectToAction("List");

            var model = channel.ToModel();
            PrepareChannelModel(model, channel, true);
            //templates
            PrepareTemplatesModel(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(ChannelModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var channel = _channelService.GetChannelById(model.Id);
            if (channel == null)
                //No channel found with the specified id
                return RedirectToAction("List");
            if (ModelState.IsValid)
            {
                model.ExtendedAttributes = form["ExtendedAttributes"];
                channel = model.ToEntity(channel);
                channel.AddEntitySysParam(false, true);
                _channelService.UpdateChannel(channel);
                UpdateExtendedAttributes(channel, model.ExtendedAttributes);
                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));
                return continueEditing ? RedirectToAction("Edit", channel.Id) : RedirectToAction("List");
            }
            PrepareChannelModel(model, channel, true);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var channel = _channelService.GetChannelById(id);
            if (channel == null)
                //No channel found with the specified id
                return RedirectToAction("List");

            _channelService.DeleteChannel(channel);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));
            return RedirectToAction("List");
        }

        public ActionResult DeleteSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var channels = new List<Channel>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                channels.AddRange(_channelService.GetChannelsByIds(ids));

                for (int i = 0; i < channels.Count; i++)
                {
                    var channel = channels[i];
                    _channelService.DeleteChannel(channel);
                }
            }

            return RedirectToAction("List");
        }
        #endregion

    }
}
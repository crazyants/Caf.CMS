using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Events;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.CustomBanner.Domain;
using CAF.WebSite.CustomBanner.Models;
using CAF.WebSite.CustomBanner.Services;
using CAF.WebSite.CustomBanner.Settings;
using CAF.WebSite.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;

namespace CAF.WebSite.CustomBanner
{
    public class Events : IConsumer<TabStripCreated>, IConsumer<ModelBoundEvent>
    {
        private readonly ICustomBannerService _customBannerService;
        private readonly ISettingService _settingService;
        private readonly ISiteContext _siteContext;
        private readonly IPermissionService _permissionService;
        public Events(ICustomBannerService customBannerService, ISiteContext siteContext, ISettingService settingService, IPermissionService permissionService)
        {
            this._customBannerService = customBannerService;
            this._siteContext = siteContext;
            this._settingService = settingService;
            this._permissionService = permissionService;
        }
        public void HandleEvent(TabStripCreated eventMessage)
        {
            string tabStripName = eventMessage.TabStripName;
            int entityId = ((EntityModelBase)eventMessage.Model).Id;
            string entityName = eventMessage.TabStripName.Substring(0, eventMessage.TabStripName.IndexOf("-"));
            if (tabStripName == "category-edit" || tabStripName == "article-edit")
            {
                eventMessage.ItemFactory.Add().Text("Banner").Name("tab-custombanner").Icon("fa fa-picture-o fa-lg fa-fw").LinkHtmlAttributes(new
                {
                    data_tab_name = "CustomBanner"
                }).Route("CAF.WebSite.CustomBanner", new
                {
                    action = "PictureEditTab",
                    entityId = entityId,
                    entityName = entityName
                }).Ajax(true);
            }
            if (tabStripName == "topic-edit")
            {
                eventMessage.ItemFactory.Add().Text("Banner").Name("tab-custombanner").Icon("fa fa-picture-o fa-lg fa-fw").LinkHtmlAttributes(new
                {
                    data_tab_name = "CustomBanner"
                }).Content(ChildActionExtensions.Action(eventMessage.Html, "PictureEditTab", "CustomBanner", new
                {
                    action = "PictureEditTab",
                    entityId = entityId,
                    entityName = entityName,
                    area = "CAF.WebSite.CustomBanner"
                }).ToHtmlString()).Route("CAF.WebSite.CustomBanner", new
                {
                    action = "PictureEditTab",
                    entityId = entityId,
                    entityName = entityName
                });
            }
        }
        [AdminAuthorize]
        public void HandleEvent(ModelBoundEvent eventMessage)
        {
            if (!eventMessage.BoundModel.CustomProperties.ContainsKey("CustomBanner"))
            {
                return;
            }
            PictureEditTabModel model = eventMessage.BoundModel.CustomProperties["CustomBanner"] as PictureEditTabModel;
            if (model == null)
            {
                return;
            }
            if (!this._permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
            {
                return;
            }
            this._settingService.LoadSetting<CustomBannerSettings>(this._siteContext.CurrentSite.Id);
            DateTime utcNow = DateTime.UtcNow;
            CustomBannerRecord entity = this._customBannerService.GetCustomBannerRecord(model.EntityId, model.EntityName);
            bool insert = entity == null;
            if (entity == null)
            {
                entity = new CustomBannerRecord
                {
                    EntityId = model.EntityId,
                    EntityName = model.EntityName,
                    CreatedOnUtc = utcNow
                };
            }
            entity.AddEntitySysParam(true, true);
            entity.PictureId = model.PictureId;
            if (insert)
            {
                this._customBannerService.InsertCustomBannerRecord(entity);
                return;
            }
            this._customBannerService.UpdateCustomBannerRecord(entity);
        }
    }
}
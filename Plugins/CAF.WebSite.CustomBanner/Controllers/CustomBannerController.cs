
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Settings;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.CustomBanner.Domain;
using CAF.WebSite.CustomBanner.Models;
using CAF.WebSite.CustomBanner.Services;
using CAF.WebSite.CustomBanner.Settings;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using CAF.WebSite.Mvc.Models.Articles;
using CAF.WebSite.Mvc.Models.Catalog;
using CAF.WebSite.Mvc.Models.Topics;
using System;
using System.Web.Mvc;
namespace CAF.WebSite.CustomBanner.Controllers
{
	public class CustomBannerController : AdminControllerBase
	{
		private readonly ISettingService _settingService;
		private readonly ISiteContext _siteContext;
		private readonly ISiteService _siteService;
		private readonly IWorkContext _workContext;
		private readonly AdminAreaSettings _adminAreaSettings;
		private readonly IPermissionService _permissionService;
		private readonly IPictureService _pictureService;
		private readonly ICustomBannerService _customBannerService;
		public CustomBannerController(IWorkContext workContext, ISettingService settingService, ISiteContext siteContext, ISiteService siteService, AdminAreaSettings adminAreaSettings, IPermissionService permissionService, ICustomBannerService cutomBannerService, IPictureService pictureService)
		{
			this._settingService = settingService;
			this._settingService = settingService;
			this._siteContext = siteContext;
			this._siteService = siteService;
			this._workContext = workContext;
			this._adminAreaSettings = adminAreaSettings;
			this._permissionService = permissionService;
			this._pictureService = pictureService;
			this._customBannerService = cutomBannerService;
		}
		[AdminAuthorize, ChildActionOnly]
		public ActionResult Configure()
		{
			int siteScope = ContollerExtensions.GetActiveSiteScopeConfiguration(this, this._siteService, this._workContext);
			CustomBannerSettings customBannerSettings = this._settingService.LoadSetting<CustomBannerSettings>(siteScope);
			ConfigurationModel model = new ConfigurationModel();
			model.MaxBannerHeight = customBannerSettings.MaxBannerHeight;
			model.StretchPicture = customBannerSettings.StretchPicture;
			model.ShowBorderTop = customBannerSettings.ShowBorderTop;
			model.ShowBorderBottom = customBannerSettings.ShowBorderBottom;
			model.BorderBottomColor = customBannerSettings.BorderBottomColor;
			model.BorderTopColor = customBannerSettings.BorderTopColor;
			SiteDependingSettingHelper siteDependingSettingHelper = new SiteDependingSettingHelper(base.ViewData);
			siteDependingSettingHelper.GetOverrideKeys(customBannerSettings, model, siteScope, this._settingService, true);
			return base.View(model);
		}
		[AdminAuthorize, ChildActionOnly, HttpPost]
		public ActionResult Configure(ConfigurationModel model, FormCollection form)
		{
			if (!base.ModelState.IsValid)
			{
				return this.Configure();
			}
			base.ModelState.Clear();
			int siteScope = ContollerExtensions.GetActiveSiteScopeConfiguration(this, this._siteService, this._workContext);
			CustomBannerSettings customBannerSettings = this._settingService.LoadSetting<CustomBannerSettings>(siteScope);
			customBannerSettings.MaxBannerHeight = model.MaxBannerHeight;
			customBannerSettings.StretchPicture = model.StretchPicture;
			customBannerSettings.BorderBottomColor = model.BorderBottomColor;
			customBannerSettings.BorderTopColor = model.BorderTopColor;
			customBannerSettings.ShowBorderBottom = model.ShowBorderBottom;
			customBannerSettings.ShowBorderTop = model.ShowBorderTop;
			SiteDependingSettingHelper siteDependingSettingHelper = new SiteDependingSettingHelper(base.ViewData);
			siteDependingSettingHelper.UpdateSettings(customBannerSettings, form, siteScope, this._settingService);
			this._settingService.ClearCache();
			return this.Configure();
		}
		[ChildActionOnly]
		public ActionResult PublicInfo(string widgetZone, object model)
		{
            //if (LicenseChecker.CheckState("SmartSite.CustomBanner", null) == null)
            //{
            //    return new EmptyResult();
            //}
			int pictureId = 0;
			if (model != null)
			{
				int pageId = 0;
				string entity = "";
                if (model.GetType() == typeof(ArticlePostModel))
                {
                    ArticlePostModel articleModel = (ArticlePostModel)model;
                    pageId = articleModel.Id;
                    entity = "article";
                }
                else
                {
                    if (model.GetType() == typeof(ArticleCategoryModel))
                    {
                        ArticleCategoryModel categoryModel = (ArticleCategoryModel)model;
                        pageId = categoryModel.Id;
                        entity = "category";
                    }
                    else
                    {
                        if (model.GetType() == typeof(TopicModel))
                        {
                            TopicModel topicModel = (TopicModel)model;
                            pageId = topicModel.Id;
                            entity = "topic";
                        }
                    }
                }
				CustomBannerRecord bannerRecord = this._customBannerService.GetCustomBannerRecord(pageId, entity);
				if (bannerRecord != null)
				{
					pictureId = bannerRecord.PictureId;
				}
			}
			if (pictureId != 0)
			{
				CustomBannerSettings customBannerSettings = this._settingService.LoadSetting<CustomBannerSettings>(this._siteContext.CurrentSite.Id);
				Picture pic = this._pictureService.GetPictureById(pictureId);
				return base.View(new PublicInfoModel
				{
					PicturePath = this._pictureService.GetPictureUrl(pic, 0, true, null),
					MaxBannerHeight = customBannerSettings.MaxBannerHeight,
					StretchPicture = customBannerSettings.StretchPicture,
					ShowBorderBottom = customBannerSettings.ShowBorderBottom,
					ShowBorderTop = customBannerSettings.ShowBorderTop,
					BorderTopColor = customBannerSettings.BorderTopColor,
					BorderBottomColor = customBannerSettings.BorderBottomColor
				});
			}
			return base.Content("");
		}
		[AdminAuthorize]
		public ActionResult PictureEditTab(int entityId, string entityName)
		{
			if (!this._permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
			{
				return base.AccessDeniedView();
			}
			this._settingService.LoadSetting<CustomBannerSettings>(this._siteContext.CurrentSite.Id);
			CustomBannerRecord customBannerRecord = new CustomBannerRecord();
			customBannerRecord = this._customBannerService.GetCustomBannerRecord(entityId, entityName);
			PictureEditTabModel model = new PictureEditTabModel();
			model.EntityId = entityId;
			model.EntityName = entityName;
			if (customBannerRecord != null)
			{
				model.PictureId = customBannerRecord.PictureId;
			}
			PartialViewResult result = base.PartialView(model);
            ViewDataDictionary viewData = result.ViewData;
			TemplateInfo templateInfo = new TemplateInfo();
            templateInfo.HtmlFieldPrefix = "CustomProperties[CustomBanner]";
            viewData.TemplateInfo = templateInfo;
			return result;
		}
	}
}

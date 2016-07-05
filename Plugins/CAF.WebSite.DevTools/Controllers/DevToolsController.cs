using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Settings;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CAF.WebSite.DevTools.Controllers
{

	public class DevToolsController : BaseController
    {
		private readonly IWorkContext _workContext;
		private readonly ISiteContext _storeContext;
		private readonly ISiteService _storeService;
		private readonly ISettingService _settingService;

		public DevToolsController(
			IWorkContext workContext,
			ISiteContext storeContext,
			ISiteService storeService,
			ISettingService settingService)
		{
			_workContext = workContext;
			_storeContext = storeContext;
			_storeService = storeService;
			_settingService = settingService;
		}

		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure()
		{
			// load settings for a chosen store scope
			var storeScope = this.GetActiveSiteScopeConfiguration(_storeService, _workContext);
			var settings = _settingService.LoadSetting<ProfilerSettings>(storeScope);

			var storeDependingSettingHelper = new SiteDependingSettingHelper(ViewData);
			storeDependingSettingHelper.GetOverrideKeys(settings, settings, storeScope, _settingService);

			return View(settings);
		}

		[AdminAuthorize]
		[HttpPost]
		[ChildActionOnly]
		public ActionResult Configure(ProfilerSettings model, FormCollection form)
		{
			if (!ModelState.IsValid)
				return Configure();

			// load settings for a chosen store scope
			var storeDependingSettingHelper = new SiteDependingSettingHelper(ViewData);
			var storeScope = this.GetActiveSiteScopeConfiguration(_storeService, _workContext);

			storeDependingSettingHelper.UpdateSettings(model /*settings*/, form, storeScope, _settingService);
			_settingService.ClearCache();

			return Configure();
		}

		public ActionResult MiniProfiler()
		{
			return View();
		}

	}
}
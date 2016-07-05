using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services
{
	
	public interface ICommonServices
	{
		ICacheManager Cache 
		{ 
			get;
		}

		IDbContext DbContext
		{
			get;
		}

        ISiteContext SiteContext
		{
			get;
		}

		IWebHelper WebHelper
		{
			get;
		}

		IWorkContext WorkContext
		{
			get;
		}

		IEventPublisher EventPublisher
		{
			get;
		}

		ILocalizationService Localization
		{
			get;
		}

        IUserActivityService UserActivity
		{
			get;
		}

		INotifier Notifier
		{
			get;
		}

		IPermissionService Permissions
		{
			get;
		}

		ISettingService Settings
		{
			get;
		}
        ISiteService SiteService
        {
            get;
        }
	}

}

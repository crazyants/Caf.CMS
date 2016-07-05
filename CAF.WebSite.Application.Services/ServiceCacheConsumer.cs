using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace CAF.WebSite.Application.Services
{
	public class ServiceCacheConsumer :
		IConsumer<EntityInserted<Site>>,
        IConsumer<EntityDeleted<Site>>,
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>
	{
		public const string SITE_LANGUAGE_MAP_KEY = "caf.svc.sitelangmap";

		private readonly ICacheManager _cacheManager;

		public ServiceCacheConsumer(Func<string, ICacheManager> cache)
        {
			this._cacheManager = cache("static");
        }

		public void HandleEvent(EntityInserted<Site> eventMessage)
		{
			_cacheManager.Remove(SITE_LANGUAGE_MAP_KEY);
		}

		public void HandleEvent(EntityDeleted<Site> eventMessage)
		{
			_cacheManager.Remove(SITE_LANGUAGE_MAP_KEY);
		}

		public void HandleEvent(EntityInserted<Language> eventMessage)
		{
			_cacheManager.Remove(SITE_LANGUAGE_MAP_KEY);
		}

		public void HandleEvent(EntityUpdated<Language> eventMessage)
		{
			_cacheManager.Remove(SITE_LANGUAGE_MAP_KEY);
		}

		public void HandleEvent(EntityDeleted<Language> eventMessage)
		{
			_cacheManager.Remove(SITE_LANGUAGE_MAP_KEY);
		}
	}
}

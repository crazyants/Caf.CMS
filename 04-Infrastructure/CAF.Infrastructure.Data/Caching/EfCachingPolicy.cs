using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using EFCache;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.Infrastructure.Core.Domain.Logging;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Themes;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Email;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.Infrastructure.Core.Domain.Cms.Topic;


namespace CAF.Infrastructure.Data.Caching
{
	
	/* TODO: (mc)
	 * ========================
	 *		1. Let developers register custom caching policies for single entities (from plugins)
	 *		2. Caching policies should contain expiration info and cacheable rows count
	 *		3. Backend: Let users decide which entities to cache
	 *		4. Backend: Let users purge the cache
	 */
	
	internal class EfCachingPolicy : CachingPolicy
	{
		private static readonly HashSet<string> _cacheableSets = new HashSet<string>
			{
				typeof(AclRecord).Name,
				typeof(ActivityLogType).Name,
				typeof(ModelTemplate).Name,
                typeof(Article).Name,
                typeof(ArticleCategory).Name,
				typeof(Country).Name,
				typeof(UserRole).Name,
				typeof(DeliveryTime).Name,
				typeof(EmailAccount).Name,
				typeof(Language).Name,
				typeof(MeasureDimension).Name,
				typeof(MeasureWeight).Name,
				typeof(MessageTemplate).Name,
				typeof(PermissionRecord).Name,
				typeof(QuantityUnit).Name,
				typeof(StateProvince).Name,
				typeof(Site).Name,
				typeof(SiteMapping).Name,
				typeof(TaxCategory).Name,
				typeof(ThemeVariable).Name,
				typeof(Topic).Name
			};

		protected override bool CanBeCached(ReadOnlyCollection<EntitySetBase> affectedEntitySets, string sql, IEnumerable<KeyValuePair<string, object>> parameters)
		{
			var entitySets = affectedEntitySets.Select(x => x.Name);
			var result = entitySets.All(x => _cacheableSets.Contains(x));
			return result;
		}

		protected override void GetExpirationTimeout(ReadOnlyCollection<EntitySetBase> affectedEntitySets, out TimeSpan slidingExpiration, out DateTimeOffset absoluteExpiration)
		{
			base.GetExpirationTimeout(affectedEntitySets, out slidingExpiration, out absoluteExpiration);
			absoluteExpiration = DateTimeOffset.Now.AddHours(24);
		}

		protected override void GetCacheableRows(ReadOnlyCollection<EntitySetBase> affectedEntitySets, out int minCacheableRows, out int maxCacheableRows)
		{
			base.GetCacheableRows(affectedEntitySets, out minCacheableRows, out maxCacheableRows);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Data.Setup;
using EFCache;

namespace CAF.Infrastructure.Data
{

	public class DefaultDbConfiguration : DbConfiguration
	{
		public DefaultDbConfiguration()
		{
            IEfDataProvider provider = null;
            try
            {
                provider = (new EfDataProviderFactory(DataSettings.Current).LoadDataProvider()) as IEfDataProvider;
            }
            catch { /* SmartStore is not installed yet! */ }

            if (provider != null)
            {
                base.SetDefaultConnectionFactory(provider.GetConnectionFactory());

                // prepare EntityFramework 2nd level cache
                ICache cache = null;
                try
                {
                    var innerCache = CAF.Infrastructure.Core.EngineContext.Current.Resolve<Func<Type, CAF.Infrastructure.Core.ICache>>();
                    cache = new Caching.EfCacheImpl(innerCache(typeof(Core.Caching.StaticCache)));
                }
                catch
                {
                    cache = new InMemoryCache();
                }

                var transactionHandler = new CacheTransactionHandler(cache);
                AddInterceptor(transactionHandler);

                Loaded +=
                  (sender, args) => args.ReplaceService<DbProviderServices>(
                    (s, _) => new CachingProviderServices(s, transactionHandler,
                      new Caching.EfCachingPolicy()));
            }
        }
	}

}

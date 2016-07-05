using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.Infrastructure.Core.Plugins
{
	public interface IProviderManager
	{
		Provider<TProvider> GetProvider<TProvider>(string systemName, int siteId = 0) where TProvider : IProvider;

		Provider<IProvider> GetProvider(string systemName, int siteId = 0);

		IEnumerable<Provider<TProvider>> GetAllProviders<TProvider>(int siteId = 0) where TProvider : IProvider;

		IEnumerable<Provider<IProvider>> GetAllProviders(int siteId = 0);
	}
}

using System;

namespace CAF.Infrastructure.Data.Setup
{
	
	public interface ILocaleResourcesProvider
	{
		void MigrateLocaleResources(LocaleResourcesBuilder builder);
	}

}

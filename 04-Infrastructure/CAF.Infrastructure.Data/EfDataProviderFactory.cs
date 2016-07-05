using System;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Exceptions;

namespace CAF.Infrastructure.Data
{
    public partial class EfDataProviderFactory : DataProviderFactory
    {
		public EfDataProviderFactory()
			: this(DataSettings.Current)
		{
		}
		
		public EfDataProviderFactory(DataSettings settings)
			: base(settings)
        {
        }

        public override IDataProvider LoadDataProvider()
        {
            var providerName = Settings.DataProvider;
			if (providerName.IsEmpty())
			{
                throw new WorkException("Data Settings doesn't contain a providerName");
			}

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                default:
                    throw new WorkException(string.Format("Unsupported dataprovider name: {0}", providerName));
            }
        }

    }
}

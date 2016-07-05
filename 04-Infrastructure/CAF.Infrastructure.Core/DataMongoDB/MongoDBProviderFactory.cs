using System;

namespace CAF.Infrastructure.Core.Data
{
    public abstract class MongoDBProviderFactory
    {
        protected MongoDBProviderFactory(MongoDbDataSettings settings)
        {
			Guard.ArgumentNotNull(() => settings);
            this.Settings = settings;
        }

        protected MongoDbDataSettings Settings 
		{ 
			get; 
			private set; 
		}

        public abstract IMongoDBProvider LoadDataProvider();
    }
}

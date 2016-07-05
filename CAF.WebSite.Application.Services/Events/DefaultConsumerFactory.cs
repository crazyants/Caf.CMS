using System;
using System.Linq;
using System.Collections.Generic;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Plugins;

namespace CAF.WebSite.Application.Services.Events
{

    public class DefaultConsumerFactory<T> : IConsumerFactory<T>
    {
        private readonly IEnumerable<Lazy<IConsumer<T>, EventConsumerMetadata>> _consumers;
        private readonly ICommonServices _services;

        public DefaultConsumerFactory(
            IEnumerable<Lazy<IConsumer<T>, EventConsumerMetadata>> consumers,
            ICommonServices services)
        {
            this._consumers = consumers;
            this._services = services;
        }

        public IEnumerable<IConsumer<T>> GetConsumers(bool? resolveAsyncs = null)
        {
            foreach (var consumer in _consumers)
            {
                var isActive = consumer.Metadata.IsActive;
                var isAsync = consumer.Metadata.ExecuteAsync;
                var pluginDescriptor = consumer.Metadata.PluginDescriptor;

                if (isActive && (resolveAsyncs == null || (resolveAsyncs.Value == isAsync)))
                {
                    if (pluginDescriptor == null || IsActiveForStore(pluginDescriptor, _services.SiteContext.CurrentSite.Id))
                    {
                        yield return consumer.Value;
                    }
                }
            }
        }


        public bool HasAsyncConsumer
        {
            get
            {
                return _consumers.Any(c => c.Metadata.IsActive == true && c.Metadata.ExecuteAsync == true);
            }
        }

        private bool IsActiveForStore(PluginDescriptor plugin, int storeId)
        {
            if (storeId == 0)
            {
                return true;
            }

            var limitedToStoresSetting = _services.Settings.GetSettingByKey<string>(plugin.GetSettingKey("LimitedToSites"));
            if (limitedToStoresSetting.IsEmpty())
            {
                return true;
            }

            var limitedToStores = limitedToStoresSetting.ToIntArray();
            if (limitedToStores.Length > 0)
            {
                var flag = limitedToStores.Contains(storeId);
                return flag;
            }

            return true;
        }
    }

}

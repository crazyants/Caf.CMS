using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
 


namespace CAF.WebSite.Application.WebUI.Mvc.Bundles
{
    /// <summary>
    /// <remarks>codehint: caf-add</remarks>
    /// </summary>
    public class BundlePublisher : IBundlePublisher
    {
        private readonly ITypeFinder _typeFinder;

        public BundlePublisher(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public void RegisterBundles(BundleCollection bundles)
        {
            var bundleProviderTypes = _typeFinder.FindClassesOfType<IBundleProvider>();
            var bundleProviders = new List<IBundleProvider>();
            foreach (var providerType in bundleProviderTypes)
            {
                if (!PluginManager.IsActivePluginAssembly(providerType.Assembly))
                {
                    continue;
                }
                
                var provider = Activator.CreateInstance(providerType) as IBundleProvider;
                bundleProviders.Add(provider);
            }

            bundleProviders = bundleProviders.OrderByDescending(bp => bp.Priority).ToList();
            bundleProviders.Each(bp => bp.RegisterBundles(bundles));
        }
    }
}

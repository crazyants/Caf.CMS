using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.Services;
namespace CAF.WebSite.Application.WebUI.Plugins
{
	
	public partial class ProviderManager : IProviderManager
	{
		private readonly IComponentContext _ctx;
		private readonly ICommonServices _services;
		private readonly PluginMediator _pluginMediator;

		public ProviderManager(IComponentContext ctx, ICommonServices services, PluginMediator pluginMediator)
		{
			this._ctx = ctx;
			this._services = services;
			this._pluginMediator = pluginMediator;
		}

		public Provider<TProvider> GetProvider<TProvider>(string systemName, int siteId = 0) where TProvider : IProvider
		{
			if (systemName.IsEmpty())
				return null;

			var provider = _ctx.ResolveOptionalNamed<Lazy<TProvider, ProviderMetadata>>(systemName);
			if (provider != null)
			{
				if (siteId > 0)
				{
					var d = provider.Metadata.PluginDescriptor;
					if (d != null && !IsActiveForSite(d, siteId))
					{
						return null;
					}
				}
				SetUserData(provider.Metadata);
				return new Provider<TProvider>(provider);
			}
			return null;
		}

		public Provider<IProvider> GetProvider(string systemName, int siteId = 0)
		{
			Guard.ArgumentNotEmpty(() => systemName);

			var provider = _ctx.ResolveOptionalNamed<Lazy<IProvider, ProviderMetadata>>(systemName);
			if (provider != null)
			{
				if (siteId > 0)
				{
					var d = provider.Metadata.PluginDescriptor;
					if (d != null && !IsActiveForSite(d, siteId))
					{
						return null;
					}
				}
				SetUserData(provider.Metadata);
				return new Provider<IProvider>(provider);
			}
			return null;
		}

		public IEnumerable<Provider<TProvider>> GetAllProviders<TProvider>(int siteId = 0) where TProvider : IProvider
		{
			var providers = _ctx.Resolve<IEnumerable<Lazy<TProvider, ProviderMetadata>>>();
			if (siteId > 0)
			{
				providers = from p in providers
							let d = p.Metadata.PluginDescriptor
							where d == null || IsActiveForSite(d, siteId)
							select p;
			}
			return SortProviders(providers.Select(x => new Provider<TProvider>(x)));
		}

		public IEnumerable<Provider<IProvider>> GetAllProviders(int siteId = 0)
		{
			var providers = _ctx.Resolve<IEnumerable<Lazy<IProvider, ProviderMetadata>>>();
			if (siteId > 0)
			{
				providers = from p in providers
							let d = p.Metadata.PluginDescriptor
							where d == null || IsActiveForSite(d, siteId)
							select p;
			}
			return SortProviders(providers.Select(x => new Provider<IProvider>(x)));
		}

		protected virtual IEnumerable<Provider<TProvider>> SortProviders<TProvider>(IEnumerable<Provider<TProvider>> providers) where TProvider : IProvider
		{
			foreach (var m in providers.Select(x => x.Metadata))
			{
				SetUserData(m);
			}

			return providers.OrderBy(x => x.Metadata.DisplayOrder).ThenBy(x => x.Metadata.FriendlyName);
		}

		protected virtual void SetUserData(ProviderMetadata metadata)
		{
			if (!metadata.IsEditable)
				return;

			var displayOrder = _pluginMediator.GetUserDisplayOrder(metadata);
			var name = _pluginMediator.GetSetting<string>(metadata, "FriendlyName");
			var description = _pluginMediator.GetSetting<string>(metadata, "Description");

			if (displayOrder.HasValue)
			{
				metadata.DisplayOrder = displayOrder.Value;
			}

			if (name != null)
			{
				metadata.FriendlyName = name;
			}

			if (description != null)
			{
				metadata.Description = description;
			}
		}

		private bool IsActiveForSite(PluginDescriptor plugin, int siteId)
		{
			if (siteId == 0)
			{
				return true;
			}

			var limitedToSitesSetting = _services.Settings.GetSettingByKey<string>(plugin.GetSettingKey("LimitedToSites"));
			if (limitedToSitesSetting.IsEmpty())
			{
				return true;
			}

			var limitedToSites = limitedToSitesSetting.ToIntArray();
			if (limitedToSites.Length > 0)
			{
				var flag = limitedToSites.Contains(siteId);
				return flag;
			}

			return true;
		}

	}

}

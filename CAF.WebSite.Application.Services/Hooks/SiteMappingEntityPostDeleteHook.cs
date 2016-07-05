using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data.Hooks;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Linq;
namespace CAF.WebSite.Application.Services.Hooks
{
	public class SiteMappingEntityPostDeleteHook : PostDeleteHook<ISiteMappingSupported>
	{
		private readonly Lazy<ISiteMappingService> _siteMappingService;

		public SiteMappingEntityPostDeleteHook(Lazy<ISiteMappingService> siteMappingService)
		{
			_siteMappingService = siteMappingService;
		}

		public override void Hook(ISiteMappingSupported entity, HookEntityMetadata metadata)
		{
			var baseEntity = entity as BaseEntity;

			if (baseEntity == null)
				return;

			var entityType = baseEntity.GetUnproxiedType();

			var records = _siteMappingService.Value
				.GetSiteMappingsFor(entityType.Name, baseEntity.Id)
				.ToList();

			records.Each(x => _siteMappingService.Value.DeleteSiteMapping(x));
		}
	}
}

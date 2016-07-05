using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data.Hooks;
using CAF.WebSite.Application.Services.Seo;
using CAF.Infrastructure.Core.Domain.Seo;
using System;
 

namespace CAF.WebSite.Application.Services.Hooks
{
	public class SlugSupportedPostDeleteHook : PostDeleteHook<ISlugSupported>
	{
		private readonly Lazy<IUrlRecordService> _urlRecordService;

		public SlugSupportedPostDeleteHook(Lazy<IUrlRecordService> urlRecordService)
		{
			this._urlRecordService = urlRecordService;
		}

		public override void Hook(ISlugSupported entity, HookEntityMetadata metadata)
		{
			var baseEntity = entity as BaseEntity;

			if (baseEntity == null)
				return;

			var entityType = baseEntity.GetUnproxiedType();

			var records = _urlRecordService.Value.GetUrlRecordsFor(entityType.Name, baseEntity.Id);
			records.Each(x => _urlRecordService.Value.DeleteUrlRecord(x));
		}
	}
}

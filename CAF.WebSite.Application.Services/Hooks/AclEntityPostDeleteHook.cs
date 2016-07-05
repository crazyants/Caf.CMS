using System;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Data.Hooks;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Domain.Security;

namespace CAF.WebSite.Application.Services.Hooks
{
	public class AclEntityPostDeleteHook : PostDeleteHook<IAclSupported>
	{
		private readonly Lazy<IAclService> _aclService;

		public AclEntityPostDeleteHook(Lazy<IAclService> aclService)
		{
			this._aclService = aclService;
		}

		public override void Hook(IAclSupported entity, HookEntityMetadata metadata)
		{
			var baseEntity = entity as BaseEntity;

			if (baseEntity == null)
				return;

			var entityType = baseEntity.GetUnproxiedType();

			var records = _aclService.Value.GetAclRecordsFor(entityType.Name, baseEntity.Id);
			records.Each(x => _aclService.Value.DeleteAclRecord(x));
		}
	}
}

using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Auditing;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Data.Hooks;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Domain.Security;
using System;
namespace CAF.WebSite.Application.Services.Hooks
{

    public class SoftDeletablePreUpdateHook : PreUpdateHook<IDeletionAudited>
	{
        private readonly Lazy<IWorkContext> _workContext;
		private readonly Lazy<IAclService> _aclService;
		private readonly Lazy<IDbContext> _dbContext;

        public SoftDeletablePreUpdateHook(Lazy<IAclService> aclService, Lazy<IDbContext> dbContext, Lazy<IWorkContext> workContext)
		{
            this._workContext = workContext;
			this._aclService = aclService;
			this._dbContext = dbContext;
		}

        public override void Hook(IDeletionAudited entity, HookEntityMetadata metadata)
		{
			var baseEntity = entity as BaseEntity;

			if (baseEntity == null)
				return;

           

			var aclEntity = baseEntity as IAclSupported;
			if (aclEntity == null || !aclEntity.SubjectToAcl)
				return;

			var ctx = _dbContext.Value;
			var modProps = ctx.GetModifiedProperties(baseEntity);
			if (modProps.ContainsKey("Deleted"))
			{
				var shouldSetIdle = entity.Deleted;
				var entityType = baseEntity.GetUnproxiedType();

				var records = _aclService.Value.GetAclRecordsFor(entityType.Name, baseEntity.Id);
				foreach (var record in records)
				{
					record.IsIdle = shouldSetIdle;
					_aclService.Value.UpdateAclRecord(record);
				}
			}
		}

		public override bool RequiresValidation
		{
			get { return false; }
		}
	}
}

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

    public class FullAuditedEntityPreUpdateHook : PreUpdateHook<IModificationAudited>
    {
        private readonly Lazy<IWorkContext> _workContext;
        private readonly Lazy<IDbContext> _dbContext;

        public FullAuditedEntityPreUpdateHook(Lazy<IDbContext> dbContext, Lazy<IWorkContext> workContext)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
        }

        public override void Hook(IModificationAudited entity, HookEntityMetadata metadata)
        {
            if (entity == null)
                return;
            entity.ModifiedOnUtc = Clock.Now;
            entity.ModifiedUserID = _workContext.Value.CurrentUser.Id;
        }

        public override bool RequiresValidation
        {
            get { return false; }
        }
    }
}

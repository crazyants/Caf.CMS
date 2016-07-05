using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Data.Hooks;
using CAF.WebSite.Application.Services.Security;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Application.Services.Localization;

namespace CAF.WebSite.Application.Services.Hooks
{
    
    public class LocalizedEntityPostDeleteHook : PostDeleteHook<ILocalizedEntity>
    {
        private readonly ILocalizedEntityService _localizedEntityService;

        public LocalizedEntityPostDeleteHook(ILocalizedEntityService localizedEntityService)
        {
            this._localizedEntityService = localizedEntityService;
        }

        public override void Hook(ILocalizedEntity entity, HookEntityMetadata metadata)
        {
            var baseEntity = entity as BaseEntity;

            if (baseEntity == null)
                return;

            var entityType = baseEntity.GetUnproxiedType();
            var localizedEntities = this._localizedEntityService.GetLocalizedProperties(baseEntity.Id, entityType.Name);

            localizedEntities.Each(x => this._localizedEntityService.DeleteLocalizedProperty(x));
        }
    }

}

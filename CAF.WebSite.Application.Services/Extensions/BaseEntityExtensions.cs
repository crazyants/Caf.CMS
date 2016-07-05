
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services
{
    public static class BaseEntityExtensions
    {
        /// <summary>
        /// 用户操作记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="CreatedStatus"></param>
        /// <param name="ModifiedStatus"></param>
        public static void AddEntitySysParam(this AuditedBaseEntity entity, bool CreatedStatus = true, bool ModifiedStatus = false)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            if (CreatedStatus)
            {
                entity.CreatedOnUtc = DateTime.Now;
                entity.CreatedUserID = workContext.CurrentUser.Id;
            }
            if (ModifiedStatus)
            {
                entity.ModifiedOnUtc = DateTime.Now;
                entity.ModifiedUserID = workContext.CurrentUser.Id;
            }
        }
    }
}

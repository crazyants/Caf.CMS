
using CAF.Infrastructure.Core.Domain.Security;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Security
{
    public interface IPermissionProvider
    {
        IEnumerable<PermissionRecord> GetPermissions();
        IEnumerable<DefaultPermissionRecord> GetDefaultPermissions();
    }
}

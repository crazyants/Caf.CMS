using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CAF.Infrastructure.Core.Data;

namespace CAF.Infrastructure.Data
{
    public interface IEfDataProvider : IDataProvider
    {
        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns>Connection factory</returns>
        IDbConnectionFactory GetConnectionFactory();

    }
}

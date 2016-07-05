using Autofac;
using CAF.Infrastructure.Core.Data;
using CAF.WebSite.CustomBanner.Domain;
using CAF.Infrastructure.Core.Domain.Common;
using System;
using System.Linq;
namespace CAF.WebSite.CustomBanner.Services
{
    public class CustomBannerService : ICustomBannerService
    {
        private readonly IRepository<CustomBannerRecord> _cbRepository;
        private readonly IDbContext _dbContext;
        private readonly AdminAreaSettings _adminAreaSettings;
        public CustomBannerService(IRepository<CustomBannerRecord> cbRepository, IDbContext dbContext, AdminAreaSettings adminAreaSettings, IComponentContext ctx)
        {
            this._cbRepository = cbRepository;
            this._dbContext = dbContext;
            this._adminAreaSettings = adminAreaSettings;
        }
        public CustomBannerRecord GetCustomBannerRecord(int entityId, string entityName)
        {
            if (entityId == 0)
            {
                return null;
            }
            CustomBannerRecord record = new CustomBannerRecord();
            IQueryable<CustomBannerRecord> query = this._cbRepository.Table.Where(p => p.EntityId == entityId & p.EntityName == entityName);

            if (query.Count() > 0)
                return query.FirstOrDefault();
            return null;
        }
        public void InsertCustomBannerRecord(CustomBannerRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("CustomBannerRecord");
            }
            this._cbRepository.Insert(record);
        }
        public void UpdateCustomBannerRecord(CustomBannerRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }
            this._cbRepository.Update(record);
        }
        public void DeleteCustomBannerRecord(CustomBannerRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }
            this._cbRepository.Delete(record);
        }
    }
}

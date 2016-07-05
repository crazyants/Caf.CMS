using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.WebSite.Application.Services.Directory;
using CAF.Infrastructure.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CAF.WebSite.Application.Services.Common
{
    /// <summary>
    /// VisitRecord service
    /// </summary>
    public partial class VisitRecordService : IVisitRecordService
    {
        #region Fields

        private readonly IRepository<VisitRecord> _visitRecordRepository;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="visitRecordRepository">VisitRecord repository</param>
        /// <param name="countryService">Country service</param>
        /// <param name="stateProvinceService">State/province service</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="visitRecordSettings">VisitRecord settings</param>
        public VisitRecordService(IRepository<VisitRecord> visitRecordRepository,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IDataProvider dataProvider, IDbContext dbContext,
            IEventPublisher eventPublisher)
        {
            this._visitRecordRepository = visitRecordRepository;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._eventPublisher = eventPublisher;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
        }

        #endregion

        #region Methods
        public IList<T> GetVisitRecordStatistics<T>(string date)
        {
            var code = _dataProvider.GetParameter();
            code.ParameterName = "VisitDate";
            code.Value = date;
            code.DbType = System.Data.DbType.String;
            var result = _dbContext.SqlQuery<T>(
              "Exec VisitRecords @VisitDate",
              code);
            var records = result.ToList();

            return records.Count() > 0 ? records : null;
        }

        public IPagedList<VisitRecord> GetAllVisitRecords(int pageIndex, int pageSize)
        {
            var query = _visitRecordRepository.Table;

            var VisitRecords = new PagedList<VisitRecord>(query, pageIndex, pageSize);
            return VisitRecords;
        }
        /// <summary>
        /// Deletes an visitRecord
        /// </summary>
        /// <param name="visitRecord">VisitRecord</param>
        public virtual void DeleteVisitRecord(VisitRecord visitRecord)
        {
            if (visitRecord == null)
                throw new ArgumentNullException("visitRecord");

            _visitRecordRepository.Delete(visitRecord);

            //event notification
            _eventPublisher.EntityDeleted(visitRecord);
        }

        /// <remarks>codehint: sm-add</remarks>
        public virtual void DeleteVisitRecord(int id)
        {
            var visitRecord = GetVisitRecordById(id);
            if (visitRecord != null)
                DeleteVisitRecord(visitRecord);
        }

        /// <summary>
        /// Gets an visitRecord by visitRecord identifier
        /// </summary>
        /// <param name="visitRecordId">VisitRecord identifier</param>
        /// <returns>VisitRecord</returns>
        public virtual VisitRecord GetVisitRecordById(int visitRecordId)
        {
            if (visitRecordId == 0)
                return null;

            var visitRecord = _visitRecordRepository.GetById(visitRecordId);
            return visitRecord;
        }

        /// <summary>
        /// Inserts an visitRecord
        /// </summary>
        /// <param name="visitRecord">VisitRecord</param>
        public virtual void InsertVisitRecord(VisitRecord visitRecord)
        {
            if (visitRecord == null)
                throw new ArgumentNullException("visitRecord");

            _visitRecordRepository.Insert(visitRecord);

            //event notification
            _eventPublisher.EntityInserted(visitRecord);
        }

        /// <summary>
        /// Updates the visitRecord
        /// </summary>
        /// <param name="visitRecord">VisitRecord</param>
        public virtual void UpdateVisitRecord(VisitRecord visitRecord)
        {
            if (visitRecord == null)
                throw new ArgumentNullException("visitRecord");

            _visitRecordRepository.Update(visitRecord);

            //event notification
            _eventPublisher.EntityUpdated(visitRecord);
        }



        #endregion
    }
}
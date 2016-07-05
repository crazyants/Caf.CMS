
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Common;
using System.Collections.Generic;
namespace CAF.WebSite.Application.Services.Common
{
    /// <summary>
    /// VisitRecord service interface
    /// </summary>
    public partial interface IVisitRecordService
    {
        IList<T> GetVisitRecordStatistics<T>(string date);
        IPagedList<VisitRecord> GetAllVisitRecords(int pageIndex, int pageSize);
        /// <summary>
        /// Deletes an visitRecord
        /// </summary>
        /// <param name="visitRecord">VisitRecord</param>
        void DeleteVisitRecord(VisitRecord visitRecord);

        /// <remarks>codehint: sm-add</remarks>
        void DeleteVisitRecord(int id);

        /// <summary>
        /// Gets an visitRecord by visitRecord identifier
        /// </summary>
        /// <param name="visitRecordId">VisitRecord identifier</param>
        /// <returns>VisitRecord</returns>
        VisitRecord GetVisitRecordById(int visitRecordId);

        /// <summary>
        /// Inserts an visitRecord
        /// </summary>
        /// <param name="visitRecord">VisitRecord</param>
        void InsertVisitRecord(VisitRecord visitRecord);

        /// <summary>
        /// Updates the visitRecord
        /// </summary>
        /// <param name="visitRecord">VisitRecord</param>
        void UpdateVisitRecord(VisitRecord visitRecord);

    }
}
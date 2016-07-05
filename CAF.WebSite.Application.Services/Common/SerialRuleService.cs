

using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Common
{

    public partial class SerialRuleService : ISerialRuleService
    {

        #region Constants


        #endregion

        #region Fields
        private readonly IRepository<SerialRule> _serialruleRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        #endregion

        #region Ctor


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="serialruleRepository"></param>
        /// <param name="eventPublisher"></param>
        public SerialRuleService(ICacheManager cacheManager,
            IRepository<SerialRule> serialruleRepository, IDataProvider dataProvider, IDbContext dbContext,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._serialruleRepository = serialruleRepository;
            this._eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        #region Utilities

        #endregion

        #region SerialRules

        public IPagedList<SerialRule> GetAllSerialRules(int pageIndex, int pageSize)
        {
            var query = _serialruleRepository.Table;

            var SerialRules = new PagedList<SerialRule>(query, pageIndex, pageSize);
            return SerialRules;
        }

        public void DeleteSerialRule(SerialRule serialrule)
        {
            if (serialrule == null)
                throw new ArgumentNullException("serialrule");

            serialrule.Deleted = true;
            UpdateSerialRule(serialrule);

            //_serialruleRepository.Delete(serialrule);

            //event notification
            _eventPublisher.EntityDeleted(serialrule);
        }

        public SerialRule GetSerialRuleById(int serialruleId)
        {
            if (serialruleId == 0)
                return null;

            var serialrule = _serialruleRepository.GetById(serialruleId);
            return serialrule;
        }

        public SerialRule GetSerialRuleByCode(string Code)
        {
            if (string.IsNullOrWhiteSpace(Code))
                return null;
            var query = from c in _serialruleRepository.Table
                        where c.Code == Code
                        select c;
            var serialrule = query.FirstOrDefault();
            return serialrule;
        }
        public string GetSerialValueByCode(string Code)
        {
            var code = _dataProvider.GetParameter();
            code.ParameterName = "Code";
            code.Value = Code;
            code.DbType = System.Data.DbType.String;
            var result = _dbContext.SqlQuery<string>(
              "Exec GetSerialNo @Code",
              code);
            var codes = result.ToList();
            return codes.Count() > 0 ? codes.First() : "";
        }

        public IList<SerialRule> GetSerialRulesByIds(int[] serialruleIds)
        {
            if (serialruleIds == null || serialruleIds.Length == 0)
                return new List<SerialRule>();

            var query = from c in _serialruleRepository.Table
                        where serialruleIds.Contains(c.Id)
                        select c;
            var serialrules = query.ToList();
            //sort by passed identifiers
            var sortedSerialRule = new List<SerialRule>();
            foreach (int id in serialruleIds)
            {
                var serialrule = serialrules.Find(x => x.Id == id);
                if (serialrule != null)
                    sortedSerialRule.Add(serialrule);
            }
            return sortedSerialRule;
        }

        /// <summary>
        /// Gets all serialrules
        /// </summary>
        /// <returns>SerialRules</returns>
        public IList<SerialRule> GetAllSerialRules()
        {
            var query = from s in _serialruleRepository.Table

                        select s;
            var serialrules = query.ToList();
            return serialrules;
        }

        public IQueryable<SerialRule> GetAllSerialRuleQ()
        {
            var query = _serialruleRepository.Table;
            return query;
        }

        public void InsertSerialRule(SerialRule serialrule)
        {
            if (serialrule == null)
                throw new ArgumentNullException("serialrule");

            _serialruleRepository.Insert(serialrule);

            //event notification
            _eventPublisher.EntityInserted(serialrule);
        }


        /// <summary>
        /// Updates the serialrule
        /// </summary>
        /// <param name="serialrule">SerialRule</param>
        public virtual void UpdateSerialRule(SerialRule serialrule)
        {
            if (serialrule == null)
                throw new ArgumentNullException("serialrule");

            _serialruleRepository.Update(serialrule);

            //event notification
            _eventPublisher.EntityUpdated(serialrule);
        }

        #endregion
        #endregion

        #region  ExtensionMethod

        #endregion

    }
}

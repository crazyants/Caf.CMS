
using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Common
{

    public partial interface ISerialRuleService
    {

        #region SerialRules

        /// <summary>
        /// Gets all SerialRules 
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>SerialRules</returns>
        IPagedList<SerialRule> GetAllSerialRules(int pageIndex, int pageSize);

        /// <summary>
        /// Delete a SerialRule
        /// </summary>
        /// <param name="SerialRule">SerialRule</param>
        void DeleteSerialRule(SerialRule SerialRule);

        /// <summary>
        /// Gets a SerialRule
        /// </summary>
        /// <param name="SerialRuleId">SerialRule identifier</param>
        /// <returns>A SerialRule</returns>
        SerialRule GetSerialRuleById(int SerialRuleId);

        /// <summary>
        /// Get SerialRules by identifiers
        /// </summary>
        /// <param name="SerialRuleIds">SerialRule identifiers</param>
        /// <returns>SerialRules</returns>
        IList<SerialRule> GetSerialRulesByIds(int[] SerialRuleIds);

        /// <summary>
        /// Get SerialRules by identifiers
        /// </summary>
        /// <returns>SerialRules</returns>
        IList<SerialRule> GetAllSerialRules();

        /// <summary>
        /// Get SerialRules by identifiers
        /// </summary>
        /// <returns>SerialRules</returns>
        IQueryable<SerialRule> GetAllSerialRuleQ();

        /// <summary>
        /// Gets a SerialRule
        /// </summary>
        /// <param name="Code">Code</param>
        /// <returns>SerialRule</returns>
        SerialRule GetSerialRuleByCode(string Code);

        string GetSerialValueByCode(string Code);

        /// <summary>
        /// Insert a SerialRule
        /// </summary>
        /// <param name="SerialRule">SerialRule</param>
        void InsertSerialRule(SerialRule SerialRule);

        /// <summary>
        /// Updates the SerialRule
        /// </summary>
        /// <param name="SerialRule">SerialRule</param>
        void UpdateSerialRule(SerialRule SerialRule);

        #endregion

        #region  ExtensionMethod

        #endregion

    }
}

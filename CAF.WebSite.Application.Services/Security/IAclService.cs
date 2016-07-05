using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Security
{
    /// <summary>
    /// ACL service inerface
    /// </summary>
    public partial interface IAclService
    {
		/// <summary>
		/// Gets a value indicating whether at least one ACL record is in active state system-wide
		/// </summary>
		bool HasActiveAcl { get; }
		
		/// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        void DeleteAclRecord(AclRecord aclRecord);

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        AclRecord GetAclRecordById(int aclRecordId);
        
        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        IList<AclRecord> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported;

		/// <summary>
		/// Gets ACL records
		/// </summary>
		/// <param name="entityName">Name of entity</param>
		/// <param name="entityId">Id of entity</param>
		/// <returns>ACL records</returns>
		IList<AclRecord> GetAclRecordsFor(string entityName, int entityId);

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        void InsertAclRecord(AclRecord aclRecord);
        
        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
		/// <param name="userRoleId">User role id</param>
        void InsertAclRecord<T>(T entity, int userRoleId) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        void UpdateAclRecord(AclRecord aclRecord);

        /// <summary>
        /// Find user role identifiers with granted access
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>User role identifiers</returns>
        int[] GetUserRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize<T>(T entity) where T : BaseEntity, IAclSupported;

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize<T>(T entity, User user) where T : BaseEntity, IAclSupported;
    }
}
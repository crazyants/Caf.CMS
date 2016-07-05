using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAF.WebSite.Application.Services.Security
{
    /// <summary>
    /// ACL service
    /// </summary>
    public partial class AclService : IAclService
    {
        #region Constants

        private const string ACLRECORD_BY_ENTITYID_NAME_KEY = "CafSite.aclrecord.entityid-name-{0}-{1}";
        private const string ACLRECORD_PATTERN_KEY = "CafSite.aclrecord.";

        #endregion

        #region Fields

        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
		private bool? _hasActiveAcl;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="workContext">Work context</param>
        /// <param name="aclRecordRepository">ACL record repository</param>
        public AclService(ICacheManager cacheManager, IWorkContext workContext,
            IRepository<AclRecord> aclRecordRepository)
        {
            this._cacheManager = cacheManager;
            this._workContext = workContext;
            this._aclRecordRepository = aclRecordRepository;

			this.QuerySettings = DbQuerySettings.Default;
		}

		public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Members

		public bool HasActiveAcl 
		{
			get
			{
				if (!_hasActiveAcl.HasValue)
				{
					var query = _aclRecordRepository.Where(x => !x.IsIdle);
					_hasActiveAcl = query.Any();
				}
				return _hasActiveAcl.Value;
			}
		}

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void DeleteAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Delete(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        public virtual AclRecord GetAclRecordById(int aclRecordId)
        {
            if (aclRecordId == 0)
                return null;

            var aclRecord = _aclRecordRepository.GetById(aclRecordId);
            return aclRecord;
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        public IList<AclRecord> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

			return GetAclRecordsFor(entityName, entityId);
        }

		public virtual IList<AclRecord> GetAclRecordsFor(string entityName, int entityId)
		{
            //Guard.ArgumentIsPositive(entityId, "entityId");
            //Guard.ArgumentNotEmpty(() => entityName);

			var query = from ur in _aclRecordRepository.Table
						where ur.EntityId == entityId &&
						ur.EntityName == entityName
						select ur;
			var aclRecords = query.ToList();
			return aclRecords;
		}


        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void InsertAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Insert(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
		/// <param name="userRoleId">User role id</param>
        public virtual void InsertAclRecord<T>(T entity, int userRoleId) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (userRoleId == 0)
                throw new ArgumentOutOfRangeException("userRoleId");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var aclRecord = new AclRecord()
            {
                EntityId = entityId,
                EntityName = entityName,
                UserRoleId = userRoleId
            };

            InsertAclRecord(aclRecord);
        }

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void UpdateAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException("aclRecord");

            _aclRecordRepository.Update(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(ACLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Find user role identifiers with granted access
        /// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="entity">Entity</param>
        /// <returns>User role identifiers</returns>
        public virtual int[] GetUserRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            string key = string.Format(ACLRECORD_BY_ENTITYID_NAME_KEY, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from ur in _aclRecordRepository.Table
                            where ur.EntityId == entityId &&
                            ur.EntityName == entityName 
                            select ur.UserRoleId;
                var result = query.ToArray();
                //little hack here. nulls aren't cacheable so set it to ""
                if (result == null)
                    result = new int[0];
                return result;
            });
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity, IAclSupported
        {
            return Authorize(entity, _workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, User user) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (user == null)
                return false;

			if (QuerySettings.IgnoreAcl)
				return true;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role1 in user.UserRoles.Where(cr => cr.Active))
                foreach (var role2Id in GetUserRoleIdsWithAccess(entity))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }
        #endregion
    }
}
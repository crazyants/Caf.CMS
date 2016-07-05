using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;
using System.Collections.Generic;


namespace CAF.Infrastructure.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public class PermissionRecord : BaseEntity
    {
        private ICollection<UserRole> _userRoles;

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public string SystemName { get; set; }
        
        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<UserRole> UserRoles
        {
			get { return _userRoles ?? (_userRoles = new HashSet<UserRole>()); }
            protected set { _userRoles = value; }
        }   
    }
}

using CAF.Infrastructure.Core;
using System;
namespace CAF.Infrastructure.Core.Domain.Users
{
    /// <summary>
    /// Represents a user generated content
    /// </summary>
    public partial class UserContent : AuditedBaseEntity
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is approved
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        public virtual User User { get; set; }
    }
}

using CAF.Infrastructure.Core.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace CAF.Infrastructure.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    // [DataContract]
    public abstract partial class AuditedBaseEntity : BaseEntity, IAudited
    {

        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Creator of this entity.
        /// </summary>
        public virtual long? CreatedUserID { get; set; }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? ModifiedOnUtc { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual long? ModifiedUserID { get; set; }

        /// <summary>
        ///     获取或设置 版本控制标识，用于处理并发
        /// </summary>
        [ConcurrencyCheck]
        [Timestamp]
        [DataMember]
        public byte[] Timestamp { get; set; }

    }
}

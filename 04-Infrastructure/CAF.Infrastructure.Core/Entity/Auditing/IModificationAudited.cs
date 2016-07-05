using System;

namespace CAF.Infrastructure.Core.Auditing
{
    /// <summary>
    /// This interface is implemented by entities that is wanted to store modification information (who and when modified lastly).
    /// Properties are automatically set when updating the <see cref="IEntity"/>.
    /// </summary>
    public interface IModificationAudited
    {
        /// <summary>
        /// The last time of modification.
        /// </summary>
        DateTime? ModifiedOnUtc { get; set; }

        /// <summary>
        /// Last modifier user for this entity.
        /// </summary>
        long? ModifiedUserID { get; set; }
    }
}
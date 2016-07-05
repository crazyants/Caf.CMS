using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    ///     Represents the storage type for the <see cref="IIntelligencePolicy" />
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        ///     Uses the temp data as storage.
        /// </summary>
        TempData = 1,

        /// <summary>
        ///     Uses the session as storage.
        /// </summary>
        Session = 2
    }
}
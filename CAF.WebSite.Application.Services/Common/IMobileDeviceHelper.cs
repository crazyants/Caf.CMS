
using System.Web;

namespace CAF.WebSite.Application.Services.Common
{
    /// <summary>
    /// Mobile device helper interface
    /// </summary>
    public partial interface IMobileDeviceHelper
    {
        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <returns>Result</returns>
        bool IsMobileDevice();

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        bool MobileDevicesSupported();

        /// <summary>
        /// Returns a value indicating whether current user prefer to use full desktop version (even request is made by a mobile device)
        /// </summary>
        bool UserDontUseMobileVersion();
    }
}
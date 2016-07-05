
using CAF.Infrastructure.Core.Configuration;
namespace CAF.WebSite.Application.Services.Helpers
{
    public class DateTimeSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a default store time zone identifier
        /// </summary>
        public string DefaultSiteTimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Users are allowed to select their time zone
        /// </summary>
        public bool AllowUsersToSetTimeZone { get; set; }
    }
}
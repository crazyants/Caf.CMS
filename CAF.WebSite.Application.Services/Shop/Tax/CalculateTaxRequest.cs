

using CAF.WebSite.Domain.Seedwork.Common;
using CAF.WebSite.Domain.Seedwork.Users;

namespace CAF.WebSite.Application.Services.Tax
{
    /// <summary>
    /// Represents a request for tax calculation
    /// </summary>
    public partial class CalculateTaxRequest
    {
        /// <summary>
        /// Gets or sets a user
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets an address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Gets or sets a tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }
    }
}



using CAF.WebSite.Domain.Seedwork.Shop.Discounts;
using CAF.WebSite.Domain.Seedwork.Sites;
using CAF.WebSite.Domain.Seedwork.Users;
namespace CAF.WebSite.Application.Services.Discounts
{
    /// <summary>
    /// Represents a discount requirement request
    /// </summary>
    public partial class CheckDiscountRequirementRequest
    {
        /// <summary>
        /// Gets or sets the discount
        /// </summary>
        public DiscountRequirement DiscountRequirement { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        public User User { get; set; }

		/// <summary>
		/// Gets or sets the store
		/// </summary>
		public Site Site { get; set; }
    }
}

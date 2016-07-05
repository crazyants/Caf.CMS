

using CAF.Infrastructure.Core.Domain.Cms.Orders;
namespace CAF.WebSite.Application.Services.Payments
{
    /// <summary>
    /// Represents a PostProcessPaymentRequest
    /// </summary>
    public partial class PostProcessPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order. Used when order is already saved (payment gateways that redirect a customer to a third-party URL)
        /// </summary>
        public Order Order { get; set; }

		/// <summary>
		/// Whether the customer clicked the button to re-post the payment process
		/// </summary>
		public bool IsRePostProcessPayment { get; set; }
    }
}

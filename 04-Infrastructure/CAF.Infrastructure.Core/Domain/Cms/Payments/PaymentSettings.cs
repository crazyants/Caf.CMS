using CAF.Infrastructure.Core.Configuration;
using System.Collections.Generic;


namespace CAF.Infrastructure.Core.Domain.Cms.Payments
{
    public class PaymentSettings : ISettings
    {
        public PaymentSettings()
        {
            ActivePaymentMethodSystemNames = new List<string>();
			AllowRePostingPayments = true;
			BypassPaymentMethodSelectionIfOnlyOne = true;
        }

        /// <summary>
        /// Gets or sets a system names of active payment methods
        /// </summary>
        public List<string> ActivePaymentMethodSystemNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to repost (complete) payments for redirection payment methods
        /// </summary>
        public bool AllowRePostingPayments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should bypass 'select payment method' page if we have only one payment method
        /// </summary>
        public bool BypassPaymentMethodSelectionIfOnlyOne { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether we should bypass the payment method info page
		/// </summary>
		public bool BypassPaymentMethodInfo { get; set; }
    }
}
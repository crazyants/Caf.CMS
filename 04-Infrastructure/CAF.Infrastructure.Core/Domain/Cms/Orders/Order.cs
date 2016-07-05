using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Payments;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Orders
{
    /// <summary>
    /// Represents an order
    /// </summary>
    [DataContract]
    public partial class Order : BaseEntity, ISoftDeletable
    {
        private ICollection<GiftCardUsageHistory> _giftCardUsageHistory;
        private ICollection<OrderNote> _orderNotes;
        private ICollection<OrderItem> _orderItems;


        #region Utilities

        protected virtual SortedDictionary<decimal, decimal> ParseTaxRates(string taxRatesStr)
        {
            var taxRatesDictionary = new SortedDictionary<decimal, decimal>();
            if (String.IsNullOrEmpty(taxRatesStr))
                return taxRatesDictionary;

            string[] lines = taxRatesStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (String.IsNullOrEmpty(line.Trim()))
                    continue;

                string[] taxes = line.Split(new char[] { ':' });
                if (taxes.Length == 2)
                {
                    try
                    {
                        decimal taxRate = decimal.Parse(taxes[0].Trim(), CultureInfo.InvariantCulture);
                        decimal taxValue = decimal.Parse(taxes[1].Trim(), CultureInfo.InvariantCulture);
                        taxRatesDictionary.Add(taxRate, taxValue);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc.ToString());
                    }
                }
            }

            //add at least one tax rate (0%)
            if (taxRatesDictionary.Count == 0)
                taxRatesDictionary.Add(decimal.Zero, decimal.Zero);

            return taxRatesDictionary;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the (formatted) order number
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        [DataMember]
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        [DataMember]
        public int SiteId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the billing address identifier
        /// </summary>
        [DataMember]
        public int BillingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the shipping address identifier
        /// </summary>
        [DataMember]
        public int? ShippingAddressId { get; set; }

        /// <summary>
        /// Gets or sets an order status identifier
        /// </summary>
        [DataMember]
        public int OrderStatusId { get; set; }



        /// <summary>
        /// Gets or sets the payment status identifier
        /// </summary>
        [DataMember]
        public int PaymentStatusId { get; set; }

        /// <summary>
        /// Gets or sets the payment method system name
        /// </summary>
        [DataMember]
        public string PaymentMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets the customer currency code (at the moment of order placing)
        /// </summary>
        [DataMember]
        public string UserCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the currency rate
        /// </summary>
        [DataMember]
        public decimal CurrencyRate { get; set; }

        /// <summary>
        /// Gets or sets the customer tax display type identifier
        /// </summary>
        [DataMember]
        public virtual int UserTaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets the VAT number (the European Union Value Added Tax)
        /// </summary>
        [DataMember]
        public string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (incl tax)
        /// </summary>
        [DataMember]
        public decimal OrderSubtotalInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (excl tax)
        /// </summary>
        [DataMember]
        public decimal OrderSubtotalExclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount (incl tax)
        /// </summary>
        [DataMember]
        public decimal OrderSubTotalDiscountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount (excl tax)
        /// </summary>
        [DataMember]
        public decimal OrderSubTotalDiscountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping (incl tax)
        /// </summary>
        [DataMember]
        public decimal OrderShippingInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping (excl tax)
        /// </summary>
        [DataMember]
        public decimal OrderShippingExclTax { get; set; }

        /// <summary>
        /// Gets or sets the tax rate for order shipping
        /// </summary>
        [DataMember]
        public decimal OrderShippingTaxRate { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee (incl tax)
        /// </summary>
        [DataMember]
        public decimal PaymentMethodAdditionalFeeInclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee (excl tax)
        /// </summary>
        [DataMember]
        public decimal PaymentMethodAdditionalFeeExclTax { get; set; }

        /// <summary>
        /// Gets or sets the tax rate for payment method additional fee
        /// </summary>
        [DataMember]
        public decimal PaymentMethodAdditionalFeeTaxRate { get; set; }

        /// <summary>
        /// Gets or sets the tax rates
        /// </summary>
        [DataMember]
        public string TaxRates { get; set; }

        /// <summary>
        /// Gets or sets the order tax
        /// </summary>
        [DataMember]
        public decimal OrderTax { get; set; }

        /// <summary>
        /// Gets or sets the order discount (applied to order total)
        /// </summary>
        [DataMember]
        public decimal OrderDiscount { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        [DataMember]
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the refunded amount
        /// </summary>
        [DataMember]
        public decimal RefundedAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        [DataMember]
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time when order was updated
        /// </summary>
        [DataMember]
        public DateTime UpdatedOnUtc { get; set; }



        #endregion

        #region Navigation properties

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        [DataMember]
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the billing address
        /// </summary>
        [DataMember]
        public virtual Address BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address
        /// </summary>
        [DataMember]
        public virtual Address ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the reward points history record
        /// </summary>
       // public virtual RewardPointsHistory RedeemedRewardPointsEntry { get; set; }

     
        /// <summary>
        /// Gets or sets gift card usage history (gift card that were used with this order)
        /// </summary>
        public virtual ICollection<GiftCardUsageHistory> GiftCardUsageHistory
        {
            get { return _giftCardUsageHistory ?? (_giftCardUsageHistory = new HashSet<GiftCardUsageHistory>()); }
            protected set { _giftCardUsageHistory = value; }
        }

        /// <summary>
        /// Gets or sets order notes
        /// </summary>
        [DataMember]
        public virtual ICollection<OrderNote> OrderNotes
        {
            get { return _orderNotes ?? (_orderNotes = new HashSet<OrderNote>()); }
            protected set { _orderNotes = value; }
        }

        /// <summary>
        /// Gets or sets order items
        /// </summary>
        [DataMember]
        public virtual ICollection<OrderItem> OrderItems
        {
            get { return _orderItems ?? (_orderItems = new HashSet<OrderItem>()); }
            protected set { _orderItems = value; }
        }

 

        #endregion

        #region Custom properties

        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        [DataMember]
        public OrderStatus OrderStatus
        {
            get
            {
                return (OrderStatus)this.OrderStatusId;
            }
            set
            {
                this.OrderStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the payment status
        /// </summary>
        [DataMember]
        public PaymentStatus PaymentStatus
        {
            get
            {
                return (PaymentStatus)this.PaymentStatusId;
            }
            set
            {
                this.PaymentStatusId = (int)value;
            }
        }


       

        #endregion
    }
}

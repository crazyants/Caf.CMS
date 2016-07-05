using CAF.WebSite.Domain.Seedwork.Common;
using CAF.WebSite.Domain.Seedwork.Directory;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Users;
using System.Collections.Generic;
 

namespace CAF.WebSite.Application.Services.Shipping
{
    /// <summary>
    /// Represents a request for getting shipping rate options
    /// </summary>
    public partial class GetShippingOptionRequest
    {
        public GetShippingOptionRequest()
        {
            this.Items = new List<OrganizedShoppingCartItem>();
        }

        /// <summary>
        /// Gets or sets a user
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets a shopping cart items
        /// </summary>
        public IList<OrganizedShoppingCartItem> Items { get; set; }

        /// <summary>
        /// Gets or sets a shipping address
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// Shipped from country
        /// </summary>
        public Country CountryFrom { get; set; }

        /// <summary>
        /// Shipped from state/province
        /// </summary>
        public StateProvince StateProvinceFrom { get; set; }

        /// <summary>
        /// Shipped from zip/postal code
        /// </summary>
        public string ZipPostalCodeFrom { get; set; }

        #region Methods

        /// <summary>
        /// Gets total width
        /// </summary>
        /// <returns>Total width</returns>
        public decimal GetTotalWidth()
        {
            decimal totalWidth = decimal.Zero;
            foreach (var shoppingCartItem in this.Items)
            {
                var product = shoppingCartItem.Item.Product;
                if (product != null)
                    totalWidth += product.Width * shoppingCartItem.Item.Quantity;
            }
            return totalWidth;
        }

        /// <summary>
        /// Gets total length
        /// </summary>
        /// <returns>Total length</returns>
        public decimal GetTotalLength()
        {
            decimal totalLength = decimal.Zero;
            foreach (var shoppingCartItem in this.Items)
            {
                var product = shoppingCartItem.Item.Product;
                if (product != null)
                    totalLength += product.Length * shoppingCartItem.Item.Quantity;
            }
            return totalLength;
        }

        /// <summary>
        /// Gets total height
        /// </summary>
        /// <returns>Total height</returns>
        public decimal GetTotalHeight()
        {
            decimal totalHeight = decimal.Zero;
            foreach (var shoppingCartItem in this.Items)
            {
                var product = shoppingCartItem.Item.Product;
                if (product != null)
                    totalHeight += product.Height * shoppingCartItem.Item.Quantity;
            }
            return totalHeight;
        }

        #endregion

    }
}

using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Domain.Seedwork.Common;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Shop.Shipping;
using System.Collections.Generic;
 

namespace CAF.WebSite.Application.Services.Shipping
{
    /// <summary>
    /// Shipping service interface
    /// </summary>
    public partial interface IShippingService
    {
        /// <summary>
        /// Load active shipping rate computation methods
        /// </summary>
		/// <param name="siteId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Shipping rate computation methods</returns>
		IEnumerable<Provider<IShippingRateComputationMethod>> LoadActiveShippingRateComputationMethods(int siteId = 0);

        /// <summary>
        /// Load shipping rate computation method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found Shipping rate computation method</returns>
		Provider<IShippingRateComputationMethod> LoadShippingRateComputationMethodBySystemName(string systemName, int siteId = 0);

        /// <summary>
        /// Load all shipping rate computation methods
        /// </summary>
		/// <param name="siteId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Shipping rate computation methods</returns>
		IEnumerable<Provider<IShippingRateComputationMethod>> LoadAllShippingRateComputationMethods(int siteId = 0);




        /// <summary>
        /// Deletes a shipping method
        /// </summary>
        /// <param name="shippingMethod">The shipping method</param>
        void DeleteShippingMethod(ShippingMethod shippingMethod);

        /// <summary>
        /// Gets a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>Shipping method</returns>
        ShippingMethod GetShippingMethodById(int shippingMethodId);


        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier to filter by</param>
        /// <returns>Shipping method collection</returns>
        IList<ShippingMethod> GetAllShippingMethods(int? filterByCountryId = null);

        /// <summary>
        /// Inserts a shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        void InsertShippingMethod(ShippingMethod shippingMethod);

        /// <summary>
        /// Updates the shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        void UpdateShippingMethod(ShippingMethod shippingMethod);


        /// <summary>
        /// Gets shopping cart item weight (of one item)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>Shopping cart item weight</returns>
		decimal GetShoppingCartItemWeight(OrganizedShoppingCartItem shoppingCartItem);


        /// <summary>
        /// Gets shopping cart item total weight
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>Shopping cart item weight</returns>
		decimal GetShoppingCartItemTotalWeight(OrganizedShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Shopping cart weight</returns>
		decimal GetShoppingCartTotalWeight(IList<OrganizedShoppingCartItem> cart);
        
        /// <summary>
        /// Create shipment package from shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <returns>Shipment package</returns>
		GetShippingOptionRequest CreateShippingOptionRequest(IList<OrganizedShoppingCartItem> cart, Address shippingAddress);

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
		/// <param name="siteId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Shipping options</returns>
		GetShippingOptionResponse GetShippingOptions(IList<OrganizedShoppingCartItem> cart, Address shippingAddress,
			string allowedShippingRateComputationMethodSystemName = "", int siteId = 0);
    }
}

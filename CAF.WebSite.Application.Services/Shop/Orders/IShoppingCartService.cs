using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Users;
using System;
using System.Collections.Generic;
 

namespace CAF.WebSite.Application.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial interface IShoppingCartService
    {
        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current user</param>
		/// <param name="deleteChildCartItems">A value indicating whether to delete child cart items</param>
        void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
			bool ensureOnlyActiveCheckoutAttributes = false, bool deleteChildCartItems = true);

		void DeleteShoppingCartItem(int shoppingCartItemId, bool resetCheckoutData = true,
			bool ensureOnlyActiveCheckoutAttributes = false, bool deleteChildCartItems = true);

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        int DeleteExpiredShoppingCartItems(DateTime olderThanUtc);

        /// <summary>
		/// Validates required products (products which require other variant to be added to the cart)
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
		/// <param name="siteId">Store identifier</param>
		/// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        IList<string> GetRequiredProductWarnings(User user,
            ShoppingCartType shoppingCartType, Product product,
			int siteId, bool automaticallyAddRequiredProductsIfEnabled);

        /// <summary>
		/// Validates a product for standard properties
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        IList<string> GetStandardWarnings(User user, ShoppingCartType shoppingCartType,
			Product product, string selectedAttributes,
            decimal userEnteredPrice, int quantity);

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
		/// <param name="user">The user</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="bundleItem">Product bundle item</param>
        /// <returns>Warnings</returns>
		IList<string> GetShoppingCartItemAttributeWarnings(User user, ShoppingCartType shoppingCartType,
			Product product, string selectedAttributes, int quantity = 1, ProductBundleItem bundleItem = null);

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        IList<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartType shoppingCartType,
            Product product, string selectedAttributes);

		/// <summary>
		/// Validates bundle items
		/// </summary>
		/// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="bundleItem">Product bundle item</param>
		/// <returns>Warnings</returns>
		IList<string> GetBundleItemWarnings(ProductBundleItem bundleItem);
		IList<string> GetBundleItemWarnings(IList<OrganizedShoppingCartItem> cartItems);

        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
		/// <param name="siteId">Store identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
		/// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a product for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
		/// <param name="getRequiredProductWarnings">A value indicating whether we should validate required products (products which require other products to be added to the cart)</param>
		/// <param name="getBundleWarnings">A value indicating whether we should validate bundle and bundle items</param>
		/// <param name="bundleItem">Product bundle item if bundles should be validated</param>
		/// <param name="childItems">Child cart items to validate bundle items</param>
        /// <returns>Warnings</returns>
        IList<string> GetShoppingCartItemWarnings(User user, ShoppingCartType shoppingCartType,
			Product product, int siteId, 
			string selectedAttributes, decimal userEnteredPrice,
			int quantity, bool automaticallyAddRequiredProductsIfEnabled,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true,
			bool getBundleWarnings = true, ProductBundleItem bundleItem = null, IList<OrganizedShoppingCartItem> childItems = null);

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
		IList<string> GetShoppingCartWarnings(IList<OrganizedShoppingCartItem> shoppingCart,
            string checkoutAttributes, bool validateCheckoutAttributes);

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">Price entered by a user</param>
        /// <returns>Found shopping cart item</returns>
		OrganizedShoppingCartItem FindShoppingCartItemInTheCart(IList<OrganizedShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
			Product product,
            string selectedAttributes = "",
            decimal userEnteredPrice = decimal.Zero);

		/// <summary>
		/// Add product to cart
		/// </summary>
		/// <param name="user">The user</param>
		/// <param name="product">The product</param>
		/// <param name="cartType">Cart type</param>
		/// <param name="siteId">Store identifier</param>
		/// <param name="selectedAttributes">Selected attributes</param>
		/// <param name="userEnteredPrice">Price entered by user</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="automaticallyAddRequiredProductsIfEnabled">Whether to add required products</param>
		/// <param name="ctx">Add to cart context</param>
		/// <returns>List with warnings</returns>
		List<string> AddToCart(User user, Product product, ShoppingCartType cartType, int siteId, string selectedAttributes,
			decimal userEnteredPrice, int quantity, bool automaticallyAddRequiredProductsIfEnabled, AddToCartContext ctx = null);

		/// <summary>
		/// Add product to cart
		/// </summary>
		/// <param name="ctx">Add to cart context</param>
		void AddToCart(AddToCartContext ctx);

		/// <summary>
		/// Stores the shopping card items in the database
		/// </summary>
		/// <param name="ctx">Add to cart context</param>
		void AddToCartStoring(AddToCartContext ctx);

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="newQuantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        IList<string> UpdateShoppingCartItem(User user, int shoppingCartItemId,
            int newQuantity, bool resetCheckoutData);

        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromUser">From user</param>
        /// <param name="toUser">To user</param>
        void MigrateShoppingCart(User fromUser, User toUser);

		/// <summary>
		/// Copies a shopping cart item.
		/// </summary>
		/// <param name="sci">Shopping cart item</param>
		/// <param name="user">The user</param>
		/// <param name="cartType">Shopping cart type</param>
		/// <param name="siteId">Store Id</param>
		/// <param name="addRequiredProductsIfEnabled">Add required products if enabled</param>
		/// <returns>List with add-to-cart warnings.</returns>
		IList<string> Copy(OrganizedShoppingCartItem sci, User user, ShoppingCartType cartType, int siteId, bool addRequiredProductsIfEnabled);
    }
}

using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Seedwork.Shop.Discounts;
using CAF.WebSite.Domain.Seedwork.Shop.Orders;
using CAF.WebSite.Domain.Seedwork.Users;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Catalog
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial interface IPriceCalculationService
    {
		/// <summary>
		/// Get product special price (is valid)
		/// </summary>
		/// <param name="product">Product</param>
		/// <returns>Product special price</returns>
		decimal? GetSpecialPrice(Product product);

		/// <summary>
		/// Gets the final price
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
		/// <returns>Final price</returns>
		decimal GetFinalPrice(Product product, bool includeDiscounts);

		/// <summary>
		/// Gets the final price
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="user">The user</param>
		/// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
		/// <returns>Final price</returns>
		decimal GetFinalPrice(Product product,
			User user,
			bool includeDiscounts);

		/// <summary>
		/// Gets the final price
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="user">The user</param>
		/// <param name="additionalCharge">Additional charge</param>
		/// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
		/// <returns>Final price</returns>
		decimal GetFinalPrice(Product product,
			User user,
			decimal additionalCharge,
			bool includeDiscounts);

		/// <summary>
		/// Gets the final price
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="user">The user</param>
		/// <param name="additionalCharge">Additional charge</param>
		/// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
		/// <param name="quantity">Shopping cart item quantity</param>
		/// <param name="bundleItem">A product bundle item</param>
		/// <returns>Final price</returns>
		decimal GetFinalPrice(Product product,
			User user,
			decimal additionalCharge,
			bool includeDiscounts,
			int quantity,
			ProductBundleItemData bundleItem = null);

		/// <summary>
		/// Gets the final price including bundle per-item pricing
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="bundleItems">Bundle items</param>
		/// <param name="user">The user</param>
		/// <param name="additionalCharge">Additional charge</param>
		/// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
		/// <param name="quantity">Shopping cart item quantity</param>
		/// <param name="bundleItem">A product bundle item</param>
		/// <returns>Final price</returns>
		decimal GetFinalPrice(Product product, IList<ProductBundleItemData> bundleItems,
			User user, decimal additionalCharge, bool includeDiscounts, int quantity, ProductBundleItemData bundleItem = null);

		/// <summary>
		/// Get the lowest possible price for a product.
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="displayFromMessage">Whether to display the from message.</param>
		/// <returns>The lowest price.</returns>
		decimal GetLowestPrice(Product product, out bool displayFromMessage);

		/// <summary>
		/// Get the lowest price of a grouped product.
		/// </summary>
		/// <param name="product">Grouped product.</param>
		/// <param name="associatedProducts">Products associated to product.</param>
		/// <param name="lowestPriceProduct">The associated product with the lowest price.</param>
		/// <returns>The lowest price.</returns>
		decimal? GetLowestPrice(Product product, IEnumerable<Product> associatedProducts, out Product lowestPriceProduct);

		/// <summary>
		/// Get the initial price including preselected attributes
		/// </summary>
		/// <param name="product">Product</param>
		/// <returns>Preselected price</returns>
		decimal GetPreselectedPrice(Product product);

		/// <summary>
		/// Gets the product cost
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="attributesXml">Shopping cart item attributes in XML</param>
		/// <returns>Product cost</returns>
		decimal GetProductCost(Product product, string attributesXml);

		/// <summary>
		/// Gets discount amount
		/// </summary>
		/// <param name="product">Product</param>
		/// <returns>Discount amount</returns>
		decimal GetDiscountAmount(Product product);

		/// <summary>
		/// Gets discount amount
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="user">The user</param>
		/// <returns>Discount amount</returns>
		decimal GetDiscountAmount(Product product,
			User user);

		/// <summary>
		/// Gets discount amount
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="user">The user</param>
		/// <param name="additionalCharge">Additional charge</param>
		/// <returns>Discount amount</returns>
		decimal GetDiscountAmount(Product product,
			User user,
			decimal additionalCharge);

		/// <summary>
		/// Gets discount amount
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="user">The user</param>
		/// <param name="additionalCharge">Additional charge</param>
		/// <param name="appliedDiscount">Applied discount</param>
		/// <returns>Discount amount</returns>
		decimal GetDiscountAmount(Product product,
			User user,
			decimal additionalCharge,
			out Discount appliedDiscount);

		/// <summary>
		/// Gets discount amount
		/// </summary>
		/// <param name="product">Product</param>
		/// <param name="user">The user</param>
		/// <param name="additionalCharge">Additional charge</param>
		/// <param name="quantity">Product quantity</param>
		/// <param name="appliedDiscount">Applied discount</param>
		/// <param name="bundleItem">A product bundle item</param>
		/// <returns>Discount amount</returns>
		decimal GetDiscountAmount(Product product,
			User user,
			decimal additionalCharge,
			int quantity,
			out Discount appliedDiscount,
			ProductBundleItemData bundleItem = null);


        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
		decimal GetSubTotal(OrganizedShoppingCartItem shoppingCartItem, bool includeDiscounts);

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
		decimal GetUnitPrice(OrganizedShoppingCartItem shoppingCartItem, bool includeDiscounts);
        



        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <returns>Discount amount</returns>
		decimal GetDiscountAmount(OrganizedShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
		decimal GetDiscountAmount(OrganizedShoppingCartItem shoppingCartItem, out Discount appliedDiscount);


		/// <summary>
		/// Gets the price adjustment of a variant attribute value
		/// </summary>
		/// <param name="attributeValue">Product variant attribute value</param>
		/// <returns>Price adjustment of a variant attribute value</returns>
		decimal GetProductVariantAttributeValuePriceAdjustment(ProductVariantAttributeValue attributeValue);
    }
}

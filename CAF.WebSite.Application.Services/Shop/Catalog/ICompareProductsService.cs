using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Catalog
{
    /// <summary>
    /// Compare products service interface
    /// </summary>
    public partial interface ICompareProductsService
    {
        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        void ClearCompareProducts();

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>"Compare products" list</returns>
        IList<Product> GetComparedProducts();

        /// <summary>
        /// Gets the count of compared products
        /// </summary>
        /// <returns></returns>
        int GetComparedProductsCount();

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        void RemoveProductFromCompareList(int productId);

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        void AddProductToCompareList(int productId);
    }
}

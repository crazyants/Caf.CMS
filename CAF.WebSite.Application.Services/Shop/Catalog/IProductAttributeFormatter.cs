
using CAF.WebSite.Domain.Seedwork.Shop.Catalog;
using CAF.WebSite.Domain.Seedwork.Users;
namespace CAF.WebSite.Application.Services.Catalog
{
    /// <summary>
    /// Product attribute formatter interface
    /// </summary>
    public partial interface IProductAttributeFormatter
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="attributes">Attributes</param>
        /// <returns>Attributes</returns>
		string FormatAttributes(Product product, string attributes);

        /// <summary>
        /// Formats attributes
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="attributes">Attributes</param>
        /// <param name="user">User</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderProductAttributes">A value indicating whether to render product attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
		string FormatAttributes(Product product, string attributes,
            User user, string serapator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true);
    }
}

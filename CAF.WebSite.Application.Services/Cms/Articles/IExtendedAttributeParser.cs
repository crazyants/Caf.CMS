using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Collections.Generic;
 

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Checkout attribute parser interface
    /// </summary>
    public partial interface IExtendedAttributeParser
    {
        /// <summary>
        /// Gets selected checkout attribute identifiers
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected checkout attribute identifiers</returns>
        IList<int> ParseExtendedAttributeIds(string attributes);

        /// <summary>
        /// Gets selected checkout attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected checkout attributes</returns>
        IList<ExtendedAttribute> ParseExtendedAttributes(string attributes);

        /// <summary>
        /// Get checkout attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Checkout attribute values</returns>
        IList<ExtendedAttributeValue> ParseExtendedAttributeValues(string attributes);

        /// <summary>
        /// Gets selected checkout attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="ExtendedAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute value</returns>
        IList<string> ParseValues(string attributes, int ExtendedAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddExtendedAttribute(string attributes, ExtendedAttribute ca, string value);

        /// <summary>
        /// Removes checkout attributes which cannot be applied to the current cart and returns an update attributes in XML format
        /// </summary>
        /// <param name="attributes">Attributes in XML format</param>
        /// <param name="cart">Shopping cart items</param>
        /// <returns>Updated attributes in XML format</returns>
		//string EnsureOnlyActiveAttributes(string attributes, IList<OrganizedShoppingCartItem> cart);
    }
}

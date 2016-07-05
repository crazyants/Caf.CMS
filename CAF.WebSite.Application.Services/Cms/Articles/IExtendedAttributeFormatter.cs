
using CAF.Infrastructure.Core.Domain.Cms.Articles;
namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Checkout attribute helper
    /// </summary>
    public partial interface IExtendedAttributeFormatter
    {

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Article</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        string FormatAttributes(string attributes,
            Article article, 
            string serapator = "<br />", 
            bool htmlEncode = true,
            bool renderPrices = true,
            bool allowHyperlinks = true);
    }
}

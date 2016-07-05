using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Localization;
namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a checkout attribute value
    /// </summary>
    public partial class ExtendedAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the checkout attribute mapping identifier
        /// </summary>
        public int ExtendedAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute name
        /// </summary>
        public string Name { get; set; }

      
        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute
        /// </summary>
        public virtual ExtendedAttribute ExtendedAttribute { get; set; }
    }

}

using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Localization;
using System.Collections.Generic;
 

namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a checkout attribute
    /// </summary>
    public partial class ExtendedAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<ExtendedAttributeValue> _checkoutAttributeValues;
        private ICollection<Channel> _channels;
		public ExtendedAttribute()
		{
			this.IsActive = true;
		}

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
        public string Title { get; set; } 
        /// <summary>
        /// Gets or sets the text prompt
        /// </summary>
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public bool IsRequired { get; set; }


        /// <summary>
        /// Gets or sets the   category identifier
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

		/// <summary>
		/// Gets or sets whether the checkout attribute is active
		/// </summary>
		public bool IsActive { get; set; }
        
        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get
            {
                return (AttributeControlType)this.AttributeControlTypeId;
            }
            set
            {
                this.AttributeControlTypeId = (int)value;
            }
        }
        /// <summary>
        /// Gets the checkout attribute values
        /// </summary>
        public virtual ICollection<ExtendedAttributeValue> ExtendedAttributeValues
        {
			get { return _checkoutAttributeValues ?? (_checkoutAttributeValues = new HashSet<ExtendedAttributeValue>()); }
            protected set { _checkoutAttributeValues = value; }
        }
        public virtual ICollection<Channel> Channels
        {
            get { return _channels ?? (_channels = new HashSet<Channel>()); }
            protected set { _channels = value; }
        }
    }

}

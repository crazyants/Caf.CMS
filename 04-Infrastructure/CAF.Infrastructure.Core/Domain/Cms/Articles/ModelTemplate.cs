
using CAF.Infrastructure.Core;
using System.Runtime.Serialization;
namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a category template
    /// </summary>
    public partial class ModelTemplate : BaseEntity
    {
        
        /// <summary>
        /// Gets or sets the template name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the view path
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 
        public int TemplageTypeId { get; set; }
        /// <summary>
        /// Gets or sets the product type
        /// </summary>
        [DataMember]
        public TemplateTypeFormat TemplageType
        {
            get
            {
                return (TemplateTypeFormat)this.TemplageTypeId;
            }
            set
            {
                this.TemplageTypeId = (int)value;
            }
        }
    }
}

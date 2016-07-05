using CAF.Infrastructure.Core;
using System.Runtime.Serialization;
namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a related article
    /// </summary>
	[DataContract]
    public partial class RelatedArticle : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first article identifier
        /// </summary>
		[DataMember]
		public int ArticleId1 { get; set; }

        /// <summary>
        /// Gets or sets the second article identifier
        /// </summary>
		[DataMember]
		public int ArticleId2 { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
		[DataMember]
		public int DisplayOrder { get; set; }
    }

}

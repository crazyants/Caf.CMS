using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// Represents a product review helpfulness
    /// </summary>
    public partial class ArticleReviewHelpfulness : UserContent
    {
        /// <summary>
        /// Gets or sets the Review review identifier
        /// </summary>
        public int ArticleReviewId { get; set; }

        /// <summary>
        /// A value indicating whether a review a helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        /// <summary>
        /// Gets the Review
        /// </summary>
        public virtual ArticleReview ArticleReview { get; set; }
    }
}

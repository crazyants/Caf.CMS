
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
    /// Represents a product review
    /// </summary>
    public partial class ArticleReview : UserContent
    {
        private ICollection<ArticleReviewHelpfulness> _productReviewHelpfulnessEntries;

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Review rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Review helpful votes total
        /// </summary>
        public int HelpfulYesTotal { get; set; }

        /// <summary>
        /// Review not helpful votes total
        /// </summary>
        public int HelpfulNoTotal { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Article Article { get; set; }

        /// <summary>
        /// Gets the entries of product review helpfulness
        /// </summary>
        public virtual ICollection<ArticleReviewHelpfulness> ArticleReviewHelpfulnessEntries
        {
			get { return _productReviewHelpfulnessEntries ?? (_productReviewHelpfulnessEntries = new HashSet<ArticleReviewHelpfulness>()); }
            protected set { _productReviewHelpfulnessEntries = value; }
        }
    }
}

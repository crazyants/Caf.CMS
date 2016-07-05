using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleReviewHelpfulnessMap : EntityTypeConfiguration<ArticleReviewHelpfulness>
    {
        public ArticleReviewHelpfulnessMap()
        {
            this.ToTable("ArticleReviewHelpfulness");
            //commented because it's already configured by CustomerContentMap class
            //this.HasKey(pr => pr.Id);

            this.HasRequired(prh => prh.ArticleReview)
                .WithMany(pr => pr.ArticleReviewHelpfulnessEntries)
                .HasForeignKey(prh => prh.ArticleReviewId).WillCascadeOnDelete(true);
        }
    }
}
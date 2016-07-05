using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleReviewMap : EntityTypeConfiguration<ArticleReview>
    {
        public ArticleReviewMap()
        {
            this.ToTable("ArticleReview");
            //commented because it's already configured by CustomerContentMap class
            //this.HasKey(pr => pr.Id);
            this.HasRequired(pr => pr.Article)
                .WithMany(p => p.ArticleReviews)
                .HasForeignKey(pr => pr.ArticleId);
        }
    }
}
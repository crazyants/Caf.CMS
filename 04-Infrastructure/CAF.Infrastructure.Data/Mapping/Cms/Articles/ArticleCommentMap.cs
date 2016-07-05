using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleCommentMap : EntityTypeConfiguration<ArticleComment>
    {
        public ArticleCommentMap()
        {
            this.ToTable("ArticleComment");

            this.Property(bc => bc.CommentText).IsMaxLength();

            this.HasRequired(bc => bc.Article)
                .WithMany(bp => bp.ArticleComments)
                .HasForeignKey(bc => bc.ArticleId);


        }
    }
}

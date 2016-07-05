using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleExtendMap : EntityTypeConfiguration<ArticleExtend>
    {
        public ArticleExtendMap()
        {
            this.ToTable("ArticleExtend");
            this.HasKey(a => a.Id);
            this.Property(a => a.Name).HasMaxLength(50);
            this.Property(a => a.Value).HasMaxLength(50);
            this.Property(a => a.Type).HasMaxLength(500);


            // Relationships
            this.HasRequired(a => a.Article)
                .WithMany(t => t.ArticleExtends)
                .HasForeignKey(t => t.ArticleId)
                .WillCascadeOnDelete(false);
        }
    }
}

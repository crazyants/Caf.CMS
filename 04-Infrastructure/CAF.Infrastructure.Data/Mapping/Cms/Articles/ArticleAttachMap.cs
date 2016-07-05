using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleAttachMap : EntityTypeConfiguration<ArticleAttach>
    {
        public ArticleAttachMap()
        {
            this.ToTable("ArticleAttach");
            this.HasKey(a => a.Id);
            this.Property(a => a.FileName).HasMaxLength(50);
            this.Property(a => a.FilePath).HasMaxLength(50);
            this.Property(a => a.FileExt).HasMaxLength(10);
            // Relationships
            this.HasRequired(a => a.Article)
                .WithMany(t => t.ArticleAttachs)
                .HasForeignKey(t => t.ArticleId)
                .WillCascadeOnDelete(false);

        }
    }
}

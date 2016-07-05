using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleCategoryMap : EntityTypeConfiguration<ArticleCategory>
    {
        public ArticleCategoryMap()
        {
            this.ToTable("ArticleCategory");
            this.HasKey(a => a.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(400);
            this.Property(c => c.FullName).HasMaxLength(400);
            this.Property(c => c.Alias).HasMaxLength(100);
            this.Property(a => a.LinkUrl).HasMaxLength(50);
            this.Property(a => a.Description).IsMaxLength();
            this.Property(c => c.PageSizeOptions).HasMaxLength(200);
            this.Property(a => a.MetaTitle).HasMaxLength(50);
            this.Property(a => a.MetaKeywords).HasMaxLength(255);
            this.Property(a => a.MetaDescription).HasMaxLength(500);
            // Relationships

            this.HasOptional(p => p.Picture)
                .WithMany()
                .HasForeignKey(p => p.PictureId)
                .WillCascadeOnDelete(false);

            this.HasRequired(cav => cav.Channel)
            .WithMany(ca => ca.ArticleCategorys)
            .HasForeignKey(cav => cav.ChannelId);
        }
    }
}

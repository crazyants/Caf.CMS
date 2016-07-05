using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleMap : EntityTypeConfiguration<Article>
    {
        public ArticleMap()
        {
            this.ToTable("Article");
            this.HasKey(a => a.Id);
            this.Property(a => a.Title).IsRequired().HasMaxLength(100);
            this.Property(a => a.LinkUrl).HasMaxLength(50);
            this.Property(a => a.ImgUrl).HasMaxLength(50);
            this.Property(a => a.MetaTitle).HasMaxLength(50);
            this.Property(a => a.MetaKeywords).HasMaxLength(255);
            this.Property(a => a.MetaDescription).HasMaxLength(500);
            this.Property(a => a.ShortContent).HasMaxLength(255);
            this.Property(a => a.FullContent).IsMaxLength();
            this.Property(a => a.GroupidsView).HasMaxLength(255);
            this.Property(a => a.Author).HasMaxLength(50);

            this.Ignore(a => a.ArticleStatus);
            this.Ignore(u => u.StatusFormat);
         
            this.HasOptional(p => p.Picture)
            .WithMany()
            .HasForeignKey(p => p.PictureId)
            .WillCascadeOnDelete(false);
            // Relationships
            this.HasRequired(a => a.ArticleCategory)
                .WithMany(t => t.Articles)
                .HasForeignKey(t => t.CategoryId)
                .WillCascadeOnDelete(false);

            this.HasMany(p => p.ArticleTags)
            .WithMany(pt => pt.Articles)
            .Map(m => m.ToTable("Article_ArticleTag_Mapping"));


        }
    }
}

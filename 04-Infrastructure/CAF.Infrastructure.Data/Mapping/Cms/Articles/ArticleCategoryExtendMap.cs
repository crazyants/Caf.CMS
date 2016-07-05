using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleCategoryExtendMap : EntityTypeConfiguration<ArticleCategoryExtend>
    {
        public ArticleCategoryExtendMap()
        {
            this.ToTable("ArticleCategoryExtend");
            this.HasKey(a => a.Id);
            this.Property(a => a.Name).HasMaxLength(50);
            this.Property(a => a.Title).HasMaxLength(50);
            this.Property(a => a.ControlType).HasMaxLength(50);
            this.Property(a => a.DataType).HasMaxLength(50);
            this.Property(a => a.ItemOption).HasMaxLength(50);
            this.Property(a => a.DefaultValue).HasMaxLength(50);
            this.Property(a => a.ValidTipMsg).HasMaxLength(50);
            this.Property(a => a.ValidErrorMsg).HasMaxLength(50);
            this.Property(a => a.ValidPattern).HasMaxLength(50);

            // Relationships
            this.HasRequired(a => a.Category)
                .WithMany(t => t.ArticleCategoryExtends)
                .HasForeignKey(t => t.CategoryId)
                .WillCascadeOnDelete(false);
        }
    }
}

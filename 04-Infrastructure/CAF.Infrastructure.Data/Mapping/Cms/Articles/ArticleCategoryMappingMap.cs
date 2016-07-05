using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping.Sites
{
    public partial class ArticleCategoryMappingMap : EntityTypeConfiguration<ArticleCategoryMapping>
	{
        public ArticleCategoryMappingMap()
		{
            this.ToTable("ArticleCategoryMapping");
            this.HasKey(cm => cm.Id);

            this.Property(cm => cm.EntityName).IsRequired().HasMaxLength(400);
		}
	}
}
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;
 

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ModelTemplateMap : EntityTypeConfiguration<ModelTemplate>
    {
        public ModelTemplateMap()
        {
            this.ToTable("ModelTemplate");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.ViewPath).IsRequired().HasMaxLength(400);

            this.Ignore(a => a.TemplageType);
        }
    }
}

using CAF.Infrastructure.Core.Domain.Sites;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping.Sites
{
	public partial class SiteMappingMap : EntityTypeConfiguration<SiteMapping>
	{
        public SiteMappingMap()
		{
            this.ToTable("SiteMapping");
            this.HasKey(cm => cm.Id);

            this.Property(cm => cm.EntityName).IsRequired().HasMaxLength(400);
		}
	}
}
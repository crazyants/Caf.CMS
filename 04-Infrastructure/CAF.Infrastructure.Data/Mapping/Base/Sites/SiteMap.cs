
using CAF.Infrastructure.Core.Domain.Sites;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping.Sites
{
    public partial class SiteMap : EntityTypeConfiguration<Site>
    {
        public SiteMap()
        {
            this.ToTable("Site");
            this.HasKey(c => c.Id);
            this.Property(c => c.Code).HasMaxLength(200);
            this.Property(c => c.SiteKey).HasMaxLength(200);
            this.Property(c => c.Email).HasMaxLength(1000);
            this.Property(c => c.Name).HasMaxLength(1000);
            this.Property(c => c.Tel).HasMaxLength(200);
            this.Property(c => c.Manager).HasMaxLength(200);
            this.Property(c => c.Icon).HasMaxLength(200);
            this.Property(c => c.Hosts).HasMaxLength(1000);
            this.Property(s => s.Url).IsRequired().HasMaxLength(400);
          
        }
    }
}

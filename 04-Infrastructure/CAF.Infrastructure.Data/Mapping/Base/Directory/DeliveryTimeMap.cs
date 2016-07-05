using CAF.Infrastructure.Core.Domain.Directory;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping
{
    public partial class DeliveryTimeMap : EntityTypeConfiguration<DeliveryTime>
    {
        public DeliveryTimeMap()
        {
            this.ToTable("DeliveryTime");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(50);
            this.Property(c => c.ColorHexValue).IsRequired().HasMaxLength(50);
            this.Property(c => c.DisplayLocale).HasMaxLength(50);
            //this.Property(c => c.Published);
            this.Property(c => c.DisplayOrder);
        }
    }
}
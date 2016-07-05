using CAF.Infrastructure.Core.Domain.Directory;
using System.Data.Entity.ModelConfiguration;
namespace CAF.Infrastructure.Data.Mapping
{
    public partial class MeasureWeightMap : EntityTypeConfiguration<MeasureWeight>
    {
        public MeasureWeightMap()
        {
            this.ToTable("MeasureWeight");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(100);
            this.Property(m => m.SystemKeyword).IsRequired().HasMaxLength(100);
            this.Property(m => m.Ratio).HasPrecision(18, 8);
        }
    }
}
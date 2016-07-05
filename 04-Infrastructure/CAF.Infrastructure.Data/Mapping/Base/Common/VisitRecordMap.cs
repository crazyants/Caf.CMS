
using CAF.Infrastructure.Core.Domain.Common;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping.Common
{
    public partial class VisitRecordMap : EntityTypeConfiguration<VisitRecord>
    {
        public VisitRecordMap()
        {
            this.ToTable("VisitRecord");
            this.HasKey(s => s.Id);
            this.Property(c => c.VisitReffer).HasMaxLength(500);

            this.Property(c => c.VisitRefferKeyWork).HasMaxLength(100);
            this.Property(c => c.VisitURL).HasMaxLength(500);
            this.Property(c => c.VisitTitle).HasMaxLength(100);
            this.Property(c => c.VisitIP).HasMaxLength(50);
            this.Property(c => c.VisitProvince).HasMaxLength(30);
            this.Property(c => c.VisitCity).HasMaxLength(30);
            this.Property(c => c.VisitBrowerType).HasMaxLength(50);
            this.Property(c => c.VisitResolution).HasMaxLength(50);
            this.Property(c => c.VisitOS).HasMaxLength(50);
            this.Property(c => c.FromSource).HasMaxLength(50);
        }
    }
}
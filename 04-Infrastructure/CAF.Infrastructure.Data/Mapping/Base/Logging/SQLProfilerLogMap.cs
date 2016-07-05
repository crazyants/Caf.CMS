using System.Data.Entity.ModelConfiguration;
using CAF.Infrastructure.Core.Domain.Base.Logging;

namespace CAF.Infrastructure.Data.Mapping.Logging
{
    public partial class SQLProfilerLogMap : EntityTypeConfiguration<SQLProfilerLog>
    {
        public SQLProfilerLogMap()
        {
            this.ToTable("SQLProfilerLog");
            this.HasKey(l => l.Id);
            this.Property(l => l.InnerException).IsMaxLength();
            this.Property(l => l.Query).IsMaxLength();
            this.Property(l => l.Parameters).IsMaxLength();
            this.Property(l => l.Exception).IsMaxLength();
            this.Property(l => l.FileName).HasMaxLength(100);
            this.Property(l => l.CommandType).HasMaxLength(1000);
        }
    }
}

using CAF.Infrastructure.Core.Domain.Common;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping.Common
{
    public partial class SerialRuleMap : EntityTypeConfiguration<SerialRule>
    {
        public SerialRuleMap()
        {
            this.ToTable("SerialRule");
            this.HasKey(s => s.Id);
            this.Property(c => c.Code).HasMaxLength(200);
            this.Property(c => c.Prefix).HasMaxLength(200);
            this.Property(c => c.Value).HasMaxLength(1000);
            this.Property(c => c.Name).HasMaxLength(1000);
            this.Property(c => c.RandNum);
            this.Property(c => c.DefaultValue).HasMaxLength(1000);
            this.Ignore(c => c.SerialRuleFormat);

        }
    }
}
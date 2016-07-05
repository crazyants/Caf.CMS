using CAF.Infrastructure.Core.Domain.Security;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Security
{
    public partial class AclRecordMap : EntityTypeConfiguration<AclRecord>
    {
        public AclRecordMap()
        {
            this.ToTable("AclRecord");
            this.HasKey(lp => lp.Id);

            this.Property(lp => lp.EntityName).IsRequired().HasMaxLength(400);
			this.Property(x => x.IsIdle).IsRequired();
        }
    }
}
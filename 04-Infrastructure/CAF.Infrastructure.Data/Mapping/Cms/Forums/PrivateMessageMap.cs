using System.Data.Entity.ModelConfiguration;
using CAF.Infrastructure.Core.Domain.Cms.Forums;

namespace CAF.Infrastructure.Data.Mapping.Forums
{
    public partial class PrivateMessageMap : EntityTypeConfiguration<PrivateMessage>
    {
        public PrivateMessageMap()
        {
            this.ToTable("Forums_PrivateMessage");
            this.HasKey(pm => pm.Id);
            this.Property(pm => pm.Subject).IsRequired().HasMaxLength(450);
            this.Property(pm => pm.Text).IsRequired().IsMaxLength();

            this.HasRequired(pm => pm.FromUser)
               .WithMany()
               .HasForeignKey(pm => pm.FromUserId)
               .WillCascadeOnDelete(false);

            this.HasRequired(pm => pm.ToUser)
               .WithMany()
               .HasForeignKey(pm => pm.ToUserId)
               .WillCascadeOnDelete(false);
        }
    }
}

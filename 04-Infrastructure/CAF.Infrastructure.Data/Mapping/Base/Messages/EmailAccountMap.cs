using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Email;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Messages
{
    public partial class EmailAccountMap : EntityTypeConfiguration<EmailAccount>
    {
        public EmailAccountMap()
        {
            this.ToTable("EmailAccount");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.Email).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.DisplayName).HasMaxLength(255);
            this.Property(ea => ea.Host).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.Username).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.Password).IsRequired().HasMaxLength(255);

            this.Ignore(ea => ea.FriendlyName);
        }
    }
}
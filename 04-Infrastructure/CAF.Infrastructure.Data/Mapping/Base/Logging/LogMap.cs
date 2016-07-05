using System.Data.Entity.ModelConfiguration;
using CAF.Infrastructure.Core.Domain.Logging;

namespace CAF.Infrastructure.Data.Mapping.Logging
{
    public partial class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            this.ToTable("Log");
            this.HasKey(l => l.Id);
            this.Property(l => l.ShortMessage).IsRequired();
			this.Property(l => l.FullMessage).IsMaxLength();
            this.Property(l => l.IpAddress).HasMaxLength(200);
			this.Property(l => l.ContentHash).HasMaxLength(40);

            this.Ignore(l => l.LogLevel);

            this.HasOptional(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
            .WillCascadeOnDelete(true);

        }
    }
}
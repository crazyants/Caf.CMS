using CAF.Infrastructure.Core.Domain.Users;
using System.Data.Entity.ModelConfiguration;
 

namespace CAF.Infrastructure.Data.Mapping.Users
{
    public partial class ExternalAuthenticationRecordMap : EntityTypeConfiguration<ExternalAuthenticationRecord>
    {
        public ExternalAuthenticationRecordMap()
        {
            this.ToTable("ExternalAuthenticationRecord");

            this.HasKey(ear => ear.Id);

            this.HasRequired(ear => ear.User)
                .WithMany(c => c.ExternalAuthenticationRecords)
                .HasForeignKey(ear => ear.UserId);

        }
    }
}
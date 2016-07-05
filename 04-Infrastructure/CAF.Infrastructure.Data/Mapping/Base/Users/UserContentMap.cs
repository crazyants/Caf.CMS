
using CAF.Infrastructure.Core.Domain.Users;
using System.Data.Entity.ModelConfiguration;


namespace CAF.Infrastructure.Data.Mapping.Users
{
    public partial class UserContentMap : EntityTypeConfiguration<UserContent>
    {
        public UserContentMap()
        {
            this.ToTable("UserContent");

            this.HasKey(cc => cc.Id);
            this.Property(cc => cc.IpAddress).HasMaxLength(200);

            this.HasRequired(cc => cc.User)
                .WithMany(c => c.UserContent)
                .HasForeignKey(cc => cc.UserId);

        }
    }
}
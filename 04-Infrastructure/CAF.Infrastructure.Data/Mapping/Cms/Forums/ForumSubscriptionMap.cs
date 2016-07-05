using System.Data.Entity.ModelConfiguration;
using CAF.Infrastructure.Core.Domain.Cms.Forums;

namespace CAF.Infrastructure.Data.Mapping.Forums
{
    public partial class ForumSubscriptionMap : EntityTypeConfiguration<ForumSubscription>
    {
        public ForumSubscriptionMap()
        {
            this.ToTable("Forums_Subscription");
            this.HasKey(fs => fs.Id);

            this.HasRequired(fs => fs.User)
                .WithMany()
                .HasForeignKey(fs => fs.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}

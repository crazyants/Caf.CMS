using CAF.Infrastructure.Core.Domain.Affiliates;
using System.Data.Entity.ModelConfiguration;
 
namespace CAF.Infrastructure.Data.Mapping.Affiliates
{
    public partial class AffiliateMap : EntityTypeConfiguration<Affiliate>
    {
        public AffiliateMap()
        {
            this.ToTable("Affiliate");
            this.HasKey(a => a.Id);

            this.HasRequired(a => a.Address).WithMany().HasForeignKey(x => x.AddressId).WillCascadeOnDelete(false);

        }
    }
}
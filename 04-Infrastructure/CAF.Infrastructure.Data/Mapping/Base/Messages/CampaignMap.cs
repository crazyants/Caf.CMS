using System.Data.Entity.ModelConfiguration;
using CAF.Infrastructure.Core.Domain.Messages;

namespace CAF.Infrastructure.Data.Mapping.Messages
{
    public partial class CampaignMap : EntityTypeConfiguration<Campaign>
    {
        public CampaignMap()
        {
            this.ToTable("Campaign");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.Name).IsRequired();
            this.Property(ea => ea.Subject).IsRequired();
            this.Property(ea => ea.Body).IsRequired().IsMaxLength();
        }
    }
}
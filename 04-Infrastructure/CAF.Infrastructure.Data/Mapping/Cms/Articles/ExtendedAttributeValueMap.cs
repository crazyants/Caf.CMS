using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ExtendedAttributeValueMap : EntityTypeConfiguration<ExtendedAttributeValue>
    {
        public ExtendedAttributeValueMap()
        {
            this.ToTable("ExtendedAttributeValue");
            this.HasKey(cav => cav.Id);
            this.Property(cav => cav.Name).IsRequired().HasMaxLength(400);

            this.HasRequired(cav => cav.ExtendedAttribute)
                .WithMany(ca => ca.ExtendedAttributeValues)
                .HasForeignKey(cav => cav.ExtendedAttributeId);
        }
    }
}
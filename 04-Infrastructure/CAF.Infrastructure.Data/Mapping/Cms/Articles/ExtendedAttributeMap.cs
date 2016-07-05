using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;
 

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ExtendedAttributeMap : EntityTypeConfiguration<ExtendedAttribute>
    {
        public ExtendedAttributeMap()
        {
            this.ToTable("ExtendedAttribute");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);
            this.Property(ca => ca.Title).IsRequired().HasMaxLength(400); 
            this.Ignore(pva => pva.AttributeControlType);
        }
    }
}
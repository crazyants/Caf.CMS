using CAF.Infrastructure.Core.Domain.Cms.Orders;
using System.Data.Entity.ModelConfiguration;
 

namespace CAF.Infrastructure.Data.Mapping.Orders
{
    public partial class ShoppingCartItemMap : EntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            this.ToTable("ShoppingCartItem");
            this.HasKey(sci => sci.Id);

            this.Property(sci => sci.UserEnteredPrice).HasPrecision(18, 4);
			this.Property(sci => sci.AttributesXml).IsMaxLength();

            this.Ignore(sci => sci.ShoppingCartType);
            this.Ignore(sci => sci.IsFreeShipping);
            this.Ignore(sci => sci.IsShipEnabled);
            this.Ignore(sci => sci.IsTaxExempt);

            this.HasRequired(sci => sci.User)
                .WithMany(c => c.ShoppingCartItems)
                .HasForeignKey(sci => sci.UserId);

            this.HasRequired(sci => sci.Product)
                .WithMany()
                .HasForeignKey(sci => sci.ProductId);

            //this.HasOptional(sci => sci.BundleItem)
            //    .WithMany()
            //    .HasForeignKey(sci => sci.BundleItemId)
            //    .WillCascadeOnDelete(false);
        }
    }
}

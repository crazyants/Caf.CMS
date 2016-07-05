using System.Data.Entity.ModelConfiguration;
using CAF.Infrastructure.Core.Domain.Cms.Clients;

namespace CAF.Infrastructure.Data.Mapping.Cms.Clients
{
    public partial class ClientMap : EntityTypeConfiguration<Client>
    {
        public ClientMap()
        {
            this.ToTable("Client");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);
			this.Property(m => m.Description).IsMaxLength();
            this.Property(m => m.MetaKeywords).HasMaxLength(400);
            this.Property(m => m.MetaTitle).HasMaxLength(400);
			this.HasOptional(p => p.Picture)
				.WithMany()
				.HasForeignKey(p => p.PictureId)
				.WillCascadeOnDelete(false);
        }
    }
}
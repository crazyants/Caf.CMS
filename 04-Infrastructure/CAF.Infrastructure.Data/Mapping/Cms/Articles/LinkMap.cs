using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class LinkMap : EntityTypeConfiguration<Link>
    {
        public LinkMap()
        {
            this.ToTable("Link");
            this.HasKey(a => a.Id);
            
            this.Property(a => a.Name).IsRequired().HasMaxLength(50);
            this.Property(a => a.Intro).HasMaxLength(255);
            this.Property(a => a.LinkUrl).HasMaxLength(255);
            this.Property(a => a.LogoUrl).HasMaxLength(255);
            this.HasOptional(p => p.Picture)
              .WithMany()
              .HasForeignKey(p => p.PictureId)
              .WillCascadeOnDelete(false);
        }
    }
}

using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class ArticleAlbumMap : EntityTypeConfiguration<ArticleAlbum>
    {
        public ArticleAlbumMap()
        {
            this.ToTable("Article_Picture_Mapping");
            this.HasKey(pp => pp.Id);

            this.HasRequired(pp => pp.Picture)
                .WithMany(p => p.ArticleAlbum)
                .HasForeignKey(pp => pp.PictureId);

            this.HasRequired(pp => pp.Article)
                .WithMany(p => p.ArticleAlbum)
                .HasForeignKey(pp => pp.ArticleId);
        }
    }
}

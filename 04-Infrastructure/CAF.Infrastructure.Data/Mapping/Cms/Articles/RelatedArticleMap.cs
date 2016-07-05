using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System.Data.Entity.ModelConfiguration;
 
namespace CAF.Infrastructure.Data.Mapping.Cms.Articles
{
    public partial class RelatedArticleMap : EntityTypeConfiguration<RelatedArticle>
    {
        public RelatedArticleMap()
        {
            this.ToTable("RelatedArticle");
            this.HasKey(c => c.Id);
        }
    }
}
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Models.Articles
{
    public partial class ArticlesByTagModel : EntityModelBase
    {
        public ArticlesByTagModel()
        {
            Articles = new List<ArticlePostModel>();
            PagingFilteringContext = new ArticleCatalogPagingFilteringMode();
        }

        public string TagName { get; set; }

        public ArticleCatalogPagingFilteringMode PagingFilteringContext { get; set; }

        public IList<ArticlePostModel> Articles { get; set; }
    }
}
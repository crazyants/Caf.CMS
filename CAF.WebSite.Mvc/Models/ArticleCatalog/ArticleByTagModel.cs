using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Articles;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    public partial class ArticleByTagModel : EntityModelBase
    {
        public ArticleByTagModel()
        {
            Articles = new List<ArticlePostModel>();
            PagingFilteringContext = new ArticleCatalogPagingFilteringModel();
        }

        public string TagName { get; set; }

        public ArticleCatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ArticlePostModel> Articles { get; set; }
    }
}
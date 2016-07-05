using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Models.Articles
{
    public partial class ArticlePostListModel : ModelBase
    {
        public ArticlePostListModel()
        {
            PagingFilteringContext = new ArticlePagingFilteringModel();
            ArticlePosts = new List<ArticlePostModel>();
        }

        public int WorkingLanguageId { get; set; }
        public ArticlePagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ArticlePostModel> ArticlePosts { get; set; }
    }
}
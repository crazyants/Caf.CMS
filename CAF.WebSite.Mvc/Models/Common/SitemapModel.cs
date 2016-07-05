using System.Collections.Generic;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Catalog;
using CAF.WebSite.Mvc.Models.Articles;
using CAF.WebSite.Mvc.Models.Topics;
using CAF.WebSite.Mvc.Models.ArticleCatalog;

namespace CAF.WebSite.Mvc.Models.Common
{
    public partial class SitemapModel : ModelBase
    {
        public SitemapModel()
        {
            Articles = new List<ArticlePostModel>();
            ArticleCategories = new List<ArticleCategoryModel>();
            Topics = new List<TopicModel>();
        }
        public IList<ArticlePostModel> Articles { get; set; }
        public IList<ArticleCategoryModel> ArticleCategories { get; set; }
        public IList<TopicModel> Topics { get; set; }
    }
}
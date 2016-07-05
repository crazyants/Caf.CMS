
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using System.Collections.Generic;
namespace CAF.WebSite.Mvc.Models.Articles
{
    public partial class ArticlePostTagModel : EntityModelBase
    {
        public string Name { get; set; }

        public int ArticlePostCount { get; set; }
    }
}
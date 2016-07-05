using CAF.WebSite.Application.WebUI.Mvc;

namespace CAF.WebSite.Mvc.Models.Articles
{
    public partial class ArticleTagModel : EntityModelBase
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public int ArticleCount { get; set; }
    }
}
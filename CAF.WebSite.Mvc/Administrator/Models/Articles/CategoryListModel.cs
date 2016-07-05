using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    public class CategoryListModel : ModelBase
    {
        public string SearchCategoryName { get; set; }
        public string SearchAlias { get; set; }
        public int? SearchChannelId { get; set; }

        public List<ArticleCategoryModel> Categories { get; set; }

        public int GridPageSize { get; set; }
    }
}
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;


namespace CAF.WebSite.Mvc.Models.Articles
{
    public partial class ArticlePostYearModel : ModelBase
    {
        public ArticlePostYearModel()
        {
            Months = new List<ArticlePostMonthModel>();
        }
        public int Year { get; set; }
        public IList<ArticlePostMonthModel> Months { get; set; }
    }
    public partial class ArticlePostMonthModel : ModelBase
    {
        public int Month { get; set; }

        public int ArticlePostCount { get; set; }
    }
}
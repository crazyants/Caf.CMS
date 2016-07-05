

using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.ArticleCatalog
{
    public partial class SearchBoxModel : ModelBase
    {
        public bool AutoCompleteEnabled { get; set; }
        public bool ShowArticleImagesInSearchAutoComplete { get; set; }
        public int SearchTermMinimumLength { get; set; }
    }
}
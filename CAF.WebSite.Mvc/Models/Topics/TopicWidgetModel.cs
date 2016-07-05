using CAF.WebSite.Application.WebUI.Mvc;
using System.Web.Routing;
 

namespace CAF.WebSite.Mvc.Models.Topics
{
    public partial class TopicWidgetModel : EntityModelBase
    {
        public string SystemName { get; set; }
        public string Title { get; set; }
        public string Html { get; set; }
        public string TitleTag { get; set; }

        public bool WrapContent { get; set; }
        public bool ShowTitle { get; set; }
        public bool IsBordered { get; set; }
    }
}
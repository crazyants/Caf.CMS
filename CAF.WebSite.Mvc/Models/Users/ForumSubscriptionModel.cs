

using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.Users
{
    public partial class ForumSubscriptionModel : EntityModelBase
    {
        public int ForumId { get; set; }
        public int ForumTopicId { get; set; }
        public bool TopicSubscription { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }
}

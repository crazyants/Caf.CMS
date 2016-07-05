using System.Collections.Generic;

namespace CAF.WebSite.Mvc.Models.Boards
{
    public partial class ForumTopicPageModel
    {
        public ForumTopicPageModel()
        {
            this.ForumPostModels = new List<ForumPostModel>();
        }

        public int Id { get; set; }
        public string Subject { get; set; }
        public string SeName { get; set; }

        public string WatchTopicText { get; set; }

        public bool IsUserAllowedToEditTopic { get; set; }
        public bool IsUserAllowedToDeleteTopic { get; set; }
        public bool IsUserAllowedToMoveTopic { get; set; }
        public bool IsUserAllowedToSubscribe { get; set; }

        public IList<ForumPostModel> ForumPostModels { get; set; }
        public int PostsPageIndex { get; set; }
        public int PostsPageSize { get; set; }
        public int PostsTotalRecords { get; set; }
    }
}
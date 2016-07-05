using CAF.Infrastructure.Core.Domain.Cms.Forums;


namespace CAF.WebSite.Mvc.Models.Boards
{
    public partial class ForumTopicRowModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string SeName { get; set; }
        public int LastPostId { get; set; }

        public int NumPosts { get; set; }
        public int Views { get; set; }
        public int NumReplies { get; set; }
        public ForumTopicType ForumTopicType { get; set; }

        public int UserId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string UserName { get; set; }
        public bool IsUserGuest { get; set; }

        //posts
        public int TotalPostPages { get; set; }
    }
}
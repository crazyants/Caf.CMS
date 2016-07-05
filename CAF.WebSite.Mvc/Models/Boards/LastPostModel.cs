
namespace CAF.WebSite.Mvc.Models.Boards
{
    public partial class LastPostModel
    {
        public int Id { get; set; }
        public int ForumTopicId { get; set; }
        public string ForumTopicSeName { get; set; }
        public string ForumTopicSubject { get; set; }
        
        public int UserId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string UserName { get; set; }
        public bool IsUserGuest { get; set; }

        public string PostCreatedOnStr { get; set; }
        
        public bool ShowTopic { get; set; }
    }
}
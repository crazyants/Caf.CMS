using System;

namespace CAF.WebSite.Mvc.Models.Boards
{
    public partial class ForumPostModel
    {
        public int Id { get; set; }
        public int ForumTopicId { get; set; }
        public string ForumTopicSeName { get; set; }

        public string FormattedText { get; set; }

        public bool IsCurrentUserAllowedToEditPost { get; set; }
        public bool IsCurrentUserAllowedToDeletePost { get; set; }
        
        public int UserId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string UserAvatarUrl { get; set; }
        public string UserName { get; set; }
        public bool IsUserForumModerator { get; set; }
        public bool IsUserGuest { get; set; }

        public string PostCreatedOnStr { get; set; }

        public bool ShowUsersPostCount { get; set; }
        public int ForumPostCount { get; set; }

        public bool ShowUsersJoinDate { get; set; }
        public DateTime UserJoinDate { get; set; }

        public bool ShowUsersLocation { get; set; }
        public string UserLocation { get; set; }

        public bool AllowPrivateMessages { get; set; }

        public bool SignaturesEnabled { get; set; }
        public string FormattedSignature { get; set; }

        public int CurrentTopicPage { get; set; }

    }
}
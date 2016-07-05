
using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.Users
{
    public partial class UserNavigationModel : ModelBase
    {
        public bool HideInfo { get; set; }
        public bool HideAddresses { get; set; }
        public bool HideRewardPoints { get; set; }
        public bool HideChangePassword { get; set; }
        public bool HideAvatar { get; set; }
        public bool HideForumSubscriptions { get; set; }

        public UserNavigationEnum SelectedTab { get; set; }
    }

    public enum UserNavigationEnum
    {
        Info,
        Addresses,
        RewardPoints,
        ChangePassword,
        Avatar,
        ForumSubscriptions
    }
}
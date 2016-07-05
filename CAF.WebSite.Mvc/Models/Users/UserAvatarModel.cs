

using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.Users
{
    public partial class UserAvatarModel : ModelBase
    {
        public string AvatarUrl { get; set; }
		public string MaxFileSize { get; set; }
        public UserNavigationModel NavigationModel { get; set; }
    }
}
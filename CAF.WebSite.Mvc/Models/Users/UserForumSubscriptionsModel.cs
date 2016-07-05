using System.Collections.Generic;
using CAF.Infrastructure.Core.Pages;
using CAF.WebSite.Mvc.Models.Users;

namespace CAF.WebSite.Mvc.Models.Users
{
    public partial class UserForumSubscriptionsModel : PagedListBase
    {
        public UserForumSubscriptionsModel(IPageable pageable)
            : base(pageable)
        {
            this.ForumSubscriptions = new List<ForumSubscriptionModel>();
        }

        public IList<ForumSubscriptionModel> ForumSubscriptions { get; set; }
        public UserNavigationModel NavigationModel { get; set; }
    }
}
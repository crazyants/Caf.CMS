using System.Collections.Generic;
using System.Web.Mvc;
using CAF.WebSite.Mvc.Admin.Validators.Messages;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.Mvc.Admin.Models.Messages
{
    public class NewsLetterSubscriptionListModel : ModelBase
    {
		public NewsLetterSubscriptionListModel()
		{
			AvailableSites = new List<SelectListItem>();
		}

		public int GridPageSize { get; set; }

        public IEnumerable<NewsLetterSubscriptionModel> NewsLetterSubscriptions { get; set; }

        [LangResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        public string SearchEmail { get; set; }

		[LangResourceDisplayName("Admin.Common.Site.SearchFor")]
		public int SiteId { get; set; }

		public IList<SelectListItem> AvailableSites { get; set; }
    }
}
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using System.Collections.Generic;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Topics
{
	public class TopicListModel : ModelBase
	{
		public TopicListModel()
		{
			AvailableSites = new List<SelectListItem>();
		}

		[LangResourceDisplayName("Admin.Common.Site.SearchFor")]
		public int SearchSiteId { get; set; }
		public IList<SelectListItem> AvailableSites { get; set; }
	}
}
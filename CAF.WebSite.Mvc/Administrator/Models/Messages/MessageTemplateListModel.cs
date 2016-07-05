using System.Collections.Generic;
using System.Web.Mvc;
using CAF.WebSite.Mvc.Admin.Validators.Messages;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.Mvc.Admin.Models.Messages
{
	public class MessageTemplateListModel : ModelBase
	{
		public MessageTemplateListModel()
		{
			AvailableSites = new List<SelectListItem>();
		}

		[LangResourceDisplayName("Admin.Common.Site.SearchFor")]
		public int SearchSiteId { get; set; }
		public IList<SelectListItem> AvailableSites { get; set; }
	}
}
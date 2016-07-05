using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.Settings
{
	public partial class SiteScopeConfigurationModel : ModelBase
	{
        public SiteScopeConfigurationModel()
		{
            AllSites = new List<SelectListItem>();
		}

        public int SiteId { get; set; }
        public IList<SelectListItem> AllSites { get; set; }
	}
}
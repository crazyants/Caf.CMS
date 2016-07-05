using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Mvc.Admin.Validators.Messages;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Models.Sites;


namespace CAF.WebSite.Mvc.Admin.Models.Messages
{
    [Validator(typeof(CampaignValidator))]
    public class CampaignModel : EntityModelBase
    {
        [LangResourceDisplayName("Admin.Promotions.Campaigns.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Campaigns.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Campaigns.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }
        
        [LangResourceDisplayName("Admin.Promotions.Campaigns.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Campaigns.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [LangResourceDisplayName("Admin.Promotions.Campaigns.Fields.TestEmail")]
        [AllowHtml]
        public string TestEmail { get; set; }

		[LangResourceDisplayName("Admin.Common.Site.LimitedTo")]
		public bool LimitedToSites { get; set; }

		[LangResourceDisplayName("Admin.Common.Site.AvailableFor")]
		public List<SiteModel> AvailableSites { get; set; }
		public int[] SelectedSiteIds { get; set; }
    }
}
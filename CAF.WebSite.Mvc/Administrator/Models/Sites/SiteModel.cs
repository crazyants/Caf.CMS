using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Sites;
using CAF.WebSite.Application.WebUI.Mvc;


namespace CAF.WebSite.Mvc.Admin.Models.Sites
{
    [Validator(typeof(SiteValidator))]
    public partial class SiteModel : EntityModelBase
	{
		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.Name")]
		[AllowHtml]
		public string Name { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.Url")]
		[AllowHtml]
		public string Url { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.SslEnabled")]
		public virtual bool SslEnabled { get; set; }


		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.SecureUrl")]
		[AllowHtml]
		public virtual string SecureUrl { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.Hosts")]
		[AllowHtml]
		public string Hosts { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.SiteLogo")]
		[UIHint("Picture")]
		public int LogoPictureId { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.DisplayOrder")]
		public int DisplayOrder { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.HtmlBodyId")]
		public string HtmlBodyId { get; set; }

		[LangResourceDisplayName("Admin.Configuration.Sites.Fields.ContentDeliveryNetwork")]
	    [AllowHtml]
	    public string ContentDeliveryNetwork { get; set; }
	}
}
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.Logging
{
    public class LogModel : EntityModelBase
    {
        public string LogLevelHint { get; set; }
        
        [LangResourceDisplayName("Admin.System.Log.Fields.LogLevel")]
        public string LogLevel { get; set; }

        [LangResourceDisplayName("Admin.System.Log.Fields.ShortMessage")]
        [AllowHtml]
        public string ShortMessage { get; set; }

        [LangResourceDisplayName("Admin.System.Log.Fields.FullMessage")]
        [AllowHtml]
        public string FullMessage { get; set; }

        [LangResourceDisplayName("Admin.System.Log.Fields.IPAddress")]
        [AllowHtml]
        public string IpAddress { get; set; }

        [LangResourceDisplayName("Admin.System.Log.Fields.User")]
        public int? UserId { get; set; }
        [LangResourceDisplayName("Admin.System.Log.Fields.User")]
        public string UserEmail { get; set; }

        [LangResourceDisplayName("Admin.System.Log.Fields.PageURL")]
        [AllowHtml]
        public string PageUrl { get; set; }

        [LangResourceDisplayName("Admin.System.Log.Fields.ReferrerURL")]
        [AllowHtml]
        public string ReferrerUrl { get; set; }

        [LangResourceDisplayName("Admin.System.Log.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

		[LangResourceDisplayName("Admin.System.Log.Fields.UpdatedOn")]
		public DateTime? UpdatedOn { get; set; }

		[LangResourceDisplayName("Admin.System.Log.Fields.Frequency")]
		public int Frequency { get; set; }

		[LangResourceDisplayName("Admin.System.Log.Fields.ContentHash")]
		public string ContentHash { get; set; }
    }
}
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.Logging
{
    public class LogListModel : ModelBase
    {
        public LogListModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        [LangResourceDisplayName("Admin.System.Log.List.CreatedOnFrom")]
        public DateTime? CreatedOnFrom { get; set; }

        [LangResourceDisplayName("Admin.System.Log.List.CreatedOnTo")]
        public DateTime? CreatedOnTo { get; set; }

        [LangResourceDisplayName("Admin.System.Log.List.Message")]
        [AllowHtml]
        public string Message { get; set; }

        [LangResourceDisplayName("Admin.System.Log.List.LogLevel")]
        public int LogLevelId { get; set; }

		[LangResourceDisplayName("Admin.System.Log.List.MinFrequency")]
		public int MinFrequency { get; set; }

        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
}
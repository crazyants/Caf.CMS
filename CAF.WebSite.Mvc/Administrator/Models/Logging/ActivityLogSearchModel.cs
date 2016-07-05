using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.Logging
{
    public class ActivityLogSearchModel : ModelBase
    {
        public ActivityLogSearchModel()
        {
            ActivityLogType = new List<SelectListItem>();
        }
        [LangResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOnFrom")]
        public DateTime? CreatedOnFrom { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOnTo")]
        public DateTime? CreatedOnTo { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.UserEmail")]
        [AllowHtml]
        public string UserEmail { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType")]
        public int ActivityLogTypeId { get; set; }

        [LangResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType")]
        public IList<SelectListItem> ActivityLogType { get; set; }
    }
}
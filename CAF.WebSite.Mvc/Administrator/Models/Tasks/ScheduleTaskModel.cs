using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Tasks;
using System;


namespace CAF.WebSite.Mvc.Admin.Models.Tasks
{
    [Validator(typeof(ScheduleTaskValidator))]
    public partial class ScheduleTaskModel : EntityModelBase
    {
        [LangResourceDisplayName("Admin.System.ScheduleTasks.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.System.ScheduleTasks.CronExpression")]
        public string CronExpression { get; set; }

        public string CronDescription { get; set; }

        [LangResourceDisplayName("Admin.System.ScheduleTasks.Enabled")]
        public bool Enabled { get; set; }

        [LangResourceDisplayName("Admin.System.ScheduleTasks.StopOnError")]
        public bool StopOnError { get; set; }

        [LangResourceDisplayName("Admin.System.ScheduleTasks.LastStart")]
        public DateTime? LastStart { get; set; }
        public string LastStartPretty { get; set; }

        [LangResourceDisplayName("Admin.System.ScheduleTasks.LastEnd")]
        public DateTime? LastEnd { get; set; }
        public string LastEndPretty { get; set; }

        [LangResourceDisplayName("Admin.System.ScheduleTasks.LastSuccess")]
        public DateTime? LastSuccess { get; set; }
        public string LastSuccessPretty { get; set; }

        [LangResourceDisplayName("Admin.System.ScheduleTasks.NextRun")]
        public DateTime? NextRun { get; set; }
        public string NextRunPretty { get; set; }

        public bool IsOverdue { get; set; }

        [LangResourceDisplayName("Common.Error")]
        public string LastError { get; set; }

        [LangResourceDisplayName("Common.Duration")]
        public string Duration { get; set; }

        public bool IsRunning { get; set; }
        public int? ProgressPercent { get; set; }
        public string ProgressMessage { get; set; }
        public string CancelUrl { get; set; }
        public string EditUrl { get; set; }
        public string ExecuteUrl { get; set; }

    }
}
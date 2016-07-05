using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CAF.WebSite.Mvc.Admin.Validators.Messages;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.Mvc.Admin.Models.Messages
{
    public class QueuedEmailListModel : ModelBase
    {
        public QueuedEmailListModel()
        {
            SearchLoadNotSent = true;
            SearchMaxSentTries = 10;
        }

        [LangResourceDisplayName("Admin.System.QueuedEmails.List.StartDate")]
        public DateTime? SearchStartDate { get; set; }

        [LangResourceDisplayName("Admin.System.QueuedEmails.List.EndDate")]
        public DateTime? SearchEndDate { get; set; }

        [LangResourceDisplayName("Admin.System.QueuedEmails.List.FromEmail")]
        [AllowHtml]
        public string SearchFromEmail { get; set; }

        [LangResourceDisplayName("Admin.System.QueuedEmails.List.ToEmail")]
        [AllowHtml]
        public string SearchToEmail { get; set; }

        [LangResourceDisplayName("Admin.System.QueuedEmails.List.LoadNotSent")]
        public bool SearchLoadNotSent { get; set; }

        [LangResourceDisplayName("Admin.System.QueuedEmails.List.SendManually")]
        public bool? SearchSendManually { get; set; }

        [LangResourceDisplayName("Admin.System.QueuedEmails.List.MaxSentTries")]
        public int SearchMaxSentTries { get; set; }

        [LangResourceDisplayName("Admin.System.QueuedEmails.List.GoDirectlyToNumber")]
        public int GoDirectlyToNumber { get; set; }
    }
}
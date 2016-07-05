using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    public class ArticleCommentModel : EntityModelBase
    {
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Comments.Fields.Article")]
        public int ArticleId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Comments.Fields.CommentTitle")]
        [AllowHtml]
        public string ArticleTitle { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Comments.Fields.User")]
        public int UserId { get; set; }
		public string UserName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Comments.Fields.IPAddress")]
        public string IpAddress { get; set; }

        [AllowHtml]
        [LangResourceDisplayName("Admin.ContentManagement.Articles.Comments.Fields.CommentText")]
        public string Comment { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Articles.Comments.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

    }
}
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Mvc.Validators.Boards;
using CAF.Infrastructure.Core.Domain.Cms.Forums;


namespace CAF.WebSite.Mvc.Models.Boards
{
    [Validator(typeof(EditForumPostValidator))]
    public partial class EditForumPostModel
    {
        public int Id { get; set; }
        public int ForumTopicId { get; set; }

        public bool IsEdit { get; set; }

        [AllowHtml]
        public string Text { get; set; }
        public EditorType ForumEditor { get; set; }

        public string ForumName { get; set; }
        public string ForumTopicSubject { get; set; }
        public string ForumTopicSeName { get; set; }

        public bool IsUserAllowedToSubscribe { get; set; }
        public bool Subscribed { get; set; }
    }
}
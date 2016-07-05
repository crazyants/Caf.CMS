using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.WebSite.Mvc.Validators.Boards;


namespace CAF.WebSite.Mvc.Models.Boards
{
    [Validator(typeof(EditForumTopicValidator))]
    public partial class EditForumTopicModel
    {
        public EditForumTopicModel()
        {
            TopicPriorities = new List<SelectListItem>();
        }

        public bool IsEdit { get; set; }

        public int Id { get; set; }

        public int ForumId { get; set; }
        public string ForumName { get; set; }
        public string ForumSeName { get; set; }

        public int TopicTypeId { get; set; }
        public EditorType ForumEditor { get; set; }
        [AllowHtml]
        public string Subject { get; set; }
        [AllowHtml]
        public string Text { get; set; }
        
        public bool IsUserAllowedToSetTopicPriority { get; set; }
        public IEnumerable<SelectListItem> TopicPriorities { get; set; }

        public bool IsUserAllowedToSubscribe { get; set; }
        public bool Subscribed { get; set; }

    }
}
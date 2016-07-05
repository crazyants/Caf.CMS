using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Models.Articles
{
    #region 评论

    public partial class ArticleCommentModel : EntityModelBase
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserAvatarUrl { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AllowViewingProfiles { get; set; }

    }

    public partial class AddArticleCommentModel : EntityModelBase
    {

        [LangResourceDisplayName("Articles.Comments.CommentTitle")]
        [AllowHtml]
        public string CommentTitle { get; set; }
        [LangResourceDisplayName("Articles.Comments.CommentText")]
        [AllowHtml]
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }


    #endregion

    #region 评论赞


    public partial class ArticleReviewsModel : ModelBase
    {
        public ArticleReviewsModel()
        {
            Item = new ArticleReviewModel();
            AddArticleReview = new AddArticleReviewModel();
        }
        public bool AllowUserReviews { get; set; }
        public int ArticleId { get; set; }

       // public string ArticleName { get; set; }

       // public string ArticleSeName { get; set; }

        public ArticleReviewModel Item { get; set; }
        public AddArticleReviewModel AddArticleReview { get; set; }
    }

    public partial class ArticleReviewModel : EntityModelBase
    {
        public ArticleReviewModel()
        {
            Helpfulness = new ArticleReviewHelpfulnessModel();
        }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public bool AllowViewingProfiles { get; set; }


        public int Rating { get; set; }

        public ArticleReviewHelpfulnessModel Helpfulness { get; set; }

        public string WrittenOnStr { get; set; }
    }


    public partial class ArticleReviewHelpfulnessModel : ModelBase
    {
        public int ArticleReviewId { get; set; }

        public int HelpfulYesTotal { get; set; }

        public int HelpfulNoTotal { get; set; }
    }

    public partial class AddArticleReviewModel : ModelBase
    {


        [LangResourceDisplayName("Reviews.Fields.Rating")]
        public int Rating { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool CanCurrentUserLeaveReview { get; set; }
        public bool SuccessfullyAdded { get; set; }
        public string Result { get; set; }
    }
    #endregion
}
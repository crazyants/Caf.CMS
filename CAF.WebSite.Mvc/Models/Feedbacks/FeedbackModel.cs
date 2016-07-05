using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Validators.Common;
using System;
using CAF.WebSite.Mvc.Validators.Feedbacks;


namespace CAF.WebSite.Mvc.Models.Feedbacks
{
    [Validator(typeof(FeedbackValidator))]
    public partial class FeedbackModel : ModelBase
    {
        /// <summary>
        /// 留言标题
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.Title", "留言标题")]
        public string Title { get; set; }
        /// <summary>
        /// 留言内容
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.Content", "留言内容")]
        public string Content { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.UserName", "用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.UserTel", "联系电话")]
        public string UserTel { get; set; }
        /// <summary>
        /// 联系QQ
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.UserQQ", "联系QQ")]
        public string UserQQ { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.UserEmail", "电子邮箱")]
        public string UserEmail { get; set; }
        /// <summary>
        /// 留言时间
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.AddTime", "留言时间")]
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.ReplyContent", "回复内容")]
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.ReplyTime", "回复时间")]
        public DateTime ReplyTime { get; set; }
        /// <summary>
        /// 是否锁定1是0否
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.IsLock", "锁定")]
        public bool IsLock { get; set; }
        /// <summary>
        /// 发布者IP地址
        /// </summary>
        [AllowHtml]
        [LangResourceDisplayName("Feedbacks.IPAddress", "IP地址")]
        public string IPAddress { get; set; }
        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }
        public bool DisplayCaptcha { get; set; }
    }
}
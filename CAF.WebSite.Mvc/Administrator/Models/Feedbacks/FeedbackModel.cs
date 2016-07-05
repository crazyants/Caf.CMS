using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using System;


namespace CAF.WebFeedbacks.Mvc.Admin.Models.Feedbacks
{

    public class FeedbackModel : EntityModelBase
    {
        public FeedbackModel()
        {
        }
        /// <summary>
        /// 留言标题
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.Title", "留言标题")]
        public string Title { get; set; }
        /// <summary>
        /// 留言内容
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.Content", "留言内容")]
        public string Content { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.UserName", "用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.UserTel", "联系电话")]
        public string UserTel { get; set; }
        /// <summary>
        /// 联系QQ
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.UserQQ", "联系QQ")]
        public string UserQQ { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.UserEmail", "电子邮箱")]
        public string UserEmail { get; set; }
        /// <summary>
        /// 留言时间
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.AddTime", "留言时间")]
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.ReplyContent", "回复内容")]
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.ReplyTime", "回复时间")]
        public DateTime ReplyTime { get; set; }
        /// <summary>
        /// 是否锁定1是0否
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.IsLock", "锁定")]
        public bool IsLock { get; set; }
        /// <summary>
        /// 发布者IP地址
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.IPAddress", "IP地址")]
        public string IPAddress { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Feedbacks.IsPass", "审核")]
        public bool IsPass { get; set; }
    }


}
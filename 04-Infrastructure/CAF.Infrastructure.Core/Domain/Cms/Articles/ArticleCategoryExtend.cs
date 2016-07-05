
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    /// <summary>
    /// 文章类别扩展信息
    /// </summary>
     [Serializable]
    public partial class ArticleCategoryExtend : BaseEntity
    {
         public int CategoryId { get; set; }
         public virtual ArticleCategory  Category { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
         [DataMember]
         public string Name { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
         [DataMember]
         public string Title { get; set; }
        /// <summary>
        /// 控件类型
        /// </summary>
         [DataMember]
         public string ControlType { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
         [DataMember]
         public string DataType { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
         [DataMember]
         public int DataLength { get; set; }
        /// <summary>
        /// 小数点位数
        /// </summary>
         [DataMember]
         public int DataPlace { get; set; }
        /// <summary>
        /// 选项列表
        /// </summary>
         [DataMember]
         public string ItemOption { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
         [DataMember]
         public string DefaultValue { get; set; }
        /// <summary>
        /// 是否必填0非必填1必填
        /// </summary>
         [DataMember]
         public bool IsRequired { get; set; }
        /// <summary>
        /// 是否密码框
        /// </summary>
         [DataMember]
         public bool IsPassword { get; set; }
        /// <summary>
        /// 是否允许HTML
        /// </summary>
         [DataMember]
         public bool IsHtml { get; set; }
        /// <summary>
        /// 编辑器类型0标准型1简洁型
        /// </summary>
         [DataMember]
         public int EditorType { get; set; }
        /// <summary>
        /// 验证提示信息
        /// </summary>
         [DataMember]
         public string ValidTipMsg { get; set; }
        /// <summary>
        /// 验证失败提示信息
        /// </summary>
         [DataMember]
         public string ValidErrorMsg { get; set; }
        /// <summary>
        /// 验证正则表达式
        /// </summary>
         [DataMember]
         public string ValidPattern { get; set; }
        /// <summary>
        /// 排序数字
        /// </summary>
         [DataMember]
         public int SortId { get; set; }
        /// <summary>
        /// 系统默认
        /// </summary>
         [DataMember]
         public bool IsSys { get; set; }
    }
}

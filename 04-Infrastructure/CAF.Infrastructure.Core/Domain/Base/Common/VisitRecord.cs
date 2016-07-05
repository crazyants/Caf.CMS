using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Core.Domain.Common
{
  public  class VisitRecord : BaseEntity
    {
      /// <summary>
      /// 访问来源地址
      /// </summary>
      [DataMember]
      public string VisitReffer { get; set; }
      /// <summary>
      /// 类型1推广2搜索等
      /// </summary>
      [DataMember]
      public int VisitRefferType { get; set; }
      /// <summary>
      /// 记录搜索引擎关键字
      /// </summary>
      [DataMember]
      public string VisitRefferKeyWork { get; set; }
      /// <summary>
      /// 访问页地址
      /// </summary>
      [DataMember]
      public string VisitURL{ get; set; }
      /// <summary>
      /// 访问页标题
      /// </summary>
      [DataMember]
      public string VisitTitle { get; set; }
      /// <summary>
      /// 访问者进入时间
      /// </summary>
      [DataMember]
      public DateTime VisitTimeIn { get; set; }
      /// <summary>
      /// 访问者离开时间
      /// </summary>
      [DataMember]
      public DateTime? VisitTimeOut { get; set; }
      /// <summary>
      /// 访问者IP
      /// </summary>
      [DataMember]
      public string VisitIP { get; set; }
      /// <summary>
      /// 访问者身份
      /// </summary>
      [DataMember]
      public string VisitProvince { get; set; }
      /// <summary>
      /// 访问者城市
      /// </summary>
      [DataMember]
      public string VisitCity { get; set; }
      /// <summary>
      /// 浏览器类型
      /// </summary>
      [DataMember]
      public string VisitBrowerType { get; set; }
      /// <summary>
      /// 电脑分辨率
      /// </summary>
      [DataMember]
      public string VisitResolution { get; set; }
      /// <summary>
      /// 操作系统
      /// </summary>
      [DataMember]
      public string VisitOS { get; set; }
      /// <summary>
      /// 来源编号
      /// </summary>
      [DataMember]
      public string FromSource { get; set; }
    }
}

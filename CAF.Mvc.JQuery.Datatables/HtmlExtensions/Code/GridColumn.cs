

using System;
using Newtonsoft.Json;
using CAF.Mvc.JQuery.Datatables.Models;
using Newtonsoft.Json.Linq;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    public class GridColumn
    {
        #region 字段

        #endregion

        #region 属性

        [JsonProperty("aTargets")]
        public int aTargets { get; set; }
        [JsonProperty("sType")]
        public string sType { get; set; }
        [JsonIgnore]
        public int Order { get; set; }
        /// <summary>
        /// 客户端 aoColumns 中的sName与后台传出的字段对象名称和数量,保持一致,同时 "bServerSide":false
        /// 如果"bServerSide":false时,不会向服务器发送任何的参数,服务端只需取出数据即可
        /// 用sName绑定字段
        /// </summary>
        [JsonProperty("sName")]
        public string Name { get; set; }//DataTables :: aoColumnDefs :: sName
        /// <summary>
        /// 客户端需要"bServerSide": true, 用mDataProp绑定字段,obj.aData.Id获取字段
        /// </summary>
        [JsonProperty("mDataProp")]
        public string DataProp { get; set; }
        [JsonProperty("sTitle")]
        public string Title { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
        /// <summary>
        /// 隐藏显示
        /// </summary>
        [JsonProperty("bVisible")]
        public bool Visible { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        [JsonProperty("sWidth")]
        public int? Width { get; set; }
        /// <summary>
        /// 自定义重绘
        /// </summary>
        [JsonProperty("bUseRendered")]
        public bool UseRendered { get; set; }
        /// <summary>
        /// 是否排序
        /// </summary>
        [JsonProperty("bSortable")]
        //[JsonIgnore]
        public bool Sortable { get; set; }
        /// <summary>
        /// 自定义排序
        /// </summary>
        [JsonProperty("fnRender")]
        public string FnRender { get; set; }
        /// <summary>
        /// 是否查询
        /// </summary>
        [JsonProperty("searchable")]
        public bool Searchable { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        [JsonProperty("sClass")]
        public String CssClass { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        [JsonIgnore]
        public string DisplayName { get; set; }
        /// <summary>
        /// 头部样式
        /// </summary>
        [JsonIgnore]
        public String CssClassHeader { get; set; }
        /// <summary>
        /// 排序类型
        /// </summary>
        //[JsonProperty("asSorting")]
        [JsonIgnore]
        public string Sorting
        {
            get { return SortDirection == SortDirection.None ? "" : SortDirection.ToString(); }
        }
        [JsonIgnore]
        public SortDirection SortDirection { get; set; }
        /// <summary>
        /// 自定义创建列
        /// </summary>
        [JsonProperty("fnCreatedCell")]
        public string FnCreatedCell { get; set; }
        /// <summary>
        /// 默认内容
        /// </summary>
        [JsonProperty("sDefaultContent")]
        public string DefaultContent { get; set; }
        /// <summary>
        /// 查询
        /// colDef.SearchCols["sSearch"] = new DateTime(DateTime.Now.Year, 1, 1).ToString("g") + "~" +  DateTimeOffset.Now.Date.AddDays(1).ToString("g");
        /// </summary>
        [JsonIgnore]
        public JObject SearchCols { get; set; }
        /// <summary>
        /// 自定义属性
        /// </summary>
        [JsonIgnore]
        public Attribute[] CustomAttributes { get; set; }
        /// <summary>
        /// 排序类型
        /// </summary>
        [JsonIgnore]
        public string SortType { get; set; }
        #endregion

        #region 主要方法

        /// <summary>
        /// 设置字段映射名
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="name">映射名</param>
        /// <returns></returns>
        public GridColumn(string field, string name)
        {
            Name = DataProp = field;
            Title = DisplayName = name;
            Visible = true;
            Sortable = false;
            SortDirection = SortDirection.None;
            CssClass = "";
            CssClassHeader = "";
            this.Searchable = true;
            Order = int.MaxValue;
        }
        #endregion


    }
}
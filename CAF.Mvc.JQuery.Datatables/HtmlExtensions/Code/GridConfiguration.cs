

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    [JsonObject]
    public class GridConfiguration
    {

        #region 构造函数

        public GridConfiguration(params GridColumn[] gridColumns)
        {
            var columns = new List<GridColumn>();
            columns.AddRange(gridColumns);
            dataTable = new DataTable(columns);
            GridOperation = new GridOperation();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty("keyId")]
        public string GridKey { get; set; }
        /// <summary>
        /// 表格ID
        /// </summary>
        [JsonProperty("tableId")]
        public string TableId { get; set; }
        /// <summary>
        /// 树结构
        /// </summary>
        [JsonProperty("isTree")]
        public bool IsTree { get; set; }
        [JsonProperty("treeId")]
        public string TreeId { get; set; }
        /// <summary>
        /// 查询按钮Id
        /// </summary>
        [JsonProperty("refreshBtnId")]
        public string RefreshBtnId { get; set; }
        /// <summary>
        /// 查询表单ID
        /// </summary>
        [JsonProperty("formId")]
        public string FormId { get; set; }

        [JsonProperty("operations")]
        public GridOperation GridOperation { get; set; }

        [JsonIgnore]
        public bool MultiSearch { get; set; }
        [JsonProperty("dataTable")]
        public DataTable dataTable { get; set; }

        #endregion

    }
    [JsonObject]
    public class DataTable
    {
        public DataTable(List<GridColumn> gridColumns)
        {
            GridColumns = gridColumns;
            PageLength = 10;
            StateSave = true;
            BProcessing = true;
            BServerSide = true;
            Paging = true;
        }
        #region 字段

        private string _dataType;

        #endregion
        /// <summary>
        /// 设置获取主表数据的地址
        /// </summary>
        [JsonProperty("sAjaxSource")]
        public string Url { get; set; }

        [JsonProperty("dataType")]
        public string DataType
        {
            get { return _dataType ?? "json"; }
            set { _dataType = value; }
        }
        // [JsonProperty("scrollY")]
        public string ScrollY { get; set; }
        // [JsonProperty("scrollX")]
        [JsonIgnore]
        public bool ScrollX { get; set; }
        // [JsonProperty("scrollCollapse")]
        [JsonIgnore]
        public bool ScrollCollapse { get; set; }
        [JsonProperty("paging")]
        public bool Paging { get; set; }
        [JsonProperty("bingTooltip")]
        public bool BingTooltip { get; set; }
        [JsonProperty("stateSave")]
        public bool StateSave { get; set; }
        [JsonProperty("bFilter")]
        public bool IsFilter { get; set; }
        [JsonIgnore]
        public bool AutoEencode { get; set; }
        /// <summary>
        ///  disable fixed width and enable fluid table
        /// </summary>
        [JsonProperty("AutoWidth")]
        public bool BAutoWidth { get; set; }

        /// <summary>
        /// enable/disable display message box on record load
        /// </summary>
        [JsonProperty("processing")]
        public bool BProcessing { get; set; }
        /// <summary>
        /// enable/disable server side ajax loading
        ///bServerSide":false  客户端 aoColumns 中的sName与后台传出的字段对象名称和数量,保持一致, 不会向服务器发送任何的参数,服务端只需取出数据即可
        /// bServerSide": true, 用mDataProp绑定字段,obj.aData.Id获取字段
        /// </summary>
        [JsonProperty("ServerSide")]
        public bool BServerSide { get; set; }
        /// <summary>
        /// Enable or display DataTables' ability to sort multiple columns at the 
        /// </summary>
        [JsonProperty("bSortMulti")]
        public bool BSortMulti { get; set; }
        [JsonProperty("sDom")]
        public string Dom { get; set; }
        [JsonProperty("pagingType")]
        public string PagingType { get; set; }
        [JsonProperty("pageLength")]
        public int PageLength { get; set; }
        [JsonProperty("lengthMenu")]
        public LengthMenus LengthMenu { get; set; }
        [JsonIgnore]
        public string[] ColNames
        {
            get { return GridColumns.ToList().Select(c => c.DisplayName).ToArray(); }
        }
        /// <summary>
        /// 排序参数
        /// </summary>
        //[JsonProperty("aaSorting")]
        [JsonProperty("order")]
        public dynamic AaSorting
        {
            get
            {
                var sortList = GridColumns.Select((c, idx) => c.SortDirection == SortDirection.None ? new dynamic[] { -1, "" } : (c.SortDirection == SortDirection.Ascending ? new dynamic[] { idx, "asc" } : new dynamic[] { idx, "desc" })).Where(x => x[0] > -1).ToArray();
                return sortList;
            }
        }
        /// <summary>
        /// 默认查询参数
        /// </summary>
        [JsonProperty("aoSearchCols")]
        public JToken SearchCols
        {
            get
            {
                var initialSearches = GridColumns
                    .Select(c => c.Searchable & c.SearchCols != null ? c.SearchCols : null as object).ToArray();
                return new JArray(initialSearches);
            }
        }
        /// <summary>
        /// 列参数
        /// </summary>
        [JsonProperty("aoColumnDefs")]
        public List<GridColumn> GridColumns { get; set; }
        [JsonProperty("colVis")]
        public string ColVisString { get; set; }
        /// <summary>
        /// 表格完成后执行
        /// </summary>
        [JsonProperty("fnInitComplete")]
        public string fnInitComplete { get; set; }
    }

    public class LengthMenus
    {
        public LengthMenus()
        {
            lengthInt = new int[] { 10, 25, 50, -1 };
            lengthString = new string[] { "10", "25", "50", "All" };
        }
        public int[] lengthInt { get; set; }
        public string[] lengthString { get; set; }
    }
}
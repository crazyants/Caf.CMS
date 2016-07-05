
using CAF.Infrastructure.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    /// <summary>
    /// 表格通用方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridTable<T> : IHtmlString
    {
        #region 构造函数
        public GridTable()
        {
            GridConfiguration = new GridConfiguration();
        }

        public GridTable(string gridId)
            : this()
        {
            _gridId = gridId;
        }

        public GridTable(string gridId, GridConfiguration gridConfiguration)
            : this(gridId)
        {
            GridConfiguration = gridConfiguration;
        }

        #endregion

        #region 字段
        private readonly string _gridId;
        private bool _ColumnFilter;
        private bool _ColVis;
        private bool _FixedLayout;
        private bool _IsCheckBox;
        private bool _IsFoot;
        private string _DefaultTableClass;
        private string _BindInitComplete;
        private string _BindRowCallback;
        private string _BindDataParamBinding;
        private string _BindAjaxCallback;
        private GridColumn[] _columns;

        #endregion

        #region 属性

        private GridConfiguration GridConfiguration { get; set; }
        /// <summary>
        /// 表格ID
        /// </summary>
        public string GetTableId
        {
            get
            {
                return GridConfiguration.TableId;
            }
        }
        /// <summary>
        /// 获取是否固定列
        /// </summary>
        public bool GetFixedLayout
        {
            get
            {
                return _FixedLayout;
            }
        }
        /// <summary>
        /// 获取列
        /// </summary>
        public GridColumn[] GetColumns
        {
            get
            {
                return _columns;
            }
        }
        /// <summary>
        /// 获取默认样式
        /// </summary>
        public string GetDefaultTableClass
        {
            get
            {
                return _DefaultTableClass;
            }
        }
        public bool GetIsCheckBox
        {
            get
            {
                return _IsCheckBox;
            }
        }
        public bool GetIsFoot
        {
            get
            {
                return _IsFoot;
            }
        }

        #endregion

        #region 主要方法

        /// <summary>
        /// 开启筛选
        /// </summary>
        /// <param name="columnFilter"></param>
        /// <returns></returns>
        public GridTable<T> ColumnFilter(bool columnFilter = false)
        {
            _ColumnFilter = columnFilter;
            return this;
        }
        /// <summary>
        /// 开启Header移动
        /// </summary>
        /// <param name="colVis"></param>
        /// <returns></returns>
        public GridTable<T> ColVis(bool colVis = false)
        {
            _ColVis = colVis;
            return this;
        }
        /// <summary>
        /// 开启固定列
        /// </summary>
        /// <param name="fixedLayout"></param>
        /// <returns></returns>
        public GridTable<T> FixedLayout(bool fixedLayout = false)
        {
            _FixedLayout = fixedLayout;
            return this;
        }
        /// <summary>
        /// 开启复选框
        /// </summary>
        /// <param name="IsCheckBox"></param>
        /// <returns></returns>
        public GridTable<T> IsCheckBox(bool isCheckBox = false)
        {
            _IsCheckBox = isCheckBox;
            return this;
        }
        /// <summary>
        /// 是否显示顶部
        /// </summary>
        /// <param name="isFoot"></param>
        /// <returns></returns>
        public GridTable<T> IsFoot(bool isFoot = false)
        {
            _IsFoot = isFoot;
            return this;
        }


        /// <summary>
        /// 表格Id
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public GridTable<T> TableId(string tableId)
        {
            GridConfiguration.TableId = tableId;
            return this;
        }
        /// <summary>
        /// 是否树结构
        /// </summary>
        /// <param name="treeId">树结构父级ID字段名称</param>
        /// <param name="isTree"></param>
        /// <returns></returns>
        public GridTable<T> IsTree(string treeId, bool isTree = true)
        {
            GridConfiguration.TreeId = treeId;
            GridConfiguration.IsTree = isTree;
            return this;
        }



        /// <summary>
        /// 主键ID名称
        /// </summary>
        /// <param name="gridKey"></param>
        /// <returns></returns>
        public GridTable<T> GridKey(string gridKey)
        {
            GridConfiguration.GridKey = gridKey;
            return this;
        }
        /// <summary>
        /// 刷新按钮ID
        /// </summary>
        /// <param name="refreshBtnId"></param>
        /// <returns></returns>
        public GridTable<T> RefreshBtnId(string refreshBtnId)
        {
            GridConfiguration.RefreshBtnId = refreshBtnId;
            return this;
        }
        /// <summary>
        /// 筛选表单Id
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public GridTable<T> FormId(string formId)
        {
            GridConfiguration.FormId = formId;
            return this;
        }

        /// <summary>
        /// 默认皮肤
        /// </summary>
        /// <param name="gridKey"></param>
        /// <returns></returns>
        public GridTable<T> DefaultTableClass(string defaultTableClass = "")
        {
            _DefaultTableClass = defaultTableClass.IsEmpty() ? "table table-striped table-bordered table-hover" : defaultTableClass;
            return this;
        }
        /// <summary>
        /// 启用内置操作类型
        /// </summary>
        /// <param name="gridOperatorTypes">内置操作类型</param>
        /// <returns></returns>
        public GridTable<T> BuiltInOperation(GridOperators gridOperatorTypes)
        {
            if (gridOperatorTypes.HasFlag(GridOperators.Add))
                GridConfiguration.GridOperation.Add = true;
            if (gridOperatorTypes.HasFlag(GridOperators.Edit))
                GridConfiguration.GridOperation.Edit = true;
            if (gridOperatorTypes.HasFlag(GridOperators.Delete))
                GridConfiguration.GridOperation.Delete = true;
            return this;
        }


        /// <summary>
        /// 启用分页
        /// </summary>
        /// <param name="pagerId">分页控件Id</param>
        /// <returns></returns>
        public GridTable<T> Pager(bool paging = false)
        {
            GridConfiguration.dataTable.Paging = paging;
            return this;
        }
        /// <summary>
        /// 获取数据的地址
        /// </summary>
        /// <param name="postUrl">获取数据的地址</param>
        /// <param name="postData">发送参数数据</param>
        /// <returns></returns>
        public GridTable<T> Url(string postUrl)
        {
            GridConfiguration.dataTable.Url = postUrl;
            if (string.IsNullOrEmpty(this.GridConfiguration.GridKey))
                throw new Exception("请指定表格标识列");

            return this;
        }



        /// <summary>
        /// 开启滚动
        /// </summary>
        /// <param name="scrollCollapse"></param>
        /// <returns></returns>
        public GridTable<T> ScrollCollapse(bool scrollCollapse = false)
        {
            GridConfiguration.dataTable.ScrollCollapse = scrollCollapse;
            return this;
        }
        /// <summary>
        /// 开启提示
        /// </summary>
        /// <param name="bingTooltip"></param>
        /// <returns></returns>
        public GridTable<T> BingTooltip(bool bingTooltip = false)
        {
            GridConfiguration.dataTable.BingTooltip = bingTooltip;
            return this;
        }
        /// <summary>
        /// 启用/禁用显示消息框记录加载
        /// </summary>
        /// <param name="bProcessing"></param>
        /// <returns></returns>
        public GridTable<T> BProcessing(bool bProcessing = true)
        {
            GridConfiguration.dataTable.BProcessing = bProcessing;
            return this;
        }
        /// <summary>
        /// 服务器字段绑定类型
        /// </summary>
        /// <param name="bServerSide"></param>
        /// <returns></returns>
        public GridTable<T> BServerSide(bool bServerSide = true)
        {
            GridConfiguration.dataTable.BServerSide = bServerSide;
            return this;
        }
        /// <summary>
        /// 启用/禁用在datatable中多个列进行排序的能力
        /// </summary>
        /// <param name="bSortMulti"></param>
        /// <returns></returns>
        public GridTable<T> BSortMulti(bool bSortMulti = false)
        {
            GridConfiguration.dataTable.BSortMulti = bSortMulti;
            return this;
        }
        /// <summary>
        /// 启用/禁用自动适应宽度
        /// </summary>
        /// <param name="autowidth"></param>
        /// <returns></returns>
        public GridTable<T> AutoWidth(bool autowidth = false)
        {
            GridConfiguration.dataTable.BAutoWidth = autowidth;
            return this;
        }


        /// <summary>
        /// 当设置为true时，对来自服务器的数据和提交数据进行encodes编码。如<![CDATA[<将被转换为&lt;]]>
        /// </summary>
        /// <param name="autoEncode">encodes编码</param>
        /// <returns></returns>
        public GridTable<T> AutoEencode(bool autoEncode)
        {
            GridConfiguration.dataTable.AutoEencode = autoEncode;
            return this;
        }

        /// <summary>
        ///     定义表格希望获得的数据的类型
        /// </summary>
        /// <param name="dataTypes">数据的类型</param>
        /// <returns></returns>
        public GridTable<T> DataType(ResponseDatas dataTypes)
        {
            GridConfiguration.dataTable.DataType = dataTypes.ToString().ToLower();
            return this;
        }
        /// <summary>
        /// dataTable 布局
        /// l - length changing input control
        /// f - filtering input
        /// t - The table!
        /// i - Table information summary
        /// p - pagination control
        /// r - processing display element
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        public GridTable<T> Dom(bool ShowPageSizes = false, bool ShowSearch = false)
        {
            string domr = "r", doml = "l", domC = "C", domf = "f", domt = "t", domi = "i", domp = "p";
            if (!ShowPageSizes)
            {
                doml = domr = domi = domp = "";
            }
            if (_ColVis)
            {
                domC = "";
            }
            if (!ShowSearch)
            {
                domf = "";
            }
            GridConfiguration.dataTable.Dom = string.Format("<'row'<'col-sm-6'{0}><'col-sm-6'{1}>><'row'<'col-sm-12'{2}>><'row'<'col-sm-6'{3}><'col-sm-6 text-right'{4}>><'clear'>".FormatWith(domr + doml, domC + domf, domt, domi, domp));
            return this;
        }

        /// <summary>
        /// 分页类型(bootstrap_full_number或bootstrap_extended)
        /// </summary>
        /// <param name="pagingType"></param>
        /// <returns></returns>
        public GridTable<T> PagingType(string pagingType)
        {
            GridConfiguration.dataTable.PagingType = pagingType;
            return this;
        }
        /// <summary>
        /// 分页默认页数
        /// </summary>
        /// <param name="PageLength"></param>
        /// <returns></returns>
        public GridTable<T> PageLength(int PageLength)
        {
            GridConfiguration.dataTable.PageLength = PageLength;
            return this;
        }
        /// <summary>
        /// 分页页数菜单
        /// </summary>
        /// <param name="lengthInt"></param>
        /// <param name="lengthString"></param>
        /// <returns></returns>
        public GridTable<T> LengthMenu(int[] lengthInt, string[] lengthString)
        {
            GridConfiguration.dataTable.LengthMenu.lengthString = lengthString;
            GridConfiguration.dataTable.LengthMenu.lengthInt = lengthInt;
            return this;
        }

        /// <summary>
        ///     表格高度。可为数值、百分比或auto
        /// </summary>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public GridTable<T> ScrollY(string scrollY)
        {
            GridConfiguration.dataTable.ScrollY = scrollY;
            return this;
        }
        /// <summary>
        /// 开启横滚动
        /// </summary>
        /// <param name="scrollX"></param>
        /// <returns></returns>
        public GridTable<T> ScrollX(bool scrollX = false)
        {
            GridConfiguration.dataTable.ScrollX = scrollX;
            return this;
        }
        /// <summary>
        /// 绑定完成方法
        /// </summary>
        /// <param name="fnInitComplete"></param>
        /// <returns></returns>
        public GridTable<T> BindInitComplete(string fnInitComplete)
        {
            _BindInitComplete = fnInitComplete;
            return this;
        }
        /// <summary>
        /// 绑定行创建回调函数
        /// </summary>
        /// <param name="fnInitComplete"></param>
        /// <returns></returns>
        public GridTable<T> BindRowCallback(string fnRowCallback)
        {
            _BindRowCallback = fnRowCallback;
            return this;
        }
        /// <summary>
        /// 绑定自定义参数函数
        /// </summary>
        /// <param name="fnInitComplete"></param>
        /// <returns></returns>
        public GridTable<T> BindDataParamBinding(string fnDataParamBinding)
        {
            _BindDataParamBinding = fnDataParamBinding;
            return this;
        }
        /// <summary>
        /// 绑定AJAX回调函数
        /// </summary>
        /// <param name="fnAjaxCallback"></param>
        /// <returns></returns>
        public GridTable<T> BindAjaxCallback(string fnAjaxCallback)
        {
            _BindAjaxCallback = fnAjaxCallback;
            return this;
        }

        /// <summary>
        /// 表格列配置
        /// </summary>
        /// <param name="gridColumns">有效列</param>
        /// <returns></returns>
        public GridTable<T> MainGrid(params GridColumn[] gridColumns)
        {
            int index = 0;
            gridColumns.Each(x =>
            {
                x.aTargets = index;
                index = index + 1;
            });
            GridConfiguration.dataTable.GridColumns = gridColumns.ToList();
            _columns = gridColumns;
            return this;
        }

        /// <summary>
        /// 表格生成器
        /// </summary>
        /// <returns></returns>
        public string ToHtmlString()
        {
            // var tableContainer = TableBuilder();
            //===============JS代码==================//
            var functionHtml = new StringBuilder();
            if (!string.IsNullOrEmpty(_BindInitComplete))
                functionHtml.AppendFormat("Datatable.bindInitComplete = {0};", _BindInitComplete);
            if (!string.IsNullOrEmpty(_BindDataParamBinding))
                functionHtml.AppendFormat("Datatable.bindDataParamBinding = {0};", _BindDataParamBinding);
            if (!string.IsNullOrEmpty(_BindRowCallback))
                functionHtml.AppendFormat("Datatable.bindRowCallback = {0};", _BindRowCallback);
            if (!string.IsNullOrEmpty(_BindAjaxCallback))
                functionHtml.AppendFormat("Datatable.bindAjaxCallback = {0};", _BindAjaxCallback);
            var gridHtml = string.Format("var {0} = Datatable.init(", _gridId);
            // var jsContainer = new TagBuilder("script");
            var jsBulider = new StringBuilder();

            //表格初始化
            jsBulider.AppendFormat("{0}", GridConfiguration.ToSerializerIgnoreNullValue());
            jsBulider.Append(");");

            // jsBulider.AppendFormat("window['{0}'] = {1};", _tableName, _tableName);
            if (_ColVis)
                jsBulider.AppendFormat("new $.fn.dataTable.ColReorder({0}.getDataTable());", _gridId);
            if (_FixedLayout)
                jsBulider.AppendFormat("new $.fn.dataTable.FixedColumns({1}.getDataTable(),{ leftColumns: \"{0}\"});", 1, _gridId);
            if (GridConfiguration.IsTree)
                jsBulider.Append("$('#" + GridConfiguration.TableId + "').treetable({ expandable: false });");
            //===============JS代码==================//
            string InnerHtml = functionHtml.ToString().Replace("\"&", "").Replace("&\"", "") + gridHtml + jsBulider.ToString().Replace("\"&", "").Replace("&\"", "");
            return InnerHtml;
        }
        /// <summary>
        /// 输出HTML
        /// </summary>
        /// <returns></returns>
        //private StringBuilder TableBuilder()
        //{
        //    //生产<table></table>
        //    var tabBuider = new TagBuilder("table");
        //    tabBuider.GenerateId(_gridId);
        //    var builder = new StringBuilder(tabBuider.ToString());
        //    if (!_hasPager) return builder;
        //    //生产<div></div>
        //    var pagerBuilder = new TagBuilder("div");
        //    pagerBuilder.GenerateId(_pagerId);
        //    builder.Append(pagerBuilder);
        //    return builder;
        //}

        public override string ToString()
        {
            var result = ToHtmlString();
            return result;
        }
        #endregion


    }
}

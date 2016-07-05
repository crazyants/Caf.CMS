

namespace CAF.Mvc.JQuery.Datatables.Core
{
    /// <summary>
    /// 表格列参数通用方法
    /// </summary>
    public static class GridColumnBuilder
    {
        /// <summary>
        /// 设置列的初始宽度，可用pixels和百分比
        /// </summary>
        /// <param name="width">初始宽度</param>
        /// <returns></returns>
        public static GridColumn Width(this GridColumn col, int width)
        {
            col.Width = width;
            return col;
        }

        /// <summary>
        /// 定义初始化时，列是否隐藏
        /// </summary>
        public static GridColumn Hidden(this GridColumn col, bool hidden = true)
        {
            col.Visible = hidden;
            return col;
        }
        /// <summary>
        /// 定义字段数据源
        /// </summary>
        /// <param name="col"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GridColumn Data(this GridColumn col, string data = null)
        {
            col.Data = data;
            return col;
        }
 
        
        /// <summary>
        /// 定义定义字段是否可编辑
        /// </summary>
        //public static GridColumn Editable(this GridColumn col, ColumnEdits edittype)
        //{
        //    col.Editable = true;
        //    col.EditType = edittype.ToString().ToLower();
        //    return col;
        //}

        /// <summary>
        /// 定义定义字段是否可编辑
        /// </summary>
        //public static GridColumn Formatter(this GridColumn col, string cellformater)
        //{
        //    col.Formatter = cellformater;
        //    return col;
        //}

        /// <summary>
        /// 定义搜索 
        /// </summary>
        //public static GridColumn Searchable(this GridColumn col, CellTypes filedType = CellTypes.String, ColumnSearchs columnSearch = ColumnSearchs.Text)
        //{
        //    col.Search = true;
        //    col.SearchFiledType = filedType;
        //    col.SearchType = columnSearch.ToString().ToLower();
        //    return col;
        //}
        public static GridColumn DefaultContent(this GridColumn col, string defaultContent)
        {
            col.Name = string.Empty;
            col.DataProp = string.Empty;
            col.DefaultContent = defaultContent;
            return col;
        }


        /// <summary>
        /// 启用排序
        /// </summary>
        /// <param name="columnSorts">排序类型</param>
        /// <returns></returns>
        public static GridColumn Sortable(this GridColumn col, ColumnSorts columnSorts = ColumnSorts.Text)
        {
            col.Sortable = true;
            col.SortType = columnSorts.ToString().ToLower();
            return col;
        }
        /// <summary>
        /// 启用排序
        /// </summary>
        /// <param name="columnSorts">排序类型</param>
        /// <returns></returns>
        public static GridColumn Sortable(this GridColumn col, SortDirection columnSorts = SortDirection.None)
        {
            col.Sortable = true;
            col.SortDirection = columnSorts;
            return col;
        }
        /// <summary>
        /// 列类型
        /// </summary>
        /// <param name="col"></param>
        /// <param name="cellType"></param>
        /// <returns></returns>
        public static GridColumn CellType(this GridColumn col, CellTypes cellType = CellTypes.String)
        {
            col.sType = cellType.ToString().ToLower();
            return col;
        }
    }
}
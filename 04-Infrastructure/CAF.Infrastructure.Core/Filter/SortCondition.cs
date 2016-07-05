using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CAF.Infrastructure.Core.Filter
{
    /// <summary>
    /// 列表字段排序条件
    /// </summary>
    public class SortCondition
    {
        /// <summary>
        /// 构造一个指定字段名称的升序排序的排序条件
        /// </summary>
        /// <param name="sortField">字段名称</param>
        public SortCondition(string sortField)
            : this(sortField, ListSortDirection.Ascending)
        { }

        /// <summary>
        /// 构造一个排序字段名称和排序方式的排序条件
        /// </summary>
        /// <param name="sortField">字段名称</param>
        /// <param name="listSortDirection">排序方式</param>
        public SortCondition(string sortField, ListSortDirection listSortDirection)
        {
            SortField = sortField;
            ListSortDirection = listSortDirection;
        }
        /// <summary>
        /// 构造一个排序字段名称和排序方式的排序条件
        /// </summary>
        /// <param name="sortField">字段名称</param>
        /// <param name="descending">是否降序</param>
        public SortCondition(string sortField, bool descending)
        {
            SortField = sortField;
            ListSortDirection = descending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }
        /// <summary>
        /// 获取或设置 排序字段名称
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 获取或设置 排序方向
        /// </summary>
        public ListSortDirection ListSortDirection { get; set; }
    }
}
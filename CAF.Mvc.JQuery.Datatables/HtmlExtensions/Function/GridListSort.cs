
using System;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    /// <summary>
    ///     IList排序类
    /// </summary>

    public class GridListSort<T>
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="propertyName">排序字段属性名</param>
        /// <param name="isAsc">true升序 false 降序 不指定则为true</param>
        public GridListSort(string propertyName, bool isAsc)
        {
            PropertyName = propertyName;
            IsAsc = isAsc;
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="propertyName">排序字段属性名</param>
        public GridListSort(string propertyName)
        {
            PropertyName = propertyName;
            IsAsc = true;
        }


        private string PropertyName { get; set; }


        private bool IsAsc { get; set; }
         
        /// <summary>
        ///     比较大小 返回值 小于零则X小于Y，等于零则X等于Y，大于零则X大于Y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T x, T y)
        {
            var property = typeof (T).GetProperty(PropertyName);
            switch (property.PropertyType.Name)
            {
                case "Int32":
                    var int1 = 0;
                    var int2 = 0;
                    if (property.GetValue(x, null) != null)
                    {
                        int1 = Convert.ToInt32(property.GetValue(x, null));
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        int2 = Convert.ToInt32(property.GetValue(y, null));
                    }
                    return IsAsc ? int2.CompareTo(int1) : int1.CompareTo(int2);
                case "Double":
                    double double1 = 0;
                    double double2 = 0;
                    if (property.GetValue(x, null) != null)
                    {
                        double1 = Convert.ToDouble(property.GetValue(x, null));
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        double2 = Convert.ToDouble(property.GetValue(y, null));
                    }
                    return IsAsc ? double2.CompareTo(double1) : double1.CompareTo(double2);
                case "String":
                    var string1 = string.Empty;
                    var string2 = string.Empty;
                    if (property.GetValue(x, null) != null)
                    {
                        string1 = property.GetValue(x, null).ToString();
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        string2 = property.GetValue(y, null).ToString();
                    }
                    return IsAsc
                        ? String.Compare(string2, string1, StringComparison.Ordinal)
                        : String.Compare(string1, string2, StringComparison.Ordinal);
                case "DateTime":
                    var dateTime1 = DateTime.Now;
                    var dateTime2 = DateTime.Now;
                    if (property.GetValue(x, null) != null)
                    {
                        dateTime1 = Convert.ToDateTime(property.GetValue(x, null));
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        dateTime2 = Convert.ToDateTime(property.GetValue(y, null));
                    }
                    return IsAsc ? dateTime2.CompareTo(dateTime1) : dateTime1.CompareTo(dateTime2);
            }
            return 0;
        }
    }
}
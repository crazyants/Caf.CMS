using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    /// <summary>
    /// Grid工厂。
    /// </summary>
    public class GridFacotory
    {
        /// <summary>
        /// Html助手。
        /// </summary>
        internal HtmlHelper HtmlHelper { get; set; }

        /// <summary>
        /// 初始化一个新的 Grid工厂。
        /// </summary>
        /// <param name="htmlHelper">Html 助手。</param>
        public GridFacotory(HtmlHelper htmlHelper)
        {
            HtmlHelper = htmlHelper;
        }
        public GridColumn GridColumn(string name, int? width)
        {
            var column = new GridColumn("", name)
            {
                Width = width ?? default(int),
            };
            return column;
        }
        public GridColumn GridColumn(string field, string name, int? width)
        {
            var column = new GridColumn(field, name)
            {
                Width = width ?? default(int),
            };
            return column;
        }
    }

    /// <summary>
    /// Grid 泛型工厂。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public sealed class GridFacotory<TModel> : GridFacotory
    {
        /// <summary>
        /// Html助手。
        /// </summary>
        internal new HtmlHelper<TModel> HtmlHelper { get; set; }
        internal new ViewDataDictionary<TModel> ViewData { get; set; }
        /// <summary>
        /// 初始化一个新的 Grid 工厂。
        /// </summary>
        /// <param name="htmlHelper">Html 助手。</param>
        public GridFacotory(HtmlHelper<TModel> htmlHelper)
            : base(htmlHelper)
        {
            HtmlHelper = htmlHelper;
            ViewData = new ViewDataDictionary<TModel>();
        }
        public GridFacotory(HtmlHelper htmlHelper)
            : base(htmlHelper)
        {
         
            ViewData = new ViewDataDictionary<TModel>();
        }
        /// <summary>
        /// 创建表格列
        /// </summary>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="expression">属性表达式</param>
        /// <param name="width">宽度</param>
        /// <param name="isEdit">是否允许编辑</param>
        public GridColumn GridColumn<TProperty>(Expression<Func<TModel, TProperty>> expression, int? width = null)
        {

            var currentProp = ModelMetadata.FromLambdaExpression(expression, ViewData);

            var column = new GridColumn(currentProp.PropertyName, currentProp.DisplayName)
            {
                Width = width ?? default(int),
            };
            return column;
        }


        /// <summary>
        /// 创建排序属性
        /// </summary>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="expression">属性表达式</param>
        public string Param<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {

            var currentProp = ModelMetadata.FromLambdaExpression(expression, ViewData);

            return currentProp.PropertyName;
        }


    }
}

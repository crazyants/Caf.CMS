
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Data.Entity;


namespace CAF.Mvc.JQuery.Datatables.Core
{
    public static class GridPagerExtention
    {
        /// <summary>
        /// 接管IList
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="list">数据集</param>
        /// <param name="expression">过滤表达式</param>
        /// <param name="baseContent">控制器对象</param>
        /// <returns></returns>
        public static string Pagination<T>(this List<T> list, Controller baseContent, Expression<Func<T, bool>> expression = null) where T : class
        {
            var pageParams = RequestHelper.InitRequestParams(baseContent);
            var predicate = PredicateBuilder.True<T>();
            if (expression != null) predicate = predicate.And(expression);
            if (!string.IsNullOrEmpty(pageParams.Filters))
                predicate = GridSearchPredicate.MultiSearchExpression<T>(pageParams.Filters);
            if (string.IsNullOrEmpty(pageParams.Filters) && !string.IsNullOrEmpty(pageParams.SearchField))
                predicate = GridSearchPredicate.SingleSearchExpression<T>(pageParams.SearchField, pageParams.SearchOper, pageParams.SearchString);
            var recordes = list.Where(predicate.Compile()).Skip((pageParams.PageIndex - 1) * pageParams.PageSize).Take(pageParams.PageSize).ToList();
            recordes.Sort(new GridListSort<T>(pageParams.SortName, pageParams.Sord == "desc").Compare);
            var gridCells = recordes.Select(p => new GridCell
            {
                Id = p.GetType().GetProperty("Id").GetValue(p, null).ToString(),
                Cell = GetObjectPropertyValues(p, pageParams.Displayfileds.Split(new[] { ',' }))
            }).ToList();

            var result = new { pageParams.PageIndex, records = list.Count, rows = gridCells, total = (int)Math.Ceiling((double)list.Count / pageParams.PageSize) }.ToSerializer();
            return result;
        }



        public static string PushSubGrid<T>(this IEnumerable<T> list, Controller baseContent) where T : class
        {
            var pageParams = RequestHelper.InitRequestParams(baseContent);
            var gridCells = list.Select(p => new GridCell
            {
                Id = p.GetType().GetProperty("Id").GetValue(p, null).ToString(),
                Cell = GetObjectPropertyValues(p, pageParams.Displayfileds.Split(new[] { ',' }))
            }).ToList();

            var result = new { rows = gridCells }.ToSerializer();
            return result;
        }

        /// <summary>
        /// 接管IList
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="repository">数据仓库</param>
        /// <param name="expression">过滤表达式</param>
        /// <param name="baseContent">控制器对象</param>
        /// <returns></returns>
        //public static string Pagination<T>(this DbSet<T> repository, Controller baseContent, Expression<Func<T, bool>> expression = null) where T : class
        //{
        //    var pageParams = RequestHelper.InitRequestParams(baseContent);
        //    var predicate = PredicateBuilder.True<T>();
        //    if (expression != null) predicate = predicate.And(expression);
        //    if (!string.IsNullOrEmpty(pageParams.Filters))
        //        predicate = GridSearchPredicate.MultiSearchExpression<T>(pageParams.Filters);
        //    if (string.IsNullOrEmpty(pageParams.Filters) && !string.IsNullOrEmpty(pageParams.SearchField))
        //        predicate = GridSearchPredicate.SingleSearchExpression<T>(pageParams.SearchField, pageParams.SearchOper, pageParams.SearchString);
        //    var recordes = repository.Where(predicate).Skip((pageParams.PageIndex - 1) * pageParams.PageSize).Take(pageParams.PageSize).ToList();
        //    recordes.Sort(new GridListSort<T>(pageParams.SortName, pageParams.Sord == "desc").Compare);
        //    var gridCells = recordes.Select(person => new GridCell
        //    {
        //        Id = person.GetType().GetProperty("Id").GetValue(person, null).ToString(),
        //        Cell = GetObjectPropertyValues(person, pageParams.Displayfileds.Split(new[] { ',' }))
        //    }).ToList();

        //    var result = new { pageParams.PageIndex, records = recordes.Count, rows = gridCells, total = (int)Math.Ceiling((double)repository.Count() / pageParams.PageSize) }.ToSerializer();
        //    return result;
        //}

        private static string[] GetObjectPropertyValues<T>(T t, IEnumerable<string> filderSortOrder)
        {
            var type = typeof(T);
            return filderSortOrder.Select(filedName => type.GetProperty(filedName).GetValue(t, null).ToString()).ToArray();
        }
    }
}


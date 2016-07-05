using CAF.Infrastructure.Core.Filter;
using CAF.Infrastructure.Core.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace CAF.Infrastructure.Core
{

    public static class CollectionExtensions
    {

        public static void AddRange<T>(this ICollection<T> initial, IEnumerable<T> other)
        {
            if (other == null)
                return;

            var list = initial as List<T>;

            if (list != null)
            {
                list.AddRange(other);
                return;
            }

            other.Each(x => initial.Add(x));
        }

        //public static bool IsNullOrEmpty(this ICollection source)
        //{
        //    return (source == null || source.Count == 0);
        //}

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return (source == null || source.Count == 0);
        }

        public static bool EqualsAll<T>(this IList<T> a, IList<T> b)
        {
            if (a == null || b == null)
                return (a == null && b == null);

            if (a.Count != b.Count)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < a.Count; i++)
            {
                if (!comparer.Equals(a[i], b[i]))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// 从指定<see cref="IQueryable{T}"/>集合 中查询指定分页条件的子数据集
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体主键类型</typeparam>
        /// <param name="source">要查询的数据集</param>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="pageCondition">分页查询条件</param>
        /// <param name="total">输出符合条件的总记录数</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity, TKey>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
             PageCondition pageCondition,
            out int total) where TEntity : BaseEntity
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(predicate, "predicate");
            Guard.ArgumentNotNull(pageCondition, "pageCondition");

            return source.Where<TEntity, TKey>(predicate, pageCondition.PageIndex, pageCondition.PageSize, out total, pageCondition.SortConditions);
        }


        /// <summary>
        /// 从指定<see cref="IQueryable{T}"/>集合 中查询指定分页条件的子数据集
        /// </summary>
        /// <typeparam name="TEntity">动态实体类型</typeparam>
        /// <typeparam name="TKey">实体主键类型</typeparam>
        /// <param name="source">要查询的数据集</param>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="total">输出符合条件的总记录数</param>
        /// <param name="sortConditions">排序条件集合</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity, TKey>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            out int total,
            SortCondition[] sortConditions = null) where TEntity : BaseEntity
        {

            total = source.Count(predicate);
            source = source.Where(predicate);
            if (sortConditions == null || sortConditions.Length == 0)
            {
                source = source.OrderBy(m => m.Id);
            }
            else
            {
                int count = 0;
                IOrderedQueryable<TEntity> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? QueryablePropertySorter<TEntity>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : QueryablePropertySorter<TEntity>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }
                source = orderSource;
            }

            return source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<TEntity>().AsQueryable();
        }
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source,
          int pageIndex,
          int pageSize,
          out int total)
        {

            total = source.Count();
            source = source.OrderBy(p => "");
            return source != null
                ? source.Skip(pageIndex  * pageSize).Take(pageSize)
                : Enumerable.Empty<TEntity>().AsQueryable();
        }


        public static IList<TEntity> Where<TEntity>(this IList<TEntity> source,
         int pageIndex,
         int pageSize,
         out int total) where TEntity : BaseEntity
        {
            total = source.Count();
            return source != null
                ? source.Skip(pageIndex * pageSize).Take(pageSize).ToList()
                : Enumerable.Empty<TEntity>().ToList();
        }

    }

}

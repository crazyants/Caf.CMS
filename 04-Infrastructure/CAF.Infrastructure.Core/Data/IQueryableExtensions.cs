using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
 

namespace CAF.Infrastructure.Core.Data
{
	public static class IQueryableExtensions
	{

		/// <summary>
		/// Instructs the repository to eager load entities that may be in the type's association path.
		/// </summary>
		/// <param name="query">A previously created query object which the expansion should be applied to.</param>
		/// <param name="path">
		/// The path of the child entities to eager load.
		/// Deeper paths can be specified by separating the path with dots.
		/// </param>
		/// <returns>A new query object to which the expansion was applied.</returns>
		public static IQueryable<T> Expand<T>(this IQueryable<T> query, string path) where T : BaseEntity
		{
			Guard.ArgumentNotNull(query, "query");
			Guard.ArgumentNotEmpty(path, "path");

			return query.Include(path);
		}

		/// <summary>
		/// Instructs the repository to eager load entities that may be in the type's association path.
		/// </summary>
		/// <param name="query">A previously created query object which the expansion should be applied to.</param>
		/// <param name="path">The path of the child entities to eager load.</param>
		/// <returns>A new query object to which the expansion was applied.</returns>
		public static IQueryable<T> Expand<T, TProperty>(this IQueryable<T> query, Expression<Func<T, TProperty>> path) where T : BaseEntity
		{
			Guard.ArgumentNotNull(query, "query");
			Guard.ArgumentNotNull(path, "path");

			return query.Include(path);
		}
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }
        public static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
	}
}

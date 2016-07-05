using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    public static class GridSearchPredicate
    {
        public static Expression<Func<T, bool>> SingleSearchExpression<T>(string searchField, string searchOper, string searchString) where T : class
        {
            var predicate = PredicateBuilder.True<T>();
            if (string.IsNullOrEmpty(searchField)) return predicate;
            if (searchOper == "eq")
                predicate =
                    predicate.And(x => x.GetType().GetProperty(searchField).GetValue(x, null).ToString() == searchString);
            if (searchOper == "bw")
                predicate =
                    predicate.And(
                        x => x.GetType().GetProperty(searchField).GetValue(x, null).ToString().StartsWith(searchString));
            if (searchOper == "cn")
                predicate =
                    predicate.And(
                        x => x.GetType().GetProperty(searchField).GetValue(x, null).ToString().Contains(searchString));
            var filedType = typeof(T).GetProperty(searchField).PropertyType.Name;
            if (searchOper == "gt" && (filedType == "Int32" || filedType == "Double"))
                predicate =
                    (x =>
                        double.Parse(x.GetType().GetProperty(searchField).GetValue(x, null).ToString()) >
                        Double.Parse(searchString));
            if (searchOper == "lt" && (filedType == "Int32" || filedType == "Double"))
                predicate =
                    (x =>
                        double.Parse(x.GetType().GetProperty(searchField).GetValue(x, null).ToString()) <
                        Double.Parse(searchString));
            return predicate;
        }

        public static Expression<Func<T, bool>> MultiSearchExpression<T>(string filtersContent) where T : class
        {
            var predicate = PredicateBuilder.True<T>();
            var filters = JsonConvert.DeserializeObject<GridFilters>(filtersContent);
            predicate = filters.GroupOp == "AND" ? filters.Rules.Aggregate(predicate, (current, rule) => current.And(SingleSearchExpression<T>(rule.Field, rule.Op, rule.Data))) : filters.Rules.Aggregate(predicate, (current, rule) => current.Or(SingleSearchExpression<T>(rule.Field, rule.Op, rule.Data)));
            return predicate;
        }
    }
}
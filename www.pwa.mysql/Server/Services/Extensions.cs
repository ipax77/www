
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace www.pwa.Server.Services {
        public static class Extensions {
        /// <summary>
        /// Builds the Queryable functions using a TSource property name.
        /// </summary>
        public static IOrderedQueryable<T> CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName,
                IComparer<object> comparer = null)
        {
            var param = Expression.Parameter(typeof(T), "x");

            var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            return comparer != null
                ? (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param),
                        Expression.Constant(comparer)
                    )
                )
                : (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param)
                    )
                );
        }
    }
}

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using www.pwa.Shared;

namespace www.pwa.Server.Services
{
    public static class Extensions
    {
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

        public static List<SchoolClass> SortClasses(List<SchoolClass> classes)
        {
            Regex rx = new Regex(@"^(\d+)(.*)");
            List<SortHelper> sorted = new List<SortHelper>();
            foreach (var c in classes)
            {
                var match = rx.Match(c.Name);
                if (match.Success)
                {
                    sorted.Add(new SortHelper()
                    {
                        number = int.Parse(match.Groups[1].Value),
                        letters = match.Groups[2].Value
                    });
                } else {
                    sorted.Add(new SortHelper()
                    {
                        number = 0,
                        letters = c.Name
                    });
                }
            }

            List<SchoolClass> sortedClasses = new List<SchoolClass>();
            foreach (var ent in sorted.OrderBy(o => o.number).ThenBy(t => t.letters)) {
                sortedClasses.Add(new SchoolClass()
                {
                    Name = ent.number == 0 ? ent.letters : $"{ent.number}{ent.letters}"
                });
            }
            return sortedClasses;
        }

        public class SortHelper
        {
            public int number { get; set; }
            public string letters { get; set; }
        }
    }
}
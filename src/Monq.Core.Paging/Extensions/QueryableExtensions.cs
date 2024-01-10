using Monq.Core.Paging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Monq.Core.Paging.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Orderings the by.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="propertyName">Name of the field.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="isSubsequent">The option whether sorting is a subsequent.</param>
        public static IQueryable<TSource> OrderByProperty<TSource>(this IQueryable<TSource> data, string propertyName, string? dir, bool isSubsequent = false)
        {
            var par = Expression.Parameter(typeof(TSource), "col");
            var propType = typeof(TSource).GetPropertyType(propertyName);
            if (propType is null)
                throw new ArgumentException($"{typeof(TSource)} doest not contain a property {propertyName} ", nameof(propertyName));

            // Проверки на Null требуется проводить только для сортировки по вложенным свойствам.
            Expression propExpr = propertyName.Contains(".")
                ? par.GetPropertyExpression(propertyName)
                : par.GetPropertyExpressionUnSafe(propertyName);

            // Декомпилируем свойства помеченные как Computed, чтобы EF мог их правильно воспринимать.
            var lambda = Expression.Lambda(propExpr.Decompile().ExpressionCallsToConstants(), par);

            if (string.IsNullOrEmpty(dir) || dir.ToLower() == "asc")
                return !isSubsequent
                    ? data.OrderBy(lambda)
                    : data.ThenBy(lambda);

            return !isSubsequent
                ? data.OrderByDescending(lambda)
                : data.ThenByDescending(lambda);
        }

        /// <summary>
        /// Построить выражение для поисковой фразы <paramref name="search"/> с параметрами <paramref name="searching"/>.
        /// </summary>
        /// <typeparam name="TSource">Тип модели для которой будет построено выражение.</typeparam>
        /// <param name="search">Поисковая фраза.</param>
        /// <param name="searching">Параметры поиска.</param>
        internal static Expression<Func<TSource, bool>>? GetExpressionForSearch<TSource>(this Searching? searching, string? search)
        {
            if (string.IsNullOrEmpty(search))
                return null;

            searching ??= new Searching();

            var props = typeof(TSource).GetPublicProperties(searching.Depth, searching.InSearch).ToList();
            var stringProperties = props.Where(p => p.Property.PropertyType == typeof(string)).ToList();

            var parameter = Expression.Parameter(typeof(TSource), "t");
            var methodContains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            var methodToLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
            var methodToString = typeof(object).GetMethod(nameof(ToString));
            var methodIsNullOrEmpty = typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) });

            var value = Expression.Constant(search.ToLower(), typeof(string));

            var expList = new List<BinaryExpression>();

            foreach (var (fullName, _) in stringProperties)
            {
                var expMember = parameter.GetPropertyExpressionUnSafe(fullName);
                var extIsNullOrEmpty = Expression.Not(Expression.Call(methodIsNullOrEmpty, expMember.AddNullConditions()));

                var expLower = Expression.Call(expMember, methodToLower);
                var expContains = Expression.Call(expLower, methodContains, value);

                var extAnd = Expression.AndAlso(extIsNullOrEmpty, expContains);
                expList.Add(extAnd);
            }

            if (long.TryParse(search, out _) || Guid.TryParse(search, out _))
            {
                var valueProperties = props.Where(p =>
                    p.Property.PropertyType == typeof(int)
                    || p.Property.PropertyType == typeof(long)
                    || p.Property.PropertyType == typeof(Guid));

                var expression = Expression.Constant(true);
                foreach (var (fullName, _) in valueProperties)
                {
                    var expMember = parameter.GetPropertyExpression(fullName);
                    var ext = Expression.Call(expMember, methodToString);
                    var extContains = Expression.Call(ext, methodContains, value);
                    var extAnd = Expression.Equal(extContains, expression);
                    expList.Add(extAnd);
                }
            }

            if (expList.Count == 0)
                return null;

            var body = expList.Aggregate(Expression.OrElse);
            return Expression.Lambda<Func<TSource, bool>>(body, parameter);
        }
    }
}
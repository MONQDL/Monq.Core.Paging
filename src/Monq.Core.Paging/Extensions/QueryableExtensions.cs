using Monq.Core.Paging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Monq.Core.Paging.Extensions
{
    public static class QueryableExtensions
    {
        // if it needs checking search value - method TryParse else - null
        static readonly Dictionary<Type, MethodInfo?> _supportedTypes = new()
        {
            [typeof(int)] = typeof(int).GetMethod(
                nameof(int.TryParse),
                new Type[] { typeof(string), typeof(int).MakeByRefType() })!,
            [typeof(long)] = typeof(int).GetMethod(
                nameof(int.TryParse),
                new Type[] { typeof(string), typeof(int).MakeByRefType() })!,
            [typeof(Guid)] = null,
        };

        static readonly MethodInfo _methodContains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
        static readonly MethodInfo _methodToString = typeof(object).GetMethod(nameof(ToString))!;
        static readonly MethodInfo _methodToLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        static readonly MethodInfo _methodIsNullOrEmpty = typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) })!;

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

            if (!props.Any())
                return null;

            var parameter = Expression.Parameter(typeof(TSource), "t");
            var value = Expression.Constant(search.ToLower(), typeof(string));
            var expList = new List<BinaryExpression>();

            foreach (var (fullName, _) in stringProperties)
            {
                var expMember = parameter.GetPropertyExpressionUnSafe(fullName);
                var extIsNullOrEmpty = Expression.Not(Expression.Call(_methodIsNullOrEmpty, expMember.AddNullConditions()));

                var expLower = Expression.Call(expMember, _methodToLower);
                var expContains = Expression.Call(expLower, _methodContains, value);

                var extAnd = Expression.AndAlso(extIsNullOrEmpty, expContains);
                expList.Add(extAnd);
            }

            var valueExpressions = FormExpressionsForSupportedTypes(
                props,
                parameter,
                search);

            expList.AddRange(valueExpressions);

            if (expList.Count == 0)
                return null;

            var body = expList.Aggregate(Expression.OrElse);
            return Expression.Lambda<Func<TSource, bool>>(body, parameter);
        }

        private static IEnumerable<BinaryExpression> FormExpressionsForSupportedTypes(
            IEnumerable<(string FullName, PropertyInfo Property)> targetProps,
            ParameterExpression parameter,
            string searchValue)
        {
            var result = new List<BinaryExpression>();
            var valueExpr = Expression.Constant(searchValue.ToLower(), typeof(string));
            var expression = Expression.Constant(true);

            foreach (var (fullName, prop) in targetProps)
            {
                if (!_supportedTypes.TryGetValue(prop.PropertyType, out var tryParseMethod))
                    continue;

                if (tryParseMethod != null
                    && (bool)tryParseMethod.Invoke(null, new object?[] { searchValue, null })! == false)
                    continue;

                var expMember = parameter.GetPropertyExpression(fullName);
                var ext = Expression.Call(expMember, _methodToString);
                var extContains = Expression.Call(ext, _methodContains, valueExpr);
                var extAnd = Expression.Equal(extContains, expression);
                result.Add(extAnd);
            }
            return result;
        }
    }
}
using Monq.Core.Paging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Monq.Core.Paging.Extensions;

public static class QueryableExtensions
{
    static readonly MethodInfo _methodContains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
    static readonly MethodInfo _methodToString = typeof(object).GetMethod(nameof(ToString))!;
    static readonly MethodInfo _methodToLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
    static readonly MethodInfo _methodIsNullOrEmpty = typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) })!;

    static readonly HashSet<Type> _supportSearchByStringPropertyTypes = new()
    {
        typeof(string),
        typeof(Guid),
    };
    static readonly HashSet<Type> _supportSearchByIntNumberPropertyTypes = new()
    {
        typeof(int),
        typeof(long),
        typeof(Guid),
    };

    /// <summary>
    /// Orderings the by.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="data">The data.</param>
    /// <param name="propertyName">Name of the field.</param>
    /// <param name="dir">The dir.</param>
    public static IQueryable<TSource> OrderByProperty<TSource>(this IQueryable<TSource> data, string propertyName, string? dir)
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

        if (string.IsNullOrEmpty(dir) || dir == "asc")
            return data.OrderBy(lambda);
        return data.OrderByDescending(lambda);
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
        var searchByIntProperties = long.TryParse(search, out _)
            ? props.Where(p => _supportSearchByIntNumberPropertyTypes.Contains(p.Property.PropertyType)).ToList()
            : new List<(string, PropertyInfo)>();
        var searchByStringProperties = props.Where(p => _supportSearchByStringPropertyTypes
            .Contains(p.Property.PropertyType))
            .Except(searchByIntProperties)
            .ToList();

        if (!props.Any())
            return null;

        var parameter = Expression.Parameter(typeof(TSource), "t");
        var value = Expression.Constant(search.ToLower(), typeof(string));
        var expList = new List<BinaryExpression>();

        foreach (var (fullName, prop) in searchByStringProperties)
        {
            var expAnd = prop.PropertyType == typeof(string)
                ? FormExpressionForStringProperty(parameter, value, fullName)
                : FormExpressionForValueProperty(parameter, value, fullName);
            expList.Add(expAnd);
        }
        foreach (var (fullName, _) in searchByIntProperties)
        {
            var expAnd = FormExpressionForValueProperty(parameter, value, fullName);
            expList.Add(expAnd);
        }

        if (expList.Count == 0)
            return null;

        var body = expList.Aggregate(Expression.OrElse);
        return Expression.Lambda<Func<TSource, bool>>(body, parameter);
    }

    static BinaryExpression FormExpressionForStringProperty(ParameterExpression parameter, ConstantExpression value, string fullName)
    {
        var expMember = parameter.GetPropertyExpressionUnSafe(fullName);
        var expIsNullOrEmpty = Expression.Not(Expression.Call(_methodIsNullOrEmpty, expMember.AddNullConditions()));
        var expLower = Expression.Call(expMember, _methodToLower);
        var expContains = Expression.Call(expLower, _methodContains, value);
        var expAnd = Expression.AndAlso(expIsNullOrEmpty, expContains);
        return expAnd;
    }

    static BinaryExpression FormExpressionForValueProperty(
        ParameterExpression parameter,
        ConstantExpression searchValue,
        string fullName)
    {
        var expMember = parameter.GetPropertyExpression(fullName);
        var expMemberToString = Expression.Call(expMember, _methodToString);
        var expContains = Expression.Call(expMemberToString, _methodContains, searchValue);
        var expAnd = Expression.Equal(expContains, Expression.Constant(true));
        return expAnd;
    }
}
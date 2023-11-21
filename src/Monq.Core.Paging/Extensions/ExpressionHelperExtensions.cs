using DelegateDecompiler;
using Monq.Core.Paging.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Monq.Core.Paging.Extensions
{
    static class ExpressionHelperExtensions
    {
        static readonly MethodInfo _orderByAscMethod =
            typeof(Queryable).GetMethods()
                .First(method => method.Name == nameof(Queryable.OrderBy)
                    && method.GetParameters().Length == 2);

        static readonly MethodInfo _orderByDescMethod =
            typeof(Queryable).GetMethods()
                .First(method => method.Name == nameof(Queryable.OrderByDescending)
                    && method.GetParameters().Length == 2);

        static readonly MethodInfo _thenByMethod =
            typeof(Queryable).GetMethods()
                .First(method => method.Name == nameof(Queryable.ThenBy)
                    && method.GetParameters().Length == 2);

        static readonly MethodInfo _thenByDescMethod =
            typeof(Queryable).GetMethods()
                .First(method => method.Name == nameof(Queryable.ThenByDescending)
                    && method.GetParameters().Length == 2);

        /// <summary>
        /// Отсортировать по возрастанию.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="lambda">The lambda.</param>
        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, LambdaExpression lambda)
            => _orderByAscMethod.Call<IQueryable<TSource>>(new[] { typeof(TSource), lambda.ReturnType }, source, lambda);

        /// <summary>
        /// Отсортировать по убыванию.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="lambda">The lambda.</param>
        public static IQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, LambdaExpression lambda)
            => _orderByDescMethod.Call<IQueryable<TSource>>(new[] { typeof(TSource), lambda.ReturnType }, source, lambda);

        /// <summary>
        /// Performs a subsequent ordering in a sequence in ascending order.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="lambda">The lambda.</param>
        /// <returns></returns>
        public static IQueryable<TSource> ThenBy<TSource>(this IQueryable<TSource> source, LambdaExpression lambda)
            => _thenByMethod.Call<IQueryable<TSource>>(new[] { typeof(TSource), lambda.ReturnType }, source, lambda);

        /// <summary>
        /// Performs a subsequent ordering in a sequence in descending order.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="lambda">The lambda.</param>
        /// <returns></returns>
        public static IQueryable<TSource> ThenByDescending<TSource>(this IQueryable<TSource> source, LambdaExpression lambda)
            => _thenByDescMethod.Call<IQueryable<TSource>>(new[] { typeof(TSource), lambda.ReturnType }, source, lambda);

        /// <summary>
        /// Вызвать метод с указанными параметрами.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="typeArgs">The type arguments.</param>
        /// <param name="args">The arguments.</param>
        public static TResult Call<TResult>(this MethodInfo method, Type[] typeArgs, params object[] args)
        {
            if (method.IsStatic)
                return (TResult)method.MakeGenericMethod(typeArgs).Invoke(null, args);

            return (TResult)method.MakeGenericMethod(typeArgs).Invoke(args.FirstOrDefault(), args.Skip(1).ToArray());
        }

        /// <summary>
        /// Получить свойство по имени.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [Игнорировать регистр в имени свойства].</param>
        public static PropertyInfo? GetProperty(this Type type, string name, bool ignoreCase)
            => type.GetProperties().FirstOrDefault(x => string.Equals(x.Name, name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

        /// <summary>
        /// Получить тип свойства.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="path">The path.</param>
        public static Type? GetPropertyType(this Type? type, string path)
            => path.Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(type,
                    (propType, name) => propType is not null && propType.IsGenericType
                        ? propType.GetGenericArguments()[0].GetProperty(name)?.PropertyType
                        : propType?.GetProperty(name)?.PropertyType);

        /// <summary>
        /// Получить выражение получения свойства.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="path">The path.</param>
        public static Expression? GetPropertyExpression(this Expression expression, string path)
            => expression.Type.GetProperties(path)
                .Aggregate(expression, Expression.Property)
                .AddNullConditions();

        /// <summary>
        /// Получить выражение получения свойства (без проверок на null).
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="path">The path.</param>
        public static Expression GetPropertyExpressionUnSafe(this Expression expression, string path)
            => expression.Type.GetProperties(path)
                .Aggregate(expression, Expression.Property);

        /// <summary>
        /// Получить значение типа по умолчанию.
        /// </summary>
        /// <param name="type">Выражение.</param>
        public static object? GetDefault(this Type type)
            => type.IsValueType ? Activator.CreateInstance(type) : null;

        /// <summary>
        /// Получить выражение константу со значением типа по умолчанию.
        /// </summary>
        /// <param name="expr">Выражение.</param>
        public static Expression GetDefaultConstantExpr(this Expression? expr)
            => Expression.Constant(expr.Type.GetDefault(), expr.Type);

        /// <summary>
        /// Создать выражения на проверку на null.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <param name="val">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        public static Expression AddNullCondition(this Expression expr, Expression val, Expression defaultValue)
            => Expression.Condition(Expression.Equal(val, Expression.Constant(null, val.Type)), defaultValue, expr);

        /// <summary>
        /// Добавить в дерево выражения проверки на null (?.).
        /// </summary>
        /// <param name="expr">Выражение.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        public static Expression? AddNullConditions(this Expression? expr, Expression defaultValue)
        {
            var safe = expr;

            while (!IsNullSafe(expr, out var obj))
            {
                safe = safe.AddNullCondition(obj, defaultValue);
                expr = obj;
            }
            return safe;
        }

        /// <summary>
        /// Добавить в дерево выражения проверки на null (?.).
        /// </summary>
        /// <param name="expr">The expr.</param>
        public static Expression? AddNullConditions(this Expression? expr) =>
            expr.AddNullConditions(expr.GetDefaultConstantExpr());

        /// <summary>
        /// Получить для указанного типа цепочку PropertyInfo по заданному полному пути.
        /// </summary>
        /// <param name="type">Тип объекта.</param>
        /// <param name="path">Полный путь свойства.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        public static IEnumerable<PropertyInfo> GetProperties(this Type type, string? path, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(path))
                yield break;
            foreach (var propName in path.Trim().Split('.', StringSplitOptions.RemoveEmptyEntries))
            {
                var prop = type.GetProperty(propName, ignoreCase);
                if (prop is null)
                    yield break;
                yield return prop;
                type = prop.PropertyType;
            }
        }

        /// <summary>
        /// Проверить правильность полного пути свойства, с возвращение его с учетом правильного регистра.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="path">Полный путь.</param>
        public static string GetValidPropertyName(this Type type, string? path)
            => string.Join(".", type.GetProperties(path, true).Select(x => x.Name));

        static bool IsNullSafe(Expression? expr, out Expression? nullableObject)
        {
            nullableObject = null;

            if (expr is not MemberExpression && expr is not MethodCallExpression)
                return true;

            Expression? obj;
            var callExpr = expr as MethodCallExpression;

            if (expr is MemberExpression memberExpr)
            {
                // Static fields don't require an instance
                var field = memberExpr.Member as FieldInfo;
                if (field != null && field.IsStatic)
                    return true;

                // Static properties don't require an instance
                var property = memberExpr.Member as PropertyInfo;
                if (property != null)
                {
                    var getter = property.GetGetMethod();
                    if (getter != null && getter.IsStatic)
                        return true;
                }
                obj = memberExpr.Expression;
            }
            else
            {
                // Static methods don't require an instance
                if (callExpr.Method.IsStatic)
                    return true;

                obj = callExpr.Object;
            }

            // Value types can't be null
            // Проверки на Null требуется проводить только по вложенным свойствам.
            if (obj.Type.IsValueType || obj.NodeType == ExpressionType.Parameter)
                return true;

            // Instance member access or instance method call is not safe
            nullableObject = obj;
            return false;
        }

        /// <summary>
        /// Декомпилировать свойства помеченные атрибутом Computed.
        /// </summary>
        /// <param name="expr">The expr.</param>
        public static Expression Decompile(this Expression expr)
            => DecompileExpressionVisitor.Decompile(expr);

        /// <summary>
        /// Превратить вызовы в константы (на данный момент поддерживается только вызовы над DateTimeOffsets).
        /// </summary>
        /// <param name="expr">The expr.</param>
        public static Expression ExpressionCallsToConstants(this Expression expr)
            => ExpressionConstantCallVisitor.ExpressionCallsToConstants(expr);

        /// <summary>
        /// Получить полное имя свойства.
        /// </summary>
        /// <typeparam name="T">Тип объекта, которому принадлежит свойство</typeparam>
        /// <typeparam name="TVal">Тип свойства.</typeparam>
        /// <param name="expression">The expression.</param>
        public static string GetFullPropertyName<T, TVal>(this Expression<Func<T, TVal>> expression)
        {
            var props = new List<string>();
            var member = GetMemberExpression(expression);
            while (member != null)
            {
                props.Add(member.Member.Name);
                member = GetMemberExpression(member.Expression);
            }
            props.Reverse();
            return string.Join('.', props);
        }

        /// <summary>
        /// Получить публичные свойства типа, включая вложенные свойства на указанной глубине
        /// и полное имя свойства удовлетворяющее заданному выражению.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="depth">Глубина.</param>
        /// <param name="exp">The exp.</param>
        public static IEnumerable<(string FullName, PropertyInfo Property)> GetPublicProperties(this Type type, int depth = 1, Func<string, bool>? exp = null)
        {
            exp ??= (_) => true;

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => exp(x.Name))
                .Select(x => (x.Name, Property: x))
                .ToList();
            if (depth <= 1) return properties;

            var nextLevel = properties
                .Where(x => x.Property.PropertyType.IsClass && x.Property.PropertyType != typeof(string))
                .SelectMany(x => GetPublicProperties(x.Property.PropertyType, depth - 1)
                .Select(y => (FullName: $"{x.Property.Name}.{y.FullName}", y.Property)))
                .Where(x => exp(x.FullName));

            return properties.Concat(nextLevel);
        }

        /// <summary>
        /// Получить MemberExpression выражения.
        /// </summary>
        /// <param name="expression">Выражение.</param>
        public static MemberExpression? GetMemberExpression(this Expression? expression) =>
            expression switch
            {
                MemberExpression memberExpression => memberExpression,
                LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression body => body,
                LambdaExpression lambdaExpression when lambdaExpression.Body is UnaryExpression unaryExpression =>
                    (MemberExpression)unaryExpression.Operand,
                _ => null
            };
    }
}
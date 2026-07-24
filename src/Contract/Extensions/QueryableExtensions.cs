using System.Linq.Expressions;

namespace Contract;

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> ApplyOrder<T>(this IQueryable<T> query, QueryInfo info)
    {
        var propName = info.OrderBy ?? AppConstants.DefaultOrderBy;
        var prop = typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                   ?? typeof(T).GetProperty("CreatedAt", BindingFlags.Public | BindingFlags.Instance);

        if (prop is null)
            throw new ArgumentException($"Property '{propName}' not found on {typeof(T).Name}.");

        var param = Expression.Parameter(typeof(T), "x");
        var keySelector = Expression.Lambda(Expression.Property(param, prop), param);
        var methodName = info.OrderType == OrderType.Ascending ? "OrderBy" : "OrderByDescending";

        var result = typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), prop.PropertyType)
            .Invoke(null, [query, keySelector]);

        return (IOrderedQueryable<T>)result!;
    }
}

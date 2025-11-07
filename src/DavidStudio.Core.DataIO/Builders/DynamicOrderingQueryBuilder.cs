using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Builders;

/// <summary>
/// Provides functionality to dynamically construct ordering expressions
/// based on a textual representation of sorting parameters.
/// </summary>
/// <remarks>
/// <para>
/// This approach allows dynamic sorting without relying on reflection at runtime,
/// while maintaining compatibility with Entity Framework Core's expression translation.
/// </para>
/// </remarks>
public static class DynamicOrderingQueryBuilder
{
    /// <summary>
    /// Builds a collection of ordering expressions and direction flags
    /// based on a string-based <paramref name="orderBy"/> definition.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <param name="orderBy">
    /// A comma-separated list of property names, optionally followed by
    /// <c>"desc"</c> to indicate descending order.  
    /// For example: <c>"Name desc, CreatedAtUtc, Address.City desc"</c>.
    /// </param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <see cref="IReadOnlyList{T}"/> of <see cref="Expression{TDelegate}"/> representing
    /// the ordering properties.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// An array of <see cref="bool"/> values indicating whether each corresponding
    /// ordering expression is descending.
    /// </description>
    /// </item>
    /// </list>
    /// </returns>
    public static (IReadOnlyList<Expression<Func<TEntity, object>>> orderBy, bool[] isDescending) Build<TEntity>(
        string orderBy)
    {
        List<Expression<Func<TEntity, object>>> orderByExpressions = [];
        List<bool> isDescending = [];

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var orderParams = orderBy.Trim().Split(',').Select(p => p.Trim());

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var orderByProperty = param.Split(' ')[0];

            var propertyExpression = orderByProperty
                .Split('.')
                .Aggregate<string?, Expression>(parameter, Expression.PropertyOrField!);

            var propertyExpressionObject = Expression.Convert(propertyExpression, typeof(object));
            var lambda = Expression.Lambda<Func<TEntity, object>>(propertyExpressionObject, parameter);

            orderByExpressions.Add(lambda);
            isDescending.Add(param.EndsWith(" desc"));
        }

        return (orderByExpressions, isDescending.ToArray());
    }
}
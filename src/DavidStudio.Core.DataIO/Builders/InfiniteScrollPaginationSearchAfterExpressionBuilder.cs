using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Expressions;

namespace DavidStudio.Core.DataIO.Builders;

/// <summary>
/// Builds a dynamic "search-after" predicate for infinite scroll pagination
/// based on multi-field ordering and the last fetched values.
/// </summary>
public static class InfiniteScrollPaginationSearchAfterExpressionBuilder
{
    /// <summary>
    /// Builds the search-after predicate expression for a given entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <param name="orderBy">The collection of ordering expressions.</param>
    /// <param name="isDescending">
    /// A boolean array indicating whether each ordering expression is descending.
    /// Must match the length of <paramref name="orderBy"/>.
    /// </param>
    /// <param name="searchAfterValues">
    /// An array of values representing the last entity fetched in the previous page.
    /// Used to compute the "after" filter for the next page.
    /// </param>
    /// <returns>
    /// An <see cref="Expression{Func{TEntity, bool}}"/> that can be used to filter entities
    /// after the last fetched record.
    /// </returns>
    public static Expression<Func<TEntity, bool>> Build<TEntity>(
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        bool[] isDescending,
        object?[] searchAfterValues) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? filter = null;

        for (var i = 0; i < orderBy.Count; i++)
        {
            var orderSelector = orderBy[i];
            var orderMember = UnWrap(orderSelector.Body);
            orderMember = new ReplaceParameterVisitor(orderSelector.Parameters[0], parameter).Visit(orderMember);
            var orderValue = Expression.Constant(searchAfterValues[i]);

            var comparison = BuildComparison(orderMember, orderValue, !isDescending[i]);

            if (filter is null)
                filter = comparison;
            else
            {
                var prevEquals = BuildPreviousEquals(orderBy, i, searchAfterValues, parameter);
                filter = Expression.OrElse(filter, Expression.AndAlso(prevEquals, comparison));
            }
        }

        return Expression.Lambda<Func<TEntity, bool>>(filter!, parameter);
    }

    /// <summary>
    /// Removes any conversion wrappers from the expression body, e.g. <c>Convert(x)</c> to get the underlying member.
    /// </summary>
    /// <param name="body">The expression body to unwrap.</param>
    /// <returns>The unwrapped expression.</returns>
    private static Expression UnWrap(Expression body)
        => body is UnaryExpression { NodeType: ExpressionType.Convert } unary
            ? unary.Operand
            : body;

    /// <summary>
    /// Builds a comparison expression between an entity member and a constant value.
    /// </summary>
    /// <param name="orderMember">The expression representing the entity property.</param>
    /// <param name="orderValue">The constant value to compare against.</param>
    /// <param name="asc">Whether the comparison is ascending (<see langword="true"/> for ascending, <see langword="false"/> for descending).</param>
    /// <returns>A <see cref="BinaryExpression"/> representing the comparison.</returns>
    private static BinaryExpression BuildComparison(Expression orderMember, Expression orderValue, bool asc)
    {
        // Support for StronglyTypedId
        if (orderMember.Type.Name.EndsWith("Id") && orderMember.Type != typeof(Guid))
        {
            var stronglyTypedId = Activator.CreateInstance(orderMember.Type, ((ConstantExpression)orderValue).Value);
            var stronglyTypedIdExpression = Expression.Constant(stronglyTypedId);

            return asc
                ? Expression.GreaterThan(orderMember, stronglyTypedIdExpression)
                : Expression.LessThan(orderMember, stronglyTypedIdExpression);
        }

        if (orderMember.Type == typeof(string))
        {
            var compareMethod = typeof(string).GetMethod(nameof(string.Compare), [typeof(string), typeof(string)])!;
            var compareCall = Expression.Call(compareMethod, orderMember, orderValue);

            return asc
                ? Expression.GreaterThan(compareCall, Expression.Constant(0))
                : Expression.LessThan(compareCall, Expression.Constant(0));
        }

        if (orderMember.Type == typeof(bool))
        {
            Expression entityProp = Expression.Equal(orderMember, Expression.Constant(asc));
            Expression cursorProp = Expression.Equal(orderValue, Expression.Constant(!asc));

            return Expression.And(entityProp, cursorProp);
        }

        return asc
            ? Expression.GreaterThan(orderMember, orderValue)
            : Expression.LessThan(orderMember, orderValue);
    }

    /// <summary>
    /// Builds a combined equality expression for all previous ordering fields,
    /// ensuring that the current comparison applies only when previous fields match.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <param name="orderBy">The collection of ordering expressions.</param>
    /// <param name="upToIndex">The index of the current ordering expression.</param>
    /// <param name="searchAfterValues">The values to compare against for previous fields.</param>
    /// <param name="parameter">The parameter expression for the entity.</param>
    /// <returns>An <see cref="Expression"/> representing the combined equality of previous fields.</returns>
    private static Expression BuildPreviousEquals<TEntity>(IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        int upToIndex,
        object?[] searchAfterValues,
        ParameterExpression parameter)
    {
        Expression? prevEquals = null;

        for (var j = 0; j < upToIndex; j++)
        {
            var prevSelector = orderBy[j];
            var prevMember = Expression.Invoke(prevSelector, parameter);
            var prevValue = Expression.Convert(
                Expression.Constant(searchAfterValues[j]),
                prevMember.Type
            );

            var eq = Expression.Equal(prevMember, prevValue);
            prevEquals = prevEquals == null ? eq : Expression.AndAlso(prevEquals, eq);
        }

        return prevEquals!;
    }
}
using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Expressions;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Builders;

/// <summary>
/// Builds dynamic cursor objects for infinite scroll pagination based on
/// the last element of a sorted query.
/// </summary>
public static class InfiniteScrollPaginationDynamicCursorBuilder
{
    /// <summary>
    /// Builds the <see cref="DynamicCursor"/> for the next page of an infinite scroll query.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <param name="options">Pagination options that specify page size and cursor parameters.</param>
    /// <param name="orderBy">
    /// A collection of expressions defining the order of the query.
    /// Each expression should correspond to a sortable entity property.
    /// </param>
    /// <param name="ordered">
    /// The <see cref="IOrderedQueryable{T}"/> representing the already sorted query.
    /// The cursor will be built from the last item in the current page.
    /// </param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>
    /// A <see cref="DynamicCursor"/> representing the key values of the next page.
    /// </returns>
    public static async Task<DynamicCursor> BuildNextCursorAsync<TEntity>(
        InfinitePageOptions options,
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        IOrderedQueryable<TEntity> ordered,
        CancellationToken cancellationToken = default)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var nextCursorExpr = Expression.NewArrayInit(
            typeof(object),
            orderBy.Select(o =>
            {
                var body = o.Body is UnaryExpression { NodeType: ExpressionType.Convert } unary
                    ? unary.Operand
                    : o.Body;

                body = new ReplaceParameterVisitor(o.Parameters[0], parameter).Visit(body);

                return Expression.Convert(body, typeof(object));
            })
        );
        var cursorSelector = Expression.Lambda<Func<TEntity, object[]>>(nextCursorExpr, parameter);

        var nextValues = await ordered
            .Skip(options.Size - 1)
            .Select(cursorSelector)
            .FirstAsync(cancellationToken);

        return new DynamicCursor(nextValues);
    }
}
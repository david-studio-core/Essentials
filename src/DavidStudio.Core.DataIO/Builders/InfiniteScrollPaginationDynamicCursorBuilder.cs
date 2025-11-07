using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Expressions;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Builders;

public static class InfiniteScrollPaginationDynamicCursorBuilder
{
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
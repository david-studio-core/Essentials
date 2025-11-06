using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Expressions;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Builders;

public static class InfiniteScrollPaginationDynamicCursorBuilder
{
    public static async Task<DynamicCursor> BuildNextCursorAsync<TEntity, TResult>(
        InfinitePageOptions options,
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        Expression<Func<TEntity, TResult>> selector,
        IOrderedQueryable<TEntity> ordered,
        List<TResult> temporaryResults,
        CancellationToken cancellationToken = default)
    {
        var nextValues = new object?[orderBy.Count];

        if (ExpressionPropertyHelper.AllOrderByFieldsExistInSelector(orderBy, selector))
        {
            var last = temporaryResults.LastOrDefault();

            for (var i = 0; i < orderBy.Count; i++)
            {
                var propertyName = ExpressionPropertyHelper.GetPropertyName(orderBy[i]);

                nextValues[i] = typeof(TResult).GetProperty(propertyName)!.GetValue(last, null);
            }
        }
        else
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

            nextValues = await ordered.Skip(options.Size - 1).Select(cursorSelector).FirstAsync(cancellationToken);
        }

        return new DynamicCursor(nextValues);
    }
}
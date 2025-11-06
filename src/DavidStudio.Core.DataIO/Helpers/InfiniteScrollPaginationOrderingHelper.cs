using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Helpers;

public static class InfiniteScrollPaginationOrderingHelper
{
    public static IOrderedQueryable<TEntity> ApplyOrdering<TEntity>(IQueryable<TEntity> query,
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        bool[] isDescending)
    {
        IOrderedQueryable<TEntity> ordered = null!;

        for (var i = 0; i < orderBy.Count; i++)
        {
            if (i == 0)
            {
                ordered = !isDescending[i]
                    ? query.OrderBy(orderBy[i])
                    : query.OrderByDescending(orderBy[i]);
            }
            else
            {
                ordered = !isDescending[i]
                    ? ordered.ThenBy(orderBy[i])
                    : ordered.ThenByDescending(orderBy[i]);
            }
        }

        return ordered;
    }
}
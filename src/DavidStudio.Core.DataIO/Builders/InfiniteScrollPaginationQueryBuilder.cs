using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Builders;

public class InfiniteScrollPaginationQueryBuilder<TEntity, TResult>(IQueryable<TEntity> query)
    : BasicQueryBuilder<TEntity>(query)
    where TEntity : class
    where TResult : class
{
    private IReadOnlyList<Expression<Func<TEntity, object>>>? _orderBy;
    private bool[]? _isDescending;
    private InfinitePageOptions? _pageOptions;
    private Expression<Func<TEntity, TResult>>? _selector;

    public InfiniteScrollPaginationQueryBuilder<TEntity, TResult> WithOrdering(
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        bool[] isDescending)
    {
        _orderBy = orderBy;
        _isDescending = isDescending;

        return this;
    }

    public InfiniteScrollPaginationQueryBuilder<TEntity, TResult> WithSearchAfter(InfinitePageOptions options)
    {
        _pageOptions = options;

        if (options.SearchAfter is null && options.SearchAfterToken is null) return this;
        options.SearchAfter ??= DynamicCursorTokenizer.Decode(options.SearchAfterToken)!;

        var filter =
            InfiniteScrollPaginationSearchAfterExpressionBuilder.Build(_orderBy!, _isDescending!,
                options.SearchAfter.Values);

        Query = Query.Where(filter);

        return this;
    }

    public InfiniteScrollPaginationQueryBuilder<TEntity, TResult> WithProjection(Expression<Func<TEntity, TResult>> selector)
    {
        _selector = selector;

        return this;
    }

    public async Task<InfinitePageData<TResult>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var ordered = InfiniteScrollPaginationOrderingHelper.ApplyOrdering(Query, _orderBy!, _isDescending!);

        var temporaryResults = await ordered
            .Select(_selector!)
            .Take(_pageOptions!.Size + 1)
            .ToListAsync(cancellationToken);

        var hasMore = temporaryResults.Count > _pageOptions.Size;

        DynamicCursor? nextCursor = null;
        if (hasMore)
        {
            nextCursor =
                await InfiniteScrollPaginationDynamicCursorBuilder.BuildNextCursorAsync(_pageOptions, _orderBy!,
                    _selector!, ordered, temporaryResults, cancellationToken);
        }

        return new InfinitePageData<TResult>(
            temporaryResults.Take(_pageOptions.Size).ToList(),
            nextCursor,
            hasMore
        );
    }
}
using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Builders;

/// <summary>
/// Builds an infinite scroll pagination query.
/// </summary>
/// <typeparam name="TEntity">The entity type being queried.</typeparam>
/// <typeparam name="TResult">The type of the projected result.</typeparam>
/// <remarks>
/// <para>
/// This query builder extends <see cref="BasicQueryBuilder{TEntity}"/> and adds
/// support for infinite scrolling using "search-after" cursors.
/// </para>
/// <para>
/// It allows specifying ordering, search-after pagination options, and projection expressions.
/// The <see cref="ExecuteAsync"/> method executes the query and returns a page of results.
/// </para>
/// </remarks>
public class InfiniteScrollPaginationQueryBuilder<TEntity, TResult>(IQueryable<TEntity> query)
    : BasicQueryBuilder<TEntity>(query)
    where TEntity : class
    where TResult : class
{
    private IReadOnlyList<Expression<Func<TEntity, object>>>? _orderBy;
    private bool[]? _isDescending;
    private InfinitePageOptions? _pageOptions;
    private Expression<Func<TEntity, TResult>>? _selector;

    /// <summary>
    /// Specifies the ordering expressions and direction for the query.
    /// </summary>
    /// <param name="orderBy">The list of property expressions to order by.</param>
    /// <param name="isDescending">
    /// Array indicating whether each corresponding ordering expression is descending.
    /// </param>
    /// <returns>The current <see cref="InfiniteScrollPaginationQueryBuilder{TEntity, TResult}"/> instance.</returns>
    public InfiniteScrollPaginationQueryBuilder<TEntity, TResult> WithOrdering(
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        bool[] isDescending)
    {
        _orderBy = orderBy;
        _isDescending = isDescending;

        return this;
    }

    /// <summary>
    /// Specifies the ordering by parsing a comma-separated string.
    /// </summary>
    /// <param name="orderBy">
    /// A comma-separated list of property names with optional "desc" for descending order.
    /// For example: <c>"Name asc, Address.City asc, Id desc"</c>.
    /// </param>
    /// <returns>The current <see cref="InfiniteScrollPaginationQueryBuilder{TEntity, TResult}"/> instance.</returns>
    public new InfiniteScrollPaginationQueryBuilder<TEntity, TResult> WithOrdering(string orderBy)
    {
        var (orderByExpressions, isDescending) = DynamicOrderingQueryBuilder.Build<TEntity>(orderBy);

        _orderBy = orderByExpressions;
        _isDescending = isDescending;

        return this;
    }

    /// <summary>
    /// Applies search-after pagination based on the provided <see cref="InfinitePageOptions"/>.
    /// </summary>
    /// <param name="options">Pagination options, including page size and search-after cursor values or token.</param>
    /// <returns>The current <see cref="InfiniteScrollPaginationQueryBuilder{TEntity, TResult}"/> instance.</returns>
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

    /// <summary>
    /// Specifies the projection to apply to the query results.
    /// </summary>
    /// <param name="selector">The projection expression.</param>
    /// <returns>The current <see cref="InfiniteScrollPaginationQueryBuilder{TEntity, TResult}"/> instance.</returns>
    public InfiniteScrollPaginationQueryBuilder<TEntity, TResult> WithProjection(Expression<Func<TEntity, TResult>> selector)
    {
        _selector = selector;

        return this;
    }

    /// <summary>
    /// Executes the query and returns a page of results as <see cref="InfinitePageData{TResult}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the query.</param>
    /// <returns>
    /// A <see cref="Task{InfinitePageData}"/> representing the asynchronous query result,
    /// including the page of results, next cursor, and a flag indicating whether more results exist.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if ordering expressions and directions or page options and projection are not set before execution.
    /// </exception>
    public async Task<InfinitePageData<TResult>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_orderBy is null || _isDescending is null ||
            _pageOptions is null || _selector is null)
        {
            throw new InvalidOperationException("Invalid query.");
        }

        var ordered = DynamicOrderingHelper.Apply(Query, _orderBy!, _isDescending!);

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
                    ordered, cancellationToken);
        }

        return new InfinitePageData<TResult>(
            temporaryResults.Take(_pageOptions.Size).ToList(),
            nextCursor,
            hasMore
        );
    }
}
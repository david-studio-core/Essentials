using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DavidStudio.Core.DataIO.Builders;

/// <summary>
/// Provides a fluent interface for building and composing Entity Framework Core queries
/// in a structured and reusable way.
/// </summary>
/// <typeparam name="TEntity">The entity type being queried.</typeparam>
/// <remarks>
/// By chaining methods, you can construct complex queries dynamically while preserving
/// readability and avoiding repetitive LINQ expressions.
/// </remarks>
public class BasicQueryBuilder<TEntity>(IQueryable<TEntity> query)
    where TEntity : class
{
    /// <summary>
    /// Gets or sets the underlying <see cref="IQueryable{TEntity}"/> being composed.
    /// </summary>
    public IQueryable<TEntity> Query { get; set; } = query;

    /// <summary>
    /// Enables or disables entity tracking in the query.
    /// </summary>
    /// <param name="trackingEnabled">
    /// <see langword="true"/> to enable tracking; <see langword="false"/> to disable it (using <see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TEntity}(IQueryable{TEntity})"/>).
    /// </param>
    /// <returns>The current <see cref="BasicQueryBuilder{TEntity}"/> instance for chaining.</returns>
    public BasicQueryBuilder<TEntity> WithTracking(bool trackingEnabled)
    {
        if (!trackingEnabled)
            Query = Query.AsNoTracking();

        return this;
    }

    /// <summary>
    /// Configures whether to ignore global query filters (such as soft-delete filters).
    /// </summary>
    /// <param name="ignore">
    /// <see langword="true"/> to ignore filters; <see langword="false"/> to keep them applied.
    /// </param>
    /// <returns>The current <see cref="BasicQueryBuilder{TEntity}"/> instance for chaining.</returns>
    public BasicQueryBuilder<TEntity> WithIgnoreQueryFilters(bool ignore)
    {
        if (ignore)
            Query = Query.IgnoreQueryFilters();

        return this;
    }

    /// <summary>
    /// Includes related navigation properties in the query using the specified include function.
    /// </summary>
    /// <param name="include">A function that specifies related entities to include in the query.</param>
    /// <returns>The current <see cref="BasicQueryBuilder{TEntity}"/> instance for chaining.</returns>
    public BasicQueryBuilder<TEntity> WithInclude(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include)
    {
        if (include != null)
            Query = include(Query);

        return this;
    }

    /// <summary>
    /// Applies a filter predicate to the query.
    /// </summary>
    /// <param name="predicate">A LINQ expression used to filter results.</param>
    /// <returns>The current <see cref="BasicQueryBuilder{TEntity}"/> instance for chaining.</returns>
    public BasicQueryBuilder<TEntity> WithPredicate(Expression<Func<TEntity, bool>>? predicate)
    {
        if (predicate != null)
            Query = Query.Where(predicate);

        return this;
    }

    /// <summary>
    /// Applies an ordering expression to the query.
    /// </summary>
    /// <param name="orderBy">A function that defines the ordering logic.</param>
    /// <returns>The current <see cref="BasicQueryBuilder{TEntity}"/> instance for chaining.</returns>
    public BasicQueryBuilder<TEntity> WithOrdering(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy)
    {
        if (orderBy != null)
            Query = orderBy(Query);

        return this;
    }

    /// <summary>
    /// Applies dynamic ordering to the query based on a string representation
    /// of ordering fields (e.g., <c>"Name desc, CreatedAtUtc"</c>).
    /// </summary>
    /// <param name="orderBy">
    /// A comma-separated string of field names with optional sort directions (<c>"asc"</c> or <c>"desc"</c>).
    /// </param>
    /// <returns>The current <see cref="BasicQueryBuilder{TEntity}"/> instance for chaining.</returns>
    /// <remarks>
    /// This method uses <see cref="DynamicOrderingQueryBuilder"/> and <see cref="DynamicOrderingHelper"/>
    /// to parse and apply ordering expressions dynamically at runtime.
    /// </remarks>
    public BasicQueryBuilder<TEntity> WithOrdering(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy)) return this;

        var (orderByExpressions, isDescending) = DynamicOrderingQueryBuilder.Build<TEntity>(orderBy);
        Query = DynamicOrderingHelper.Apply(Query, orderByExpressions, isDescending);

        return this;
    }

    /// <summary>
    /// Projects the current query to a different result type using the specified selector.
    /// </summary>
    /// <typeparam name="TResult">The projection type.</typeparam>
    /// <param name="selector">An expression defining the projection.</param>
    /// <returns>A new <see cref="BasicQueryBuilder{TEntity}"/> for the projected result type.</returns>
    public BasicQueryBuilder<TResult> WithProjection<TResult>(Expression<Func<TEntity, TResult>> selector)
        where TResult : class
    {
        return new BasicQueryBuilder<TResult>(Query.Select(selector));
    }

    /// <summary>
    /// Applies offset-based pagination to the query.
    /// </summary>
    /// <param name="options">The pagination options, including page number and page size.</param>
    /// <returns>The current <see cref="BasicQueryBuilder{TEntity}"/> instance for chaining.</returns>
    /// <remarks>
    /// This method skips and takes records based on the provided <see cref="PageOptions"/>.
    /// It should not be used when user can access very deep pages instead use <see cref="InfiniteScrollPaginationQueryBuilder{TEntity,TResult}"/>. 
    /// </remarks>
    public BasicQueryBuilder<TEntity> WithOffsetPagination(PageOptions options)
    {
        Query = Query
            .Skip((options.Page - 1) * options.Size)
            .Take(options.Size);

        return this;
    }
}
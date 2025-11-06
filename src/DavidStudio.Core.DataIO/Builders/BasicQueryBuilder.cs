using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using DavidStudio.Core.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DavidStudio.Core.DataIO.Builders;

public class BasicQueryBuilder<TEntity>(IQueryable<TEntity> query)
    where TEntity : class
{
    public IQueryable<TEntity> Query { get; protected set; } = query;

    public BasicQueryBuilder<TEntity> WithTracking(bool trackingEnabled)
    {
        if (!trackingEnabled)
            Query = Query.AsNoTracking();

        return this;
    }

    public BasicQueryBuilder<TEntity> WithIgnoreQueryFilters(bool ignore)
    {
        if (ignore)
            Query = Query.IgnoreQueryFilters();

        return this;
    }

    public BasicQueryBuilder<TEntity> WithInclude(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include)
    {
        if (include != null)
            Query = include(Query);

        return this;
    }

    public BasicQueryBuilder<TEntity> WithPredicate(Expression<Func<TEntity, bool>>? predicate)
    {
        if (predicate != null)
            Query = Query.Where(predicate);

        return this;
    }

    public BasicQueryBuilder<TEntity> WithOrdering(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy)
    {
        if (orderBy != null)
            Query = orderBy(Query);

        return this;
    }

    public BasicQueryBuilder<TEntity> WithOrdering(string? orderBy,
        IReadOnlyList<Expression<Func<TEntity, object>>>? allowedToOrderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy)) return this;

        var orderByQuery = DynamicOrderByQueryBuilder.BuildString(orderBy);

        if (!string.IsNullOrWhiteSpace(orderByQuery))
            Query = Query.OrderBy(orderByQuery);

        return this;
    }

    public BasicQueryBuilder<TResult> WithProjection<TResult>(Expression<Func<TEntity, TResult>> selector)
        where TResult : class
    {
        return new BasicQueryBuilder<TResult>(Query.Select(selector));
    }

    public BasicQueryBuilder<TEntity> WithOffsetPagination(PageOptions options)
    {
        Query = Query
            .Skip((options.Page - 1) * options.Size)
            .Take(options.Size);

        return this;
    }
}
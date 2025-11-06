using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Builders;
using DavidStudio.Core.DataIO.Entities;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace DavidStudio.Core.DataIO.Repositories;

/// <summary>
/// Defines common operations for entities in the repository.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The key type of entity</typeparam>
public abstract class BaseRepository<TEntity, TKey>(DbContext context)
    : IBaseRepository<TEntity, TKey>, IBaseAggregationRepository<TEntity>
    where TEntity : class, IEntity<TKey>
{
    public DbSet<TEntity> Entities { get; } = context.Set<TEntity>();

    public virtual Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        var builder = new BasicQueryBuilder<TEntity>(Entities)
            .WithTracking(!disableTracking)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithInclude(include)
            .WithPredicate(predicate)
            .WithOrdering(orderBy)
            .WithProjection(selector);

        return builder.Query.ToListAsync(cancellationToken);
    }

    public async Task<PageData<TResult>> GetAllAsync<TResult>(
        PageOptions options,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        var builder = new BasicQueryBuilder<TEntity>(Entities)
            .WithTracking(!disableTracking)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithInclude(include)
            .WithPredicate(predicate)
            .WithOrdering(orderBy)
            .WithProjection(selector);

        var count = await builder.Query.CountAsync(cancellationToken);
        var entities = await builder.WithOffsetPagination(options).Query.ToListAsync(cancellationToken);

        return new PageData<TResult>(entities, count, options);
    }

    public async Task<PageData<TResult>> GetAllAsync<TResult>(
        PageOptions options,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        string? orderBy = null,
        IReadOnlyList<Expression<Func<TEntity, object>>>? allowedToOrderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        if (orderBy != null && allowedToOrderBy != null)
            DynamicOrderByHelper.EnsureOrderByAllowed(orderBy, allowedToOrderBy);

        var builder = new BasicQueryBuilder<TEntity>(Entities)
            .WithTracking(!disableTracking)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithInclude(include)
            .WithPredicate(predicate)
            .WithOrdering(orderBy, allowedToOrderBy)
            .WithProjection(selector);

        var count = await builder.Query.CountAsync(cancellationToken);
        var entities = await builder.WithOffsetPagination(options).Query.ToListAsync(cancellationToken);

        return new PageData<TResult>(entities, count, options);
    }

    public Task<InfinitePageData<TResult>> GetAllAsync<TResult>(
        InfinitePageOptions options,
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        bool[] isDescending,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        if (orderBy.Count == 0)
            throw new ArgumentException("At least one ordering selector must be provided.", nameof(orderBy));

        var builder = new InfiniteScrollPaginationQueryBuilder<TEntity, TResult>(Entities)
            .WithTracking(!disableTracking)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithInclude(include)
            .WithPredicate(predicate) as InfiniteScrollPaginationQueryBuilder<TEntity, TResult>;

        builder = builder!
            .WithOrdering(orderBy, isDescending)
            .WithSearchAfter(options)
            .WithProjection(selector);

        return builder.ExecuteAsync(cancellationToken);
    }

    public Task<InfinitePageData<TResult>> GetAllAsync<TResult>(
        InfinitePageOptions options,
        string orderBy,
        IReadOnlyList<Expression<Func<TEntity, object>>>? allowedToOrderBy,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        if (allowedToOrderBy != null)
            DynamicOrderByHelper.EnsureOrderByAllowed(orderBy, allowedToOrderBy);

        var (orderByExpressions, isDescending) =
            DynamicOrderByQueryBuilder.BuildInfiniteScrollPaginationObjects<TEntity>(orderBy);

        var builder = new InfiniteScrollPaginationQueryBuilder<TEntity, TResult>(Entities)
            .WithTracking(!disableTracking)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithInclude(include)
            .WithPredicate(predicate) as InfiniteScrollPaginationQueryBuilder<TEntity, TResult>;

        builder = builder!
            .WithOrdering(orderByExpressions, isDescending)
            .WithSearchAfter(options)
            .WithProjection(selector);

        return builder.ExecuteAsync(cancellationToken);
    }

    public virtual Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        var builder = new BasicQueryBuilder<TEntity>(Entities)
            .WithTracking(!disableTracking)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithInclude(include)
            .WithPredicate(predicate)
            .WithOrdering(orderBy)
            .WithProjection(selector);

        return builder.Query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual ValueTask<TEntity?> GetByIdAsync(TKey[] id,
        CancellationToken cancellationToken = default)
    {
        return Entities.FindAsync([..id], cancellationToken: cancellationToken);
    }

    public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        var builder = new BasicQueryBuilder<TEntity>(Entities)
            .WithIgnoreQueryFilters(ignoreQueryFilters);

        return builder.Query.AnyAsync(predicate, cancellationToken);
    }

    public virtual ValueTask<EntityEntry<TEntity>> CreateAsync(TEntity model,
        CancellationToken cancellationToken = default)
    {
        return Entities.AddAsync(model, cancellationToken);
    }

    public virtual void Update(TEntity model)
    {
        Entities.Attach(model);
        context.Entry(model).State = EntityState.Modified;
    }

    public virtual void Delete(TEntity model)
    {
        if (context.Entry(model).State == EntityState.Detached)
            Entities.Attach(model);

        Entities.Remove(model);
    }

    public virtual async Task<bool> DeleteAsync(TKey id,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        var entityToDelete = await Entities
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(e => id!.Equals(e.Id), cancellationToken);

        if (entityToDelete is null) return false;

        Delete(entityToDelete);

        return true;
    }

    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        var builder = new BasicQueryBuilder<TEntity>(Entities)
            .WithTracking(false)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithPredicate(predicate);

        return builder.Query.CountAsync(cancellationToken);
    }

    public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        var builder = new BasicQueryBuilder<TEntity>(Entities)
            .WithTracking(false)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithPredicate(predicate);

        return builder.Query.LongCountAsync(cancellationToken);
    }

    public virtual Task<double> AverageAsync(
        Expression<Func<TEntity, int>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        var builder = new BasicQueryBuilder<TEntity>(Entities)
            .WithTracking(false)
            .WithIgnoreQueryFilters(ignoreQueryFilters)
            .WithPredicate(predicate);

        return builder.Query.AverageAsync(selector, cancellationToken);
    }
}
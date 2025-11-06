using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Entities;
using DavidStudio.Core.DataIO.Expressions;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.DataIO.Repositories;
using DavidStudio.Core.DataIO.UnitOfWork.EfCore;
using DavidStudio.Core.Pagination;
using DavidStudio.Core.Pagination.InfiniteScroll;
using DavidStudio.Core.Results;
using DavidStudio.Core.Results.Generic;

namespace DavidStudio.Core.DataIO.Services;

public abstract class BaseReadonlyService<TRepository, TEntity, TKey, TReadDto>(TRepository repository)
    : IBaseReadonlyService<TEntity, TKey, TReadDto>
    where TRepository : class, IBaseRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : new()
    where TReadDto : class
{
    protected readonly TRepository Repository = repository;

    protected abstract Expression<Func<TEntity, TReadDto>> ToReadDto { get; }

    public virtual async Task<OperationResult<List<TReadDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await Repository.GetAllAsync(selector: ToReadDto, cancellationToken: cancellationToken);

        return OperationResult<List<TReadDto>>.Success(result);
    }

    public virtual async Task<OperationResult<PageData<TReadDto>>> GetAllAsync(PageOptions options,
        string? orderBy = null,
        IReadOnlyList<Expression<Func<TEntity, object>>>? allowedToOrderBy = null,
        CancellationToken cancellationToken = default)
    {
        PageData<TReadDto> result;

        if (orderBy is null)
        {
            result = await Repository.GetAllAsync(options,
                orderBy: null,
                selector: ToReadDto,
                cancellationToken: cancellationToken);
        }
        else
        {
            var validationResult = DynamicOrderingHelper.Validate(orderBy, allowedToOrderBy);
            if (!validationResult.Succeeded)
                return OperationResult<PageData<TReadDto>>.Failure(validationResult.Messages[0]);

            result = await Repository.GetAllAsync(options,
                orderByString: orderBy,
                selector: ToReadDto,
                cancellationToken: cancellationToken);
        }

        return OperationResult<PageData<TReadDto>>.Success(result);
    }

    public virtual async Task<OperationResult<InfinitePageData<TReadDto>>> GetAllAsync(InfinitePageOptions options,
        string? orderBy = null,
        IReadOnlyList<Expression<Func<TEntity, object>>>? allowedToOrderBy = null,
        CancellationToken cancellationToken = default)
    {
        InfinitePageData<TReadDto> result;

        if (orderBy is null)
        {
            result = await Repository.GetAllAsync(options,
                orderBy: [e => e.Id!],
                isDescending: [true],
                selector: ToReadDto,
                cancellationToken: cancellationToken);
        }
        else
        {
            var validationResult = DynamicOrderingHelper.Validate(orderBy, allowedToOrderBy);
            if (!validationResult.Succeeded)
                return OperationResult<InfinitePageData<TReadDto>>.Failure(validationResult.Messages[0]);

            result = await Repository.GetAllAsync(options,
                orderByString: orderBy,
                selector: ToReadDto,
                cancellationToken: cancellationToken);
        }

        return OperationResult<InfinitePageData<TReadDto>>.Success(result);
    }

    public virtual async Task<OperationResult<TReadDto>> GetByIdAsync(TKey id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync([id], cancellationToken);

        if (entity is null)
        {
            return OperationResult<TReadDto>.Failure(
                new OperationResultMessage(ErrorMessages.EntityNotFound, OperationResultSeverity.Error));
        }

        var readDto = CacheHelper.CompileToReadDtoExpression(ToReadDto).Invoke(entity);

        return OperationResult<TReadDto>.Success(readDto);
    }
}
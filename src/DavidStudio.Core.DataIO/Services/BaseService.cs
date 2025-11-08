using DavidStudio.Core.DataIO.Entities;
using DavidStudio.Core.DataIO.Expressions;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.DataIO.Repositories;
using DavidStudio.Core.DataIO.UnitOfWork.EfCore;
using DavidStudio.Core.Results;
using DavidStudio.Core.Results.Generic;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Services;

public abstract class BaseService<TDbContext, TRepository, TEntity, TKey, TCreateModel, TUpdateModel, TReadDto>(
    TRepository repository,
    IEfUnitOfWork<TDbContext> unitOfWork)
    : BaseReadonlyService<TRepository, TEntity, TKey, TReadDto>(repository),
        IBaseService<TEntity, TKey, TCreateModel, TUpdateModel, TReadDto>
    where TDbContext : DbContext
    where TRepository : class, IBaseRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, ISelfManageable<TEntity, TCreateModel, TUpdateModel>
    where TKey : new()
    where TCreateModel : class
    where TUpdateModel : class
    where TReadDto : class
{
    protected readonly IEfUnitOfWork<TDbContext> UnitOfWork = unitOfWork;

    public virtual async Task<OperationResult<TReadDto>> CreateAsync(TCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = TEntity.Create(model);

        await Repository.CreateAsync(entity, cancellationToken);
        await UnitOfWork.SaveAsync(cancellationToken);

        var readDto = CacheHelper.CompileToReadDtoExpression(ToReadDto).Invoke(entity);

        return OperationResult<TReadDto>.Success(readDto);
    }

    public virtual async Task<OperationResult<TReadDto>> UpdateAsync(TKey id,
        TUpdateModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync([id], cancellationToken);

        if (entity is null)
        {
            return OperationResult<TReadDto>.Failure(
                new OperationResultMessage(ErrorMessages.NotFound, OperationResultSeverity.Error));
        }

        entity.Update(model);

        Repository.Update(entity);
        await UnitOfWork.SaveAsync(cancellationToken);
        
        var readDto = CacheHelper.CompileToReadDtoExpression(ToReadDto).Invoke(entity);
        
        return OperationResult<TReadDto>.Success(readDto);
    }

    public virtual async Task<OperationResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync([id], cancellationToken);

        if (entity is null)
        {
            return OperationResult<TReadDto>.Failure(
                new OperationResultMessage(ErrorMessages.NotFound, OperationResultSeverity.Error));
        }

        await Repository.DeleteAsync(entity.Id, cancellationToken: cancellationToken);
        await UnitOfWork.SaveAsync(cancellationToken);

        return OperationResult.Success();
    }
}
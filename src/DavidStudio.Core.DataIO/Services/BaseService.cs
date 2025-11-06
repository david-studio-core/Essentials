// using DavidStudio.Core.DataIO.Entities;
// using DavidStudio.Core.DataIO.Repositories;
// using DavidStudio.Core.DataIO.UnitOfWork.EfCore;
//
// namespace DavidStudio.Core.DataIO.Services;
//
// public abstract class BaseService<TRepository, TContext, TEntity, TKey>(
//     TRepository repository,
//     IEfUnitOfWork<TContext> unitOfWork) : IBaseService<TEntity, TKey>
//     where TRepository : class, IBaseRepository<TEntity, TKey>
//     where TContext : DbContext
//     where TEntity : class, IEntity<TKey>
//     where TKey : new()
// {
//     protected readonly TRepository Repository = repository;
//     protected readonly IEfUnitOfWork<TContext> UnitOfWork = unitOfWork;
//
//     public virtual async Task<OperationResult<PaginatedResult<TEntity>>> GetAllAsync(PageOptions options)
//     {
//         var data = await Repository.GetAllAsync(options, e => e);
//
//         return new PaginatedResult<TEntity>(data, options);
//     }
//
//     public virtual async Task<OperationResult<IReadOnlyCollection<TEntity>>> GetAllAsync()
//     {
//         var entities = await Repository.GetAllAsync(e => e);
//
//         return entities.ToArray();
//     }
//
//     public virtual async Task<OperationResult<TEntity>> GetByIdAsync(TKey id)
//     {
//         var entity = await Repository.GetByIdAsync(id);
//
//         return entity ?? OperationResult<TEntity>.Failure(
//             new OperationResultMessage(ErrorMessages.EntityNotFound, OperationResultSeverity.Error));
//     }
//
//     public virtual async Task<OperationResult<TEntity>> CreateAsync(TEntity model)
//     {
//         await Repository.CreateAsync(model);
//         await UnitOfWork.SaveAsync();
//
//         return OperationResult<TEntity>.Success(model);
//     }
//
//     public virtual async Task<OperationResult> UpdateAsync(TEntity model)
//     {
//         var entity = await Repository.GetByIdAsync(model.Id);
//         if (entity is null)
//         {
//             return OperationResult.Failure(
//                 new OperationResultMessage(ErrorMessages.EntityNotFound, OperationResultSeverity.Error));
//         }
//
//         Repository.Update(model);
//         await UnitOfWork.SaveAsync();
//
//         return OperationResult.Success();
//     }
//
//     public virtual async Task<OperationResult> DeleteAsync(TKey id)
//     {
//         var entity = await Repository.GetByIdAsync(id);
//         if (entity is null)
//         {
//             return OperationResult.Failure(
//                 new OperationResultMessage(ErrorMessages.EntityNotFound, OperationResultSeverity.Error));
//         }
//
//         await Repository.DeleteAsync(entity.Id);
//         await UnitOfWork.SaveAsync();
//
//         return OperationResult.Success();
//     }
// }
//
// public abstract class BaseService<TRepository, TContext, TEntity, TKey, TCreateDto, TCreateRequest, TReadDto>(
//     TRepository repository,
//     IEfUnitOfWork<TContext> unitOfWork,
//     Func<TEntity, TReadDto> toReadDto) : IBaseService<TKey, TCreateDto, TReadDto>
//     where TRepository : class, IBaseRepository<TEntity, TKey>
//     where TContext : DbContext
//     where TEntity : class, IEntity<TKey>, ISelfManageable<TEntity, TCreateRequest>
//     where TKey : new()
//     where TCreateDto : class
//     where TReadDto : class
// {
//     protected readonly TRepository Repository = repository;
//     protected readonly IEfUnitOfWork<TContext> UnitOfWork = unitOfWork;
//
//     protected abstract TCreateRequest ToCreateRequest(TCreateDto model);
//
//     public virtual async Task<OperationResult<PaginatedResult<TReadDto>>> GetAllAsync(PageOptions options)
//     {
//         var data = await Repository.GetAllAsync(options, e => toReadDto(e));
//
//         return new PaginatedResult<TReadDto>(data, options);
//     }
//
//     public virtual async Task<OperationResult<IReadOnlyCollection<TReadDto>>> GetAllAsync()
//     {
//         var entities = await Repository.GetAllAsync(e => toReadDto(e));
//
//         return entities.ToArray();
//     }
//
//     public virtual async Task<OperationResult<TReadDto>> GetByIdAsync(TKey id)
//     {
//         var entity = await Repository.GetByIdAsync(id);
//
//         if (entity is null)
//         {
//             return OperationResult<TReadDto>.Failure(
//                 new OperationResultMessage(ErrorMessages.EntityNotFound, OperationResultSeverity.Error));
//         }
//
//         return toReadDto(entity);
//     }
//
//     public virtual async Task<OperationResult<TReadDto>> CreateAsync(TCreateDto dto)
//     {
//         var entity = TEntity.Create(ToCreateRequest(dto));
//
//         await Repository.CreateAsync(entity);
//         await UnitOfWork.SaveAsync();
//
//         return OperationResult<TReadDto>.Success(toReadDto(entity));
//     }
//
//     public virtual async Task<OperationResult> DeleteAsync(TKey id)
//     {
//         var entity = await Repository.GetByIdAsync(id);
//
//         if (entity is null)
//         {
//             return OperationResult.Failure(
//                 new OperationResultMessage(ErrorMessages.EntityNotFound, OperationResultSeverity.Error));
//         }
//
//         await Repository.DeleteAsync(entity.Id);
//         await UnitOfWork.SaveAsync();
//
//         return OperationResult.Success();
//     }
// }
//
// public abstract class BaseService<TRepository, TContext, TEntity, TKey, TCreateDto, TCreateRequest, TUpdateDto, TUpdateRequest, TReadDto>(
//     TRepository repository,
//     IEfUnitOfWork<TContext> unitOfWork,
//     Func<TEntity, TReadDto> toReadDto)
//     : BaseService<TRepository, TContext, TEntity, TKey, TCreateDto, TCreateRequest, TReadDto>(repository, unitOfWork, toReadDto),
//         IBaseService<TKey, TCreateDto, TUpdateDto, TReadDto>
//     where TRepository : class, IBaseRepository<TEntity, TKey>
//     where TContext : DbContext
//     where TEntity : class, IEntity<TKey>, ISelfManageable<TEntity, TCreateRequest, TUpdateRequest>
//     where TKey : new()
//     where TCreateDto : class
//     where TCreateRequest : class
//     where TUpdateDto : class
//     where TUpdateRequest : class
//     where TReadDto : class
// {
//     protected abstract TUpdateRequest ToUpdateRequest(TUpdateDto model);
//
//     public virtual async Task<OperationResult> UpdateAsync(TKey id, TUpdateDto dto)
//     {
//         var entity = await Repository.GetByIdAsync(id);
//
//         if (entity is null)
//         {
//             return OperationResult.Failure(
//                 new OperationResultMessage(ErrorMessages.EntityNotFound, OperationResultSeverity.Error));
//         }
//
//         entity.Update(ToUpdateRequest(dto));
//
//         Repository.Update(entity);
//         await UnitOfWork.SaveAsync();
//
//         return OperationResult.Success();
//     }
// }
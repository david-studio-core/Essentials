// using DavidStudio.Core.DataIO.Entities;
//
// namespace DavidStudio.Core.DataIO.Services;
//
// public interface IBaseService<TEntity, in TKey>
//     where TEntity : class, IEntity<TKey>
//     where TKey : new()
// {
//     public Task<OperationResult<IReadOnlyCollection<TEntity>>> GetAllAsync();
//     public Task<OperationResult<PaginatedResult<TEntity>>> GetAllAsync(PageOptions options);
//     public Task<OperationResult<TEntity>> GetByIdAsync(TKey id);
//     public Task<OperationResult<TEntity>> CreateAsync(TEntity entity);
//     public Task<OperationResult> UpdateAsync(TEntity entity);
//     public Task<OperationResult> DeleteAsync(TKey id);
// }
//
// public interface IBaseService<in TKey, in TCreateDto, TReadDto>
//     where TKey : new()
//     where TCreateDto : class
//     where TReadDto : class
// {
//     public Task<OperationResult<IReadOnlyCollection<TReadDto>>> GetAllAsync();
//     public Task<OperationResult<PaginatedResult<TReadDto>>> GetAllAsync(PageOptions options);
//     public Task<OperationResult<TReadDto>> GetByIdAsync(TKey id);
//     public Task<OperationResult<TReadDto>> CreateAsync(TCreateDto dto);
//     public Task<OperationResult> DeleteAsync(TKey id);
// }
//
// public interface IBaseService<in TKey, in TCreateDto, in TUpdateDto, TReadDto>
//     : IBaseService<TKey, TCreateDto, TReadDto>
//     where TKey : new()
//     where TCreateDto : class
//     where TUpdateDto : class
//     where TReadDto : class
// {
//     public Task<OperationResult> UpdateAsync(TKey id, TUpdateDto dto);
// }
namespace DavidStudio.Core.DataIO.UnitOfWork;

public interface IBaseUnitOfWork
{
    Task CreateTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
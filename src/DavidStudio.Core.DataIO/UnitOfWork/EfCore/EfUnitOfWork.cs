using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DavidStudio.Core.DataIO.UnitOfWork.EfCore;

public class EfUnitOfWork<TContext>(TContext context) : IEfUnitOfWork<TContext>, IDisposable
    where TContext : DbContext
{
    public TContext Context { get; } = context;
    public IDbContextTransaction? Transaction { get; private set; }

    private bool _disposed = false;

    public async Task CreateTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        Transaction = await Context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        await Transaction.CommitAsync(cancellationToken);
        await Transaction.DisposeAsync();

        Transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null)
            throw new InvalidOperationException("No active transaction to rollback.");

        await Transaction.RollbackAsync(cancellationToken);
        await Transaction.DisposeAsync();

        Transaction = null;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Transaction?.Dispose();
            Transaction = null;
            Context.Dispose();
        }

        _disposed = true;
    }

    ~EfUnitOfWork() => Dispose(false);
}
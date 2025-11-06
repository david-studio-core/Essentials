using System.Data;
using Microsoft.Data.SqlClient;

namespace DavidStudio.Core.DataIO.UnitOfWork.ADO.NET;

public class AdoUnitOfWork(string connectionString) : IAdoUnitOfWork, IDisposable, IAsyncDisposable
{
    private SqlConnection? _connection;
    private SqlTransaction? _transaction;

    private bool _disposed = false;

    public SqlConnection Connection => _connection ?? throw new InvalidOperationException("Connection is not initialized.");
    public SqlTransaction Transaction => _transaction ?? throw new InvalidOperationException("Transaction is not initialized.");

    public async Task OpenConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection == null || _connection.State == ConnectionState.Broken)
        {
            _connection?.Dispose();
            _connection = new SqlConnection(connectionString);
        }

        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync(cancellationToken);
    }

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        await OpenConnectionAsync(cancellationToken);

        _transaction = (SqlTransaction)await _connection!.BeginTransactionAsync(cancellationToken);

        try
        {
            await action();
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await _transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task CreateTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        await OpenConnectionAsync(cancellationToken);

        _transaction = (SqlTransaction)await _connection!.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _transaction.CommitAsync(cancellationToken);

        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No active transaction to rollback.");

        await _transaction.RollbackAsync(cancellationToken);

        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _transaction?.Dispose();
            _transaction = null;

            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }

        _disposed = true;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync().ConfigureAwait(false);

        if (_connection is not null)
        {
            await _connection.CloseAsync().ConfigureAwait(false);
            await _connection.DisposeAsync().ConfigureAwait(false);
        }

        _connection = null;
        _transaction = null;
    }

    ~AdoUnitOfWork() => Dispose(false);
}
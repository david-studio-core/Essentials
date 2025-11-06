using Microsoft.Data.SqlClient;

namespace DavidStudio.Core.DataIO.UnitOfWork.ADO.NET;

public interface IAdoUnitOfWork : IBaseUnitOfWork
{
    SqlConnection Connection { get; }
    SqlTransaction Transaction { get; }

    Task OpenConnectionAsync(CancellationToken cancellationToken = default);
    
    Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
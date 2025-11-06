using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DavidStudio.Core.DataIO.UnitOfWork.EfCore;

public interface IEfUnitOfWork<out TContext> : IBaseUnitOfWork
    where TContext : DbContext
{
    TContext Context { get; }
    IDbContextTransaction? Transaction { get; }

    Task SaveAsync(CancellationToken cancellationToken = default);
}

using DavidStudio.Core.DataIO.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DavidStudio.Core.DataIO.Interceptors;

/// <summary>
/// Intercepts <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> calls
/// to automatically manage timestamp fields for entities implementing the <see cref="ITimedEntity"/> interface.
/// </summary>
/// <remarks>
/// This interceptor ensures that entities are automatically assigned and updated with
/// <see cref="ITimedEntity.CreatedAtUtc"/> and <see cref="ITimedEntity.ModifiedAtUtc"/> values
/// whenever they are added or modified in the <see cref="DbContext"/>.
/// <para>
/// By using UTC timestamps, this approach maintains consistent time tracking across distributed systems
/// and different time zones.
/// </para>
/// </remarks>
public class TimedEntitiesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = eventData.Context.ChangeTracker.Entries<ITimedEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    entry.Entity.ModifiedAtUtc = entry.Entity.CreatedAtUtc;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAtUtc = DateTime.UtcNow;
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
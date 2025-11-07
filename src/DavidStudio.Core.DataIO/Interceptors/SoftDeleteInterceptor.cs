using DavidStudio.Core.DataIO.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DavidStudio.Core.DataIO.Interceptors;

/// <summary>
/// Intercepts <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> calls
/// to automatically convert physical deletions into soft deletions for entities implementing
/// the <see cref="ISoftDeletable"/> interface.
/// </summary>
/// <remarks>
/// This interceptor ensures that entities marked for deletion are not physically removed
/// from the database. Instead, they are updated with <see cref="ISoftDeletable.IsDeleted"/> set to <see langword="true"/>.
/// <para>
/// This allows applications to retain historical data and support recovery scenarios,
/// while still excluding deleted records from normal queries (when filtered appropriately).
/// </para>
/// </remarks>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = eventData.Context.ChangeTracker.Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
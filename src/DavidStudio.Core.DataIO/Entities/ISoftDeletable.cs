namespace DavidStudio.Core.DataIO.Entities;

/// <summary>
/// Marks an entity that supports soft deletion.
/// </summary>
/// <remarks>
/// Soft deletion allows marking an entity as deleted without physically removing it
/// from the data store. This enables scenarios such as data recovery,
/// historical tracking, or audit logging.
/// </remarks>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
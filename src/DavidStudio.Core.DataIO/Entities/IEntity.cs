namespace DavidStudio.Core.DataIO.Entities;

/// <summary>
/// Represents an entity with a strongly-typed primary key.
/// </summary>
/// <typeparam name="TKey">The type of the primary key.</typeparam>
public interface IEntity<TKey>
{
    /// <summary>
    /// Gets or sets the unique identifier for this entity.
    /// </summary>
    public TKey Id { get; set; }
}
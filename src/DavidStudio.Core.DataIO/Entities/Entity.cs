using System.ComponentModel.DataAnnotations;

namespace DavidStudio.Core.DataIO.Entities;

public abstract class Entity<TKey> : IEntity<TKey>
    where TKey : struct
{
    [Key]
    public TKey Id { get; set; }
}
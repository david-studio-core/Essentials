namespace DavidStudio.Core.DataIO.Entities;

public interface ITimedEntity
{
    DateTime CreatedAtUtc { get; set; }
    DateTime ModifiedAtUtc { get; set; }
}
namespace DavidStudio.Core.DataIO.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
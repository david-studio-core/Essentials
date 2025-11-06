using DavidStudio.Core.DataIO.Entities;

namespace DavidStudio.Core.DataIO.Benchmarks.Assets;

public class TestEntity : Entity<int>
{
    public string Name { get; set; } = null!;
    public int Year { get; set; }
}
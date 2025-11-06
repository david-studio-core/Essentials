using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Benchmarks.Assets;

public class TestDbContext(DbContextOptions<TestDbContext> options)
    : DbContext(options)
{
    public DbSet<TestEntity> TestEntities { get; init; } = null!;
}
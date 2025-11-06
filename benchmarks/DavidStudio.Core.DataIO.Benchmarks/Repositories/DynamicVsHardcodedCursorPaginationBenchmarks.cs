using BenchmarkDotNet.Attributes;
using DavidStudio.Core.DataIO.Benchmarks.Assets;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Benchmarks.Repositories;

[MemoryDiagnoser]
[RankColumn]
public class DynamicVsHardcodedCursorPaginationBenchmarks
{
    private TestDbContext _dbContext = null!;
    private TestRepository _testRepository = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TestDbContext(dbContextOptions);
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var a = new TestEntity
        {
            Name = "A",
            Year = 2022
        };
        var b = new TestEntity
        {
            Name = "B",
            Year = 2024
        };
        var c = new TestEntity
        {
            Name = "C",
            Year = 2023
        };

        await _dbContext.TestEntities.AddRangeAsync(a, b, c);
        await _dbContext.SaveChangesAsync();

        _testRepository = new TestRepository(_dbContext);
    }

    [Benchmark]
    public async Task DynamicCursorPaginationFirstPage()
    {
        await _testRepository.GetAllAsync(
            options: new InfinitePageOptions(10, searchAfter: null),
            orderBy:
            [
                e => e.Name,
                e => e.Year,
                e => e.Id
            ],
            isDescending: [false, true, true],
            selector: e => e
        );
    }
    
    [Benchmark]
    public async Task DynamicCursorPaginationLastPage()
    {
        await _testRepository.GetAllAsync(
            options: new InfinitePageOptions(10, searchAfter: new DynamicCursor(["B", 2024, 2])),
            orderBy:
            [
                e => e.Name,
                e => e.Year,
                e => e.Id
            ],
            isDescending: [false, true, true],
            selector: e => e
        );
    }

    [Benchmark]
    public async Task HardcodedCursorPaginationFirstPage()
    {
        await HardcodedCursorPaginationExampleFirstPage();
    }
    
    [Benchmark]
    public async Task HardcodedCursorPaginationLastPage()
    {
        await HardcodedCursorPaginationExampleLastPage();
    }

    private async Task<InfinitePageData<TestEntity>> HardcodedCursorPaginationExampleFirstPage()
    {
        const int pageSize = 10;

        var entities = await _dbContext.TestEntities
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ThenByDescending(e => e.Year)
            .ThenByDescending(e => e.Id)
            .Take(pageSize + 1)
            .ToListAsync();

        return new InfinitePageData<TestEntity>(
            entities.Take(pageSize).ToList(),
            null,
            entities.Count > pageSize
        );
    }
    
    private async Task<InfinitePageData<TestEntity>> HardcodedCursorPaginationExampleLastPage()
    {
        const int pageSize = 10;

        var entities = await _dbContext.TestEntities
            .AsNoTracking()
            .Where(e => string.Compare(e.Name, "B") > 0 || 
                        (string.Equals(e.Name, "B") && e.Year < 2024) || 
                        (string.Equals(e.Name, "B") && e.Year == 2024 && e.Id < 2))
            .OrderBy(e => e.Name)
            .ThenByDescending(e => e.Year)
            .ThenByDescending(e => e.Id)
            .Take(pageSize + 1)
            .ToListAsync();

        return new InfinitePageData<TestEntity>(
            entities.Take(pageSize).ToList(),
            null,
            entities.Count > pageSize
        );
    }
}
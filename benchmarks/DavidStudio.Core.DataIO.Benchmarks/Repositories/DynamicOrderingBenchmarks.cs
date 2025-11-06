using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using DavidStudio.Core.DataIO.Benchmarks.Assets;
using DavidStudio.Core.DataIO.Builders;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Benchmarks.Repositories;

[MemoryDiagnoser]
[RankColumn]
public class DynamicOrderingBenchmarks
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
    public async Task DynamicCursorPagination()
    {
        await _testRepository.GetAllAsync(
            options: new InfinitePageOptions(10, searchAfter: null),
            selector: e => e,
            orderByString: "id desc"
        );
    }

    [Benchmark]
    public async Task DynamicOffsetPagination()
    {
        await _testRepository.GetAllAsync(
            options: new PageOptions(1, 10),
            selector: e => e,
            orderByString: "id desc"
        );
    }

    [Benchmark]
    public async Task HardcodedDynamicLinqCursorPagination()
    {
        await HardcodedDynamicLinqCursorPaginationExample();
    }
    
    private async Task<InfinitePageData<TestEntity>> HardcodedDynamicLinqCursorPaginationExample()
    {
        var options = new InfinitePageOptions(10, searchAfter: null);
    
        var query = _dbContext.TestEntities
            .AsNoTracking()
            .OrderBy("id desc");
    
        var entities = await query
            .Take(options.Size)
            .ToListAsync();
    
        return new InfinitePageData<TestEntity>(
            entities.Take(options.Size).ToList(),
            lastCursor: null,
            hasNextPage: entities.Count > options.Size
        );
    }
    
    [Benchmark]
    public async Task HardcodedDynamicLinqOffsetPagination()
    {
        await HardcodedDynamicLinqOffsetPaginationExample();
    }
    
    private async Task<PageData<TestEntity>> HardcodedDynamicLinqOffsetPaginationExample()
    {
        var options = new PageOptions(1, 10);
    
        var query = _dbContext.TestEntities
            .AsNoTracking()
            .OrderBy("id desc");
    
        var totalCount = await query.CountAsync();
    
        var entities = await query
            .Skip((options.Page - 1) * options.Size)
            .Take(options.Size)
            .ToListAsync();
    
        return new PageData<TestEntity>(
            entities,
            totalCount,
            options
        );
    }

    [Benchmark]
    public async Task HardcodedCursorPagination()
    {
        await HardcodedCursorPaginationExample();
    }

    private async Task<InfinitePageData<TestEntity>> HardcodedCursorPaginationExample()
    {
        var options = new InfinitePageOptions(10, searchAfter: null);

        var query = _dbContext.TestEntities
            .AsNoTracking()
            .OrderByDescending(e => e.Id);

        var entities = await query
            .Take(options.Size + 1)
            .ToListAsync();

        return new InfinitePageData<TestEntity>(
            entities.Take(options.Size).ToList(),
            lastCursor: null,
            hasNextPage: entities.Count > options.Size
        );
    }

    [Benchmark]
    public async Task HardcodedOffsetPagination()
    {
        await HardcodedOffsetPaginationExample();
    }

    private async Task<PageData<TestEntity>> HardcodedOffsetPaginationExample()
    {
        var options = new PageOptions(1, 10);

        var query = _dbContext.TestEntities
            .AsNoTracking()
            .OrderByDescending(e => e.Id);

        var totalCount = await query.CountAsync();

        var entities = await query
            .Skip((options.Page - 1) * options.Size)
            .Take(options.Size)
            .ToListAsync();

        return new PageData<TestEntity>(
            entities,
            totalCount,
            options
        );
    }
}
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using DavidStudio.Core.DataIO.Benchmarks.Assets;
using DavidStudio.Core.DataIO.Builders;
using DavidStudio.Core.DataIO.Helpers;
using DavidStudio.Core.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.DataIO.Benchmarks.Repositories;

[MemoryDiagnoser]
[RankColumn]
public class DynamicVsHardcodedOrderingBenchmarks
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
    public async Task DynamicOffsetPagination()
    {
        await _testRepository.GetAllAsync(
            options: new PageOptions(1, 2),
            selector: e => e,
            orderBy: "id desc",
            allowedToOrderBy:
            [
                e => e.Name,
                e => e.Year,
                e => e.Id
            ]
        );
    }

    [Benchmark]
    public async Task HardcodedDynamicLinqWithBuilder()
    {
        await HardcodedDynamicLinqWithBuilderExample();
    }

    [Benchmark]
    public async Task HardcodedDynamicLinq()
    {
        await HardcodedDynamicLinqExample();
    }

    [Benchmark]
    public async Task Hardcoded()
    {
        await HardcodedExample();
    }

    private async Task<PageData<TestEntity>> HardcodedDynamicLinqWithBuilderExample()
    {
        var options = new PageOptions(1, 2);
        var orderBy = "id desc";
        List<Expression<Func<TestEntity, object>>> allowedToOrderBy =
        [
            e => e.Name,
            e => e.Year,
            e => e.Id
        ];

        DynamicOrderByHelper.EnsureOrderByAllowed(orderBy, allowedToOrderBy);

        var query = _dbContext.TestEntities
            .AsNoTracking()
            .OrderBy(DynamicOrderByQueryBuilder.BuildString(orderBy));

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

    private async Task<PageData<TestEntity>> HardcodedDynamicLinqExample()
    {
        var options = new PageOptions(1, 2);

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

    private async Task<PageData<TestEntity>> HardcodedExample()
    {
        var options = new PageOptions(1, 2);

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
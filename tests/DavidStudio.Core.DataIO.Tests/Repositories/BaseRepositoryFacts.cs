using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Entities;
using DavidStudio.Core.DataIO.Repositories;
using DavidStudio.Core.DataIO.Services;
using DavidStudio.Core.Pagination;
using DavidStudio.Core.Pagination.InfiniteScroll;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace DavidStudio.Core.DataIO.Tests.Repositories;

public class BaseRepositoryFacts : IAsyncLifetime
{
    private readonly MsSqlContainer _msSql = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public Task InitializeAsync()
    {
        return _msSql.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _msSql.DisposeAsync().AsTask();
    }

    private protected class TestEntity : Entity<int>
    {
        public string Name { get; set; } = null!;
        public int Year { get; set; }
    }

    private protected record TestEntityReadDto(int Id, string Name, int Year);

    private protected class TestEntityMapper
    {
        public static TestEntityReadDto ToReadDto(TestEntity entity)
        {
            return new TestEntityReadDto(entity.Id, entity.Name, entity.Year);
        }
    }

    private protected class TestDbContext(DbContextOptions<TestDbContext> options)
        : DbContext(options)
    {
        public DbSet<TestEntity> TestEntities { get; init; } = null!;
    }

    private protected interface ITestRepository : IBaseRepository<TestEntity, int>;

    private protected interface ITestService : IBaseReadonlyService<TestEntity, int, TestEntityReadDto>;

    private protected class TestRepository(TestDbContext context)
        : BaseRepository<TestEntity, int>(context), ITestRepository;

    private protected class TestService(ITestRepository repository)
        : BaseReadonlyService<ITestRepository, TestEntity, int, TestEntityReadDto>(repository), ITestService
    {
        protected override Expression<Func<TestEntity, TestEntityReadDto>> ToReadDto => e => new TestEntityReadDto(e.Id, e.Name, e.Year);
    }

    // TODO: Transform into complex scenarios
    [Fact]
    public async Task Temp()
    {
        // Arrange
        var builder = new SqlConnectionStringBuilder(_msSql.GetConnectionString())
        {
            InitialCatalog = $"TestDb_{Guid.NewGuid():N}"
        };
        var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseLoggerFactory(Logger.TestsLoggerFactory)
            .UseSqlServer(builder.ConnectionString)
            .Options;

        await using var dbContext = new TestDbContext(dbContextOptions);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var a = new TestEntity
        {
            Name = "A",
            Year = 2022
        };
        var b1 = new TestEntity
        {
            Name = "B",
            Year = 2024
        };
        var b2 = new TestEntity
        {
            Name = "B",
            Year = 2023
        };
        var c = new TestEntity
        {
            Name = "C",
            Year = 2024
        };

        await dbContext.TestEntities.AddRangeAsync(a, b1, b2, c);

        // var a = new TestEntity
        // {
        //     Name = "A",
        //     Year = 2022
        // };
        // var b = new TestEntity
        // {
        //     Name = "B",
        //     Year = 2024
        // };
        // var c = new TestEntity
        // {
        //     Name = "C",
        //     Year = 2023
        // };
        //
        // await dbContext.TestEntities.AddRangeAsync(a, b, c);

        await dbContext.SaveChangesAsync();

        ITestRepository repository = new TestRepository(dbContext);

        // Act
        var result = await repository.GetAllAsync(
            // options: new InfinitePageOptions(1, searchAfterToken: "WyJCIiwyMDI0LDJd"),
            options: new InfinitePageOptions(1, searchAfterToken: null),
            orderBy:
            [
                e => e.Name,
                e => e.Year,
                o => o.Id
            ],
            isDescending: [false, true, true],
            // isDescending: [false],
            selector: e => e.Name
        );

        // Assert
        Assert.Equal(4, await dbContext.TestEntities.CountAsync());
    }

    // TODO: Transform into complex scenarios
    [Fact]
    public async Task Temp2()
    {
        // Arrange
        var builder = new SqlConnectionStringBuilder(_msSql.GetConnectionString())
        {
            InitialCatalog = $"TestDb_{Guid.NewGuid():N}"
        };
        var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseLoggerFactory(Logger.TestsLoggerFactory)
            .UseSqlServer(builder.ConnectionString)
            .Options;

        await using var dbContext = new TestDbContext(dbContextOptions);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var a = new TestEntity
        {
            Name = "A",
            Year = 2022
        };
        var b1 = new TestEntity
        {
            Name = "B",
            Year = 2024
        };
        var b2 = new TestEntity
        {
            Name = "B",
            Year = 2023
        };
        var c = new TestEntity
        {
            Name = "C",
            Year = 2024
        };

        await dbContext.TestEntities.AddRangeAsync(a, b1, b2, c);
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository(dbContext);

        // Act
        var result = await repository.GetAllAsync(
            selector: e => e.Name,
            predicate: e => e.Id > 0 && e.Id < 10,
            orderBy: o => o.OrderByDescending(e => e.Id)
        );

        // Assert
        Assert.Equal(4, await dbContext.TestEntities.CountAsync());
    }

    // TODO: Transform into complex scenarios
    [Fact]
    public async Task Temp3()
    {
        // Arrange
        var builder = new SqlConnectionStringBuilder(_msSql.GetConnectionString())
        {
            InitialCatalog = $"TestDb_{Guid.NewGuid():N}"
        };
        var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseLoggerFactory(Logger.TestsLoggerFactory)
            .UseSqlServer(builder.ConnectionString)
            .Options;

        await using var dbContext = new TestDbContext(dbContextOptions);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var a = new TestEntity
        {
            Name = "A",
            Year = 2022
        };
        var b1 = new TestEntity
        {
            Name = "B",
            Year = 2024
        };
        var b2 = new TestEntity
        {
            Name = "B",
            Year = 2023
        };
        var c = new TestEntity
        {
            Name = "C",
            Year = 2024
        };

        await dbContext.TestEntities.AddRangeAsync(a, b1, b2, c);
        await dbContext.SaveChangesAsync();

        ITestRepository repository = new TestRepository(dbContext);

        // Act
        var result1 = await repository.GetAllAsync(
            new PageOptions(1, 2),
            selector: e => e.Name,
            orderByString: "id desc"
        );
        
        var result2 = await repository.GetAllAsync(
            new PageOptions(2, 2),
            selector: e => e.Name,
            orderByString: "id desc"
        );

        // Assert
        Assert.Equal(4, await dbContext.TestEntities.CountAsync());
    }

    // TODO: Transform into complex scenarios
    [Fact]
    public async Task Temp4()
    {
        // Arrange
        var builder = new SqlConnectionStringBuilder(_msSql.GetConnectionString())
        {
            InitialCatalog = $"TestDb_{Guid.NewGuid():N}"
        };
        var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseLoggerFactory(Logger.TestsLoggerFactory)
            .UseSqlServer(builder.ConnectionString)
            .Options;

        await using var dbContext = new TestDbContext(dbContextOptions);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var a = new TestEntity
        {
            Name = "A",
            Year = 2022
        };
        var b1 = new TestEntity
        {
            Name = "B",
            Year = 2024
        };
        var b2 = new TestEntity
        {
            Name = "B",
            Year = 2023
        };
        var c = new TestEntity
        {
            Name = "C",
            Year = 2024
        };

        await dbContext.TestEntities.AddRangeAsync(a, b1, b2, c);
        await dbContext.SaveChangesAsync();

        ITestRepository repository = new TestRepository(dbContext);

        // Act
        var result = await repository.GetAllAsync(
            new InfinitePageOptions(size: 2, searchAfter: null),
            selector: e => e.Name,
            predicate: e => e.Id > 0 && e.Id < 10,
            orderByString: "id2 desc"
        );

        // Assert
        Assert.Equal(4, await dbContext.TestEntities.CountAsync());
    }

    // TODO: Transform into complex scenarios
    [Fact]
    public async Task Temp5()
    {
        // Arrange
        var builder = new SqlConnectionStringBuilder(_msSql.GetConnectionString())
        {
            InitialCatalog = $"TestDb_{Guid.NewGuid():N}"
        };
        var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseLoggerFactory(Logger.TestsLoggerFactory)
            .UseSqlServer(builder.ConnectionString)
            .Options;

        await using var dbContext = new TestDbContext(dbContextOptions);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var a = new TestEntity
        {
            Name = "A",
            Year = 2022
        };
        var b1 = new TestEntity
        {
            Name = "B",
            Year = 2024
        };
        var b2 = new TestEntity
        {
            Name = "B",
            Year = 2023
        };
        var c = new TestEntity
        {
            Name = "C",
            Year = 2024
        };

        await dbContext.TestEntities.AddRangeAsync(a, b1, b2, c);
        await dbContext.SaveChangesAsync();

        ITestRepository repository = new TestRepository(dbContext);
        ITestService service = new TestService(repository);

        // Act
        var result = await service.GetAllAsync(
            new PageOptions(page: 1, size: 2),
            orderBy: "id desc",
            // allowedToOrderBy: null
            allowedToOrderBy:
            [
                e => e.Name,
                e => e.Year,
                e => e.Id
            ]
        );
        
        var result2 = await service.GetAllAsync(
            new InfinitePageOptions(size: 2, searchAfter: null),
            orderBy: "id desc",
            // allowedToOrderBy: null
            allowedToOrderBy:
            [
                e => e.Name,
                e => e.Year,
                e => e.Id
            ]
        );

        // Assert
        Assert.Equal(4, await dbContext.TestEntities.CountAsync());
    }
}
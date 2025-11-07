using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Entities;
using DavidStudio.Core.DataIO.Helpers;
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

    private protected class Product : Entity<int>
    {
        public string Name { get; set; } = null!;

        public ManufacturingInfo ManufacturingInfo { get; set; } = null!;
    }

    private protected class ManufacturingInfo : Entity<int>
    {
        public int Year { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }

    private protected class TestDbContext(DbContextOptions<TestDbContext> options)
        : DbContext(options)
    {
        public DbSet<Product> Products { get; init; } = null!;
        public DbSet<ManufacturingInfo> ManufacturingInfos { get; init; } = null!;
    }

    private protected interface IProductsRepo : IBaseRepository<Product, int>;

    private protected class ProductsRepo(TestDbContext context)
        : BaseRepository<Product, int>(context), IProductsRepo;

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

        var iPhone15 = new Product
        {
            Name = "iPhone 15",
            ManufacturingInfo = new ManufacturingInfo
            {
                Year = 2023
            }
        };

        var iPhone16 = new Product
        {
            Name = "iPhone 16",
            ManufacturingInfo = new ManufacturingInfo
            {
                Year = 2024
            }
        };

        var iPhone17 = new Product
        {
            Name = "iPhone 17",
            ManufacturingInfo = new ManufacturingInfo
            {
                Year = 2025
            }
        };

        await dbContext.Products.AddRangeAsync(iPhone15, iPhone16, iPhone17);
        await dbContext.SaveChangesAsync();

        IProductsRepo repository = new ProductsRepo(dbContext);

        // Act
        var result1 = await repository.GetAllAsync(
            options: new PageOptions(1, 10),
            orderByString: "ManufacturingInfo.Year desc, Id desc",
            selector: e => e.Name
        );

        var result2 = await repository.GetAllAsync(
            // options: new InfinitePageOptions(1, searchAfterToken: "WyJCIiwyMDI0LDJd"),
            options: new InfinitePageOptions(1, searchAfter: new DynamicCursor([2024, 2])),
            orderBy:
            [
                e => e.ManufacturingInfo.Year,
                o => o.Id
            ],
            isDescending: [true, true],
            selector: e => new { e.Id, e.Name, e.ManufacturingInfo.Year },
            // selector: e => e,
            include: i => i.Include(p => p.ManufacturingInfo)
        );

        var result3 = await repository.GetAllAsync(
            // options: new InfinitePageOptions(1, searchAfterToken: "WyJCIiwyMDI0LDJd"),
            options: new InfinitePageOptions(1, searchAfterToken: null),
            orderBy:
            [
                e => e.ManufacturingInfo.Year,
                o => o.Id
            ],
            isDescending: [true, true],
            selector: e => new { e.Id, e.Name, ManufacturingYear = e.ManufacturingInfo.Year },
            // selector: e => e,
            include: i => i.Include(p => p.ManufacturingInfo)
        );

        var result4 = await repository.GetAllAsync(
            // options: new InfinitePageOptions(1, searchAfterToken: "WyJCIiwyMDI0LDJd"),
            options: new InfinitePageOptions(1, searchAfterToken: null),
            orderBy:
            [
                e => e.ManufacturingInfo.Year,
                o => o.Id
            ],
            isDescending: [true, true],
            selector: e => new TestEntity { Id = e.Id, Name = e.Name, Year3 = e.ManufacturingInfo.Year },
            // selector: e => e,
            include: i => i.Include(p => p.ManufacturingInfo)
        );

        var result5 = await repository.GetAllAsync(
            // options: new InfinitePageOptions(1, searchAfterToken: "WyJCIiwyMDI0LDJd"),
            options: new InfinitePageOptions(1, searchAfterToken: null),
            orderByString: "ManufacturingInfo.Year desc, Id desc",
            selector: e => new { e.Name, e.ManufacturingInfo.Year },
            include: i => i.Include(p => p.ManufacturingInfo)
        );

        // Assert
        var data = await dbContext.Products.AsNoTracking().Include(e => e.ManufacturingInfo).ToListAsync();

        Assert.Equal(3, await dbContext.Products.CountAsync());
        Assert.Equal(3, await dbContext.ManufacturingInfos.CountAsync());
    }

    private class A
    {
        public B B { get; set; } = null!;
    }

    private class B
    {
        public C C { get; set; } = null!;
    }

    private class C
    {
        public string Name { get; set; } = null!;
        public int Year { get; set; }
    }

    private class TestEntity
    {
        public TestEntity() { }

        public TestEntity(int id, int year2, string name)
        {
            Year3 = year2;
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Year3 { get; set; }
    }

    [Fact]
    public void Temp2()
    {
        var a = new A
        {
            B = new B
            {
                C = new C
                {
                    Name = "C's name",
                    Year = 2023
                }
            }
        };

        // var name = ReflectionHelper.GetPropertyValue(a, "B.C.Name");
        //
        // var res = DynamicOrderingHelper.Validate<A>("B.C.Name desc",
        // [
        //     e => e.B.C.Year,
        //     e => e.B.C.Name
        // ]);

        // var str1 = ExpressionPropertyHelper.GetPropertyPath<A>(e => e.B.C.Name);
        // var str2 = ExpressionPropertyHelper.GetPropertyPath<A>(e => e.B.C.Year);
        // var str3 = ExpressionPropertyHelper.GetPropertyPath<A>(e => e.B);

        Expression<Func<A, object>> selector1 =
            e => new { Id = 1, e.B.C.Name, e.B.C.Year };

        Expression<Func<A, object>> selector2 =
            e => new { Id = 1, e.B.C.Name, Year2 = e.B.C.Year };

        Expression<Func<A, object>> selector3 =
            e => e;

        Expression<Func<A, TestEntity>> selector4 =
            e => new TestEntity { Id = 1, Name = e.B.C.Name, Year3 = e.B.C.Year };

        Expression<Func<A, TestEntity>> selector5 =
            e => new TestEntity(1, e.B.C.Year, e.B.C.Name);

        Expression<Func<A, object>> selector6 =
            e => e.B.C.Name;

        Expression<Func<A, object>> selector7 =
            e => e.B.C.Year;

        // var s1 = ExpressionPropertyHelper.GetSelectorPropertyPaths(selector1);
        // var s2 = ExpressionPropertyHelper.GetSelectorPropertyPaths(selector2);
        // // var s3 = ExpressionPropertyHelper.GetSelectorPropertyPaths(selector3);
        // var s4 = ExpressionPropertyHelper.GetSelectorPropertyPaths(selector4);
        // var s5 = ExpressionPropertyHelper.GetSelectorPropertyPaths(selector5);
        // var s6 = ExpressionPropertyHelper.GetSelectorPropertyPaths(selector6);
        // var s7 = ExpressionPropertyHelper.GetSelectorPropertyPaths(selector7);

        // var s1d = ExpressionPropertyHelper.DeconstructSelector(selector1);
        // var s2d = ExpressionPropertyHelper.DeconstructSelector(selector2);
        // var s3d = ExpressionPropertyHelper.DeconstructSelector(selector3);
        // var s4d = ExpressionPropertyHelper.DeconstructSelector(selector4);
        // var s5d = ExpressionPropertyHelper.DeconstructSelector(selector5);
        // var s6d = ExpressionPropertyHelper.DeconstructSelector(selector6);
        // var s7d = ExpressionPropertyHelper.DeconstructSelector(selector7);
    }
}
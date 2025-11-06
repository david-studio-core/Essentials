using DavidStudio.Core.DataIO.Repositories;

namespace DavidStudio.Core.DataIO.Benchmarks.Assets;

public class TestRepository(TestDbContext context) : BaseRepository<TestEntity, int>(context);
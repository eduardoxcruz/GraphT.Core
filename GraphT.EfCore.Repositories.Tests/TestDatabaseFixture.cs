using Microsoft.EntityFrameworkCore;

namespace GraphT.EfCore.Repositories.Tests;

public class TestDatabaseFixture
{
	private static readonly object _lock = new();
	private static bool _databaseInitialized;
	public EfDbContext CreateContext() => new(new DbContextOptionsBuilder<EfDbContext>()
		.UseSqlServer("Server=localhost;Database=Testing;User Id=sa;Password=DevPassword123_;Encrypt=False")
		.Options);

	public TestDatabaseFixture()
	{
		lock (_lock)
		{
			if (_databaseInitialized)
			{
				return;
			}

			using (EfDbContext context = CreateContext())
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();
			}

			_databaseInitialized = true;
		}
	}
}

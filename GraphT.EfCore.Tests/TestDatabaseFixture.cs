using Microsoft.EntityFrameworkCore;

namespace GraphT.EfCore.Tests;

public class TestDatabaseFixture
{
	private const string ConnectionStringName = "EfDb:TestingString";
	
	private static readonly object _lock = new();
	private static bool _databaseInitialized;
	public EfDbContext CreateContext() => new(new DbContextOptionsBuilder<EfDbContext>()
		.UseSqlServer(ConnectionStringBuilder.GetConnectionString(ConnectionStringName))
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

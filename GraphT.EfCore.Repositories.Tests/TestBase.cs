using Microsoft.EntityFrameworkCore;

namespace GraphT.EfCore.Repositories.Tests;

public class TestBase : IDisposable
{
	protected readonly EfDbContext _context;

	public TestBase()
	{
		var options = new DbContextOptionsBuilder<EfDbContext>()
			.UseSqlServer("Server=localhost;Database=Testing;User Id=sa;Password=DevPassword123_;Encrypt=False")
			.Options;
		_context = new EfDbContext(options);
		ResetDatabase();
	}

	private void ResetDatabase()
	{
		_context.Database.EnsureDeleted();
		_context.Database.EnsureCreated();
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}

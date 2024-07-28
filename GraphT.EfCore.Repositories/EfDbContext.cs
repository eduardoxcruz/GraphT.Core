using Microsoft.EntityFrameworkCore;

namespace GraphT.EfCore.Repositories;

public partial class EfDbContext : DbContext
{
	public EfDbContext(DbContextOptions<EfDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EfDbContext>
{
	public EfDbContext CreateDbContext(string[] args)
	{
		
		DbContextOptionsBuilder<EfDbContext> builder = new();
		builder.UseSqlServer("Server=localhost;Database=Testing;User Id=sa;Password=DevPassword123_;Encrypt=False");
		return new EfDbContext(builder.Options);
	}
}

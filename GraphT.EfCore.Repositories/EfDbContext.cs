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

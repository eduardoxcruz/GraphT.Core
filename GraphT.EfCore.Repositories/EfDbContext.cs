using GraphT.EfCore.Repositories.EntityTypeConfigurations;
using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GraphT.EfCore.Repositories;

public partial class EfDbContext : DbContext
{
	public DbSet<TodoTask> TodoTasks { get; set; }
	public DbSet<TaskLog> TaskLogs { get; set; }
	public DbSet<LifeArea> LifeAreas { get; set; }
	
	public EfDbContext(DbContextOptions<EfDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		new TodoTaskEntityTypeConfiguration().Configure(modelBuilder.Entity<TodoTask>());
		new TaskLogEntityTypeConfiguration().Configure(modelBuilder.Entity<TaskLog>());
		new LifeAreaEntityTypeConfiguration().Configure(modelBuilder.Entity<LifeArea>());
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

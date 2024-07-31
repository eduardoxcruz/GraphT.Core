using GraphT.EfCore.Repositories.EntityTypeConfigurations;
using GraphT.EfCore.Repositories.Models;
using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GraphT.EfCore.Repositories;

public partial class EfDbContext : DbContext
{
	public DbSet<TodoTask> TodoTasks { get; set; }
	public DbSet<TaskAggregate> TaskAggregates { get; set; }
	public DbSet<TodoTaskStream> TaskStreams { get; set; }
	public DbSet<TaskLog> TaskLogs { get; set; }
	
	public EfDbContext(DbContextOptions<EfDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		new TodoTaskEntityTypeConfiguration().Configure(modelBuilder.Entity<TodoTask>());
		new TaskAggregateEntityTypeConfiguration().Configure(modelBuilder.Entity<TaskAggregate>());
		new TodoTaskStreamsEntityTypeConfiguration().Configure(modelBuilder.Entity<TodoTaskStream>());
		new TaskLogEntityTypeConfiguration().Configure(modelBuilder.Entity<TaskLog>());
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

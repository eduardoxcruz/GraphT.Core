using GraphT.EfCore.EntityTypeConfigurations;
using GraphT.EfCore.Models;
using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GraphT.EfCore;

public partial class EfDbContext : DbContext
{
    public DbSet<TodoTask> TodoTasks { get; set; }
    public DbSet<TaskLog> TaskLogs { get; set; }
    public DbSet<TaskStream> TaskStreams { get; set; }
    public DbSet<LifeArea> LifeAreas { get; set; }
    public DbSet<TaskLifeArea> TaskLifeAreas { get; set; }
    
    public EfDbContext(DbContextOptions<EfDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new TodoTaskEntityTypeConfiguration().Configure(modelBuilder.Entity<TodoTask>());
        new TaskLogEntityTypeConfiguration().Configure(modelBuilder.Entity<TaskLog>());
        new TaskStreamEntityTypeConfiguration().Configure(modelBuilder.Entity<TaskStream>());
        new LifeAreaEntityTypeConfiguration().Configure(modelBuilder.Entity<LifeArea>());
        new TaskLifeAreaEntityTypeConfiguration().Configure(modelBuilder.Entity<TaskLifeArea>());
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EfDbContext>
{
	public EfDbContext CreateDbContext(string[] args)
	{
		string connectionString = ConnectionStringBuilder.GetConnectionString(args[0]);
        
		DbContextOptionsBuilder<EfDbContext> optionsBuilder = new();
		optionsBuilder.UseSqlServer(connectionString);
        
		return new EfDbContext(optionsBuilder.Options);
	}
}

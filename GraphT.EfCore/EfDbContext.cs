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
	private const string ConnectionStringName = "EfDb:ConnectionString";
    
	public EfDbContext CreateDbContext(string[] args)
	{
		IConfigurationRoot configuration = BuildConfiguration();
		string connectionString = GetConnectionString(configuration);
        
		DbContextOptionsBuilder<EfDbContext> optionsBuilder = new();
		optionsBuilder.UseSqlServer(connectionString);
        
		return new EfDbContext(optionsBuilder.Options);
	}

	private static IConfigurationRoot BuildConfiguration()
	{
		return new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true)
			.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
			.AddUserSecrets<DesignTimeDbContextFactory>()
			.AddEnvironmentVariables()
			.Build();
	}

	private static string GetConnectionString(IConfiguration configuration)
	{
		string? connectionString = configuration[ConnectionStringName];
        
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new InvalidOperationException(
				$"Connection string '{ConnectionStringName}' not found. " +
				"Configure it using: " +
				"dotnet user-secrets set \"EfDb:ConnectionString\" \"your-connection-string\" " +
				"or in environment variables.");
		}
        
		return connectionString;
	}
}

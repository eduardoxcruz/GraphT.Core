using Microsoft.Extensions.Configuration;

namespace GraphT.EfCore;

public static class ConnectionStringBuilder
{
	public static string GetConnectionString(string connectionStringName)
	{
		IConfiguration configuration = BuildConfiguration();
			
		string? connectionString = configuration[connectionStringName];
        
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new InvalidOperationException(
				$"Connection string '{connectionStringName}' not found. " +
				"Configure it using: " +
				"dotnet user-secrets set \"EfDb:ConnectionString\" \"your-connection-string\" " +
				"or in environment variables.");
		}
        
		return connectionString;
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
}

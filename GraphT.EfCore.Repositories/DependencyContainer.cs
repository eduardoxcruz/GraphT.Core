using System.Diagnostics;

using GraphT.EfCore.Repositories.Repositories;
using GraphT.Model.Services.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SeedWork;

namespace GraphT.EfCore.Repositories;

public static class DependencyContainer
{
    public static IServiceCollection AddGraphTEfCoreRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
	    string? connectionString = configuration["EfDb:ConnectionString"];
        
	    if (string.IsNullOrEmpty(connectionString))
	    {
		    throw new InvalidOperationException(
			    "Connection string 'EfDb:ConnectionString' not found. " +
			    "Configure it using: " +
			    "dotnet user-secrets set \"EfDb:ConnectionString\" \"your-connection-string\" " +
			    "or in environment variables."
		    );
	    }

	    services.AddDbContext<EfDbContext>(options => options
		    .UseSqlServer(connectionString)
		    .LogTo(message => Debug.WriteLine(message), LogLevel.Warning));

        services.AddScoped<ILifeAreasRepository, LifeAreasRepository>();
        services.AddScoped<ITaskDownstreamsRepository, TaskDownstreamsRepository>();
        services.AddScoped<ITaskLogRepository, TaskLogRepository>();
        services.AddScoped<ITaskUpstreamsRepository, TaskUpstreamsRepository>();
        services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

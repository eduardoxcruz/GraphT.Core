using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SeedWork;

namespace GraphT.EfCore.Repositories;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTEfCoreRepositories(
		this IServiceCollection services,
		IConfiguration configuration,
		string connectionString)
	{
		services.AddDbContext<EfDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString(connectionString)));
		services.AddScoped<IRepository<TaskAggregate>, Repository<TaskAggregate>>();
		services.AddScoped<IRepository<TaskLog>, Repository<TaskLog>>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}

using System.Diagnostics;

using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

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
		IConfiguration configuration,
		string connectionString)
	{
		services.AddDbContext<EfDbContext>(options => options
			.UseSqlServer(configuration[$"ConnectionStrings:{connectionString}"])
			.LogTo(message => Debug.WriteLine(message), LogLevel.Warning));
		services.AddScoped<IRepository<TodoTask>, Repository<TodoTask>>();
		services.AddScoped<IRepository<TaskLog>, Repository<TaskLog>>();
		services.AddScoped<IRepository<LifeArea>, Repository<LifeArea>>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}
